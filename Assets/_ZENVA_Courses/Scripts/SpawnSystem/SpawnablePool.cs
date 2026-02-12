using ObjectPools;
using Project.Tools.DictionaryHelp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpawnSystem
{
    // Manages a pool of balloon objects for efficient reuse.
    [DisallowMultipleComponent]
    public class SpawnablePool : MonoBehaviour, IObjectPool<ISpawnable, ISpawnableData>
    {
        private const int DefaultInitialCapacityPerSpawnableType = 20;
        
        private readonly Dictionary<Guid, Stack<ISpawnable>> _spawnables = new();

        private bool _isInitialized = false;
        private SpawnableFactory _factory = null;

        [SerializeField]
        private PoolScope scope = PoolScope.Global;
        [SerializeField]
        private SerializableDictionary<SpawnableDataSO, int> PoolSizePerSpawnableType = default;

        public bool IsInitialized => _isInitialized;
        public PoolScope Scope => scope;


        #region Unity Lifecycle Methods
        private void Awake()
        {
            _factory ??= new(this);
            _isInitialized = TryInitializeAllTypePools();
        }

        private void OnDestroy()
        {
            TryReturnAll();
            _spawnables.Clear();
            _isInitialized = false;
        }
        #endregion


        #region Private Methods
        private bool TryInitializeAllTypePools()
        {
            foreach (var kvp in PoolSizePerSpawnableType)
            {
                if (!TryInitializeTypePool(kvp.Key, kvp.Value))
                {
                    Debug.LogErrorFormat("Failed to initialize Spawnable type: {0} | ID: {1} " +
                        "\nin SpawnablePool: {2} | ID: {3}",
                        kvp.Key.InstanceName,
                        kvp.Key.ID,
                        this.gameObject.name,
                        this.gameObject.GetEntityId());
                    return false;
                }
            }

            return true;
        }
        #endregion


        #region Public Methods
        public ISpawnable GetOrCreate(ISpawnableData data)
        {
            if (!TryGet(data.ID, out ISpawnable spawnable))
            {
                if (_factory.TryCreate(data, out spawnable))
                {
                    _spawnables[spawnable.Data.ID].Push(spawnable);
                }
            }

            return spawnable;
        }

        public bool TryGet(Guid id, out ISpawnable spawnable)
        {
            spawnable = null;

            if (!_spawnables.ContainsKey(id))
            {                 
                Debug.LogWarningFormat("Spawnable type with ID: {0}" +
                    "\nnot found in SpawnablePool: {1} | ID: {2}",
                    id,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            return _spawnables[id].TryPop(out spawnable);
        }

        public bool TryGet(ISpawnableData data, out ISpawnable spawnable)
        {
            return TryGet(data.ID, out spawnable);
        }

        public bool TryInitializeTypePool(ISpawnableData data, int initialCapacity = DefaultInitialCapacityPerSpawnableType)
        {
            if (data == null)
            {
                Debug.LogErrorFormat("Attempted to initialize SpawnPool with null data: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Ensure the initial capacity is greater than zero to prevent creating an empty pool.
            if (initialCapacity <= 0)
            {
                Debug.LogWarningFormat("Attempted to initialize SpawnablePool with invalid capacity: {0} | ID: {1} " +
                    "\nCapacity must be greater than zero. Given capacity: {2}" +
                    "\nSetting capacity to default: {3}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId(),
                    initialCapacity,
                    DefaultInitialCapacityPerSpawnableType);

                initialCapacity = DefaultInitialCapacityPerSpawnableType;
            }

            // Ensure the initial capacity is at least the default to prevent under-populating the pool.
            if (initialCapacity != DefaultInitialCapacityPerSpawnableType)
            {
                if (initialCapacity < DefaultInitialCapacityPerSpawnableType)
                {
                    Debug.LogWarningFormat("Attempted to initialize SpawnablePool with custom capacity: {0} | ID: {1} " +
                    "\nGiven capacity is less than default capacity: {2}" +
                    "\nSetting capacity to default: {3}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId(),
                    initialCapacity,
                    DefaultInitialCapacityPerSpawnableType);
                }

                initialCapacity = Mathf.Max(initialCapacity, DefaultInitialCapacityPerSpawnableType);
            }

            // Prevent duplicate initialization for the same Spawnable type by checking if the type already exists in the pool.
            if (!_spawnables.TryAdd(data.ID, new Stack<ISpawnable>(initialCapacity)))
            {
                Debug.LogWarningFormat("Failed to initialize Spawnable type: {0} | ID: {1} " +
                    "\nbecause type already in SpawnablePool: {2} | ID: {3}",
                    data.InstanceName,
                    data.ID,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Populate the pool with the initial capacity of balloons for the given type.
            for (int i = 0; i < _spawnables[data.ID].Count; i++)
            {
                if (!_factory.TryCreate(data, out ISpawnable spawnable))
                {
                    Debug.LogErrorFormat("Failed to create Spawnable of type: {0} | ID: {1} " +
                        "\nin SpawnablePool during initialization: {2} | ID: {3}",
                        data.InstanceName,
                        data.ID,
                        this.gameObject.name,
                        this.gameObject.GetEntityId());
                    return false;
                }

                _spawnables[data.ID].Push(spawnable);
            }

            // Verify that the pool was populated with the expected number of balloons for the given type.
            if (_spawnables[data.ID] == null ||
                _spawnables[data.ID].Count == 0 ||
                _spawnables[data.ID].Count < initialCapacity)
            {
                Debug.LogErrorFormat("Failed to initialize Spawnable type: {0} | {1} " +
                    "\nin SpawnablePool: {2} | ID: {3}",
                    data.InstanceName,
                    data.ID,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            return true;
        }
        
        public bool TryReturn(ISpawnable spawnable)
        {
            Guid spawnableTypeID = spawnable.Data.ID;

            // Ensure the Spawnable type exists in the pool.
            if (!_spawnables.ContainsKey(spawnableTypeID))
            {
                Debug.LogErrorFormat("Spawnable type: {0} | ID: {1}" +
                    "\nnot found in SpawnablePool: {2} | ID: {3}",
                    spawnable.Data.InstanceName,
                    spawnableTypeID,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Deactivate and push the spawnable back into the pool.
            if (spawnable.GameObject.activeInHierarchy)
                spawnable.GameObject.SetActive(false);
            _spawnables[spawnableTypeID].Push(spawnable);

            // Reparent the spawnable to the pool and reset its position for organization.
            if (spawnable.GameObject.transform.parent != this.transform)
                spawnable.GameObject.transform.SetParent(this.transform, false);
            spawnable.GameObject.transform.localPosition = Vector3.zero;

            return true;
        }

        public bool TryReturnAll()
        {
            foreach (var spawnableStack in _spawnables.Values)
            {
                foreach (var spawnable in spawnableStack)
                {
                    if (spawnable.State != IToggleable.ToggleState.Active)
                    {
                        break;
                    }

                    if (!TryReturn(spawnable))
                    {
                        Debug.LogErrorFormat("Failed to return Spawnable: {0} | ID: {1} " +
                            "\nto SpawnablePool: {2} | ID: {3}",
                            spawnable.GameObject.name,
                            spawnable.GameObject.GetEntityId(),
                            this.gameObject.name,
                            this.gameObject.GetEntityId());

                        return false;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}