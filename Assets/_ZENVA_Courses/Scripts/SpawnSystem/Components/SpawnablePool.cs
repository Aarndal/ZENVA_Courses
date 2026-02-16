using Cysharp.Threading.Tasks;
using ObjectPools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnSystem
{
    /// <summary>
    /// Component responsible for managing a pool of spawnable entities. 
    /// It allows for efficient reuse of spawnable objects by maintaining a collection of inactive instances that can be quickly retrieved and returned to the pool as needed. 
    /// The SpawnablePool is designed to work with various types of spawnable entities, as defined by their corresponding data objects, and provides methods for initializing the pool, retrieving spawnables, and returning them after use.
    /// </summary>
    public class SpawnablePool : IObjectPool<ISpawnable, ISpawnableData>, IDisposable
    {
        /// Private Member Variables
        private const int DefaultInitialCapacityPerSpawnableType = 10;

        private readonly Dictionary<ISpawnableData, Stack<ISpawnable>> _availableSpawnablesByType = new();
        private readonly Spawner _daddy = null;
        private readonly SpawnableFactory _factory = null;
        private readonly HashSet<EntityId> _registeredEntityIDs = new();

        private bool _isDisposed = false;
        private bool _isInitialized = false;

        // Properties
        public bool IsInitialized => _isInitialized;
        public PoolScope Scope => PoolScope.Local;

        // Events
        public event Action ReturnAllRequested;

        // Constructors
        public SpawnablePool(Spawner spawner)
        {
            _daddy = spawner;
            _factory = new(spawner);
            _availableSpawnablesByType = new Dictionary<ISpawnableData, Stack<ISpawnable>>();

            var amountToSpawnPerType = new Dictionary<Guid, int>();

            // Initialize the pool for each spawnable type specified in the spawner's instructions with the maximum amount to spawn in a sequence.
            foreach (var instruction in spawner.Instructions)
            {
                // Check if the spawnable type already has a pool in the dictionary. If not, create a new stack with the specified initial capacity and add it to the dictionary.
                if (_availableSpawnablesByType.TryAdd(
                    instruction.SpawnableTypeToSpawn,
                    new Stack<ISpawnable>(instruction.AmountToSpawn)))
                {
                    amountToSpawnPerType.TryAdd(instruction.SpawnableTypeToSpawn.ID, instruction.AmountToSpawn);

                    continue;
                }

                // Ensure the pool has enough capacity for the specified amount to spawn for each type. If not, add more capacity to the existing stack.
                if (instruction.AmountToSpawn > amountToSpawnPerType[instruction.SpawnableTypeToSpawn.ID])
                {
                    _availableSpawnablesByType.EnsureCapacity(instruction.AmountToSpawn);
                    amountToSpawnPerType[instruction.SpawnableTypeToSpawn.ID] = instruction.AmountToSpawn;
                }
            }

            // Initialize the pool for each spawnable type with the specified initial capacity.
            foreach (var spawnable in _availableSpawnablesByType)
            {
                if (!TryInitializeTypePool(spawnable.Key, amountToSpawnPerType[spawnable.Key.ID]))
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat(
                        "Failed to initialize Spawnable type: {0} | ID: {1} " +
                        "\nfor Spawner: {2} | ID: {3}",
                        spawnable.Key.InstanceName,
                        spawnable.Key.ID,
                        spawner.gameObject.name,
                        spawner.gameObject.GetEntityId());
#endif
                }
            }

            _isInitialized = true;
        }


        #region Private Methods
        /// <summary>
        /// Adds a spawnable instance to the pool by deactivating it and pushing it onto the stack of available spawnables for its corresponding type.
        /// </summary>
        /// <param name="spawnable">The spawnable instance to add to the pool.</param>
        private void AddToPool(ISpawnable spawnable)
        {
            spawnable.GameObject.SetActive(false);
            _availableSpawnablesByType[spawnable.Data].Push(spawnable);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                ReturnAllRequested?.Invoke();

                if (disposing)
                {
                    _isInitialized = false;
                }

                foreach (var spawnableStack in _availableSpawnablesByType.Values)
                {
                    foreach (var spawnable in spawnableStack)
                    {
                        if (spawnable != null && spawnable.GameObject != null)
                        {
                            UnityEngine.Object.Destroy(spawnable.GameObject);
                        }
                    }
                }

                // set large fields to null
                _availableSpawnablesByType.Clear();
                _registeredEntityIDs.Clear();

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Tries to register a newly created spawnable instance in the pool.
        /// </summary>
        /// <param name="spawnable">The spawnable instance to register.</param>
        /// <returns>true if the spawnable was successfully registered; otherwise, false.</returns>
        private bool TryRegister(ISpawnable spawnable)
        {
            if (!_registeredEntityIDs.Add(spawnable.GameObject.GetEntityId()))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat(
                    "Attempted to register already registered Spawnable: {0} | ID: {1}" +
                    "\nSpawnable Type: {2} | ID: {3}" +
                    "\nSpawner: {4} | ID: {5}",
                    spawnable.GameObject.name,
                    spawnable.GameObject.GetEntityId(),
                    spawnable.Data.InstanceName,
                    spawnable.Data.ID,
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId());
#endif
                return false;
            }

            spawnable.DespawnRequested += TryReturn;
            ReturnAllRequested += spawnable.Despawn;
            return true;
        }
        #endregion


        #region Public Methods
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        
        public bool TryGet(ISpawnableData data, out ISpawnable spawnable)
        {
            spawnable = null;

            if (!_availableSpawnablesByType.ContainsKey(data))
            {
                Debug.LogWarningFormat("Spawnable type: {0} | ID: {1}" +
                    "\nnot found in SpawnablePool of Spawner: {2} | ID: {3}",
                    data.InstanceName,
                    data.ID,
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId());
                return false;
            }

            return _availableSpawnablesByType[data].TryPop(out spawnable);
        }

        public bool TryGet(Guid id, out ISpawnable spawnable)
        {
            var data = _availableSpawnablesByType.Keys.
                FirstOrDefault(data => data.ID == id);

            return TryGet(data, out spawnable);
        }

        public bool TryGetOrCreate(ISpawnableData data, out ISpawnable spawnable)
        {
            if (TryGet(data.ID, out spawnable))
            {
                return true;
            }

            if (_factory.TryCreate(data, out spawnable))
            {
                return TryRegister(spawnable);
            }

            return false;
        }

        /// <summary>
        /// Attempts to initialize the pool for a specific spawnable type by creating a fixed number of instances based on the provided data.
        /// </summary>
        /// <param name="data">The spawnable data used to create instances.</param>
        /// <param name="initialCapacity">The initial number of instances to create in the pool.</param>
        /// <returns>true if the pool was successfully initialized; otherwise, false.</returns>
        public bool TryInitializeTypePool(ISpawnableData data, int initialCapacity = DefaultInitialCapacityPerSpawnableType)
        {
            if (data == null)
            {
                Debug.LogErrorFormat("Attempted to initialize SpawnPool with null data: {0} | ID: {1}",
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId());
                return false;
            }

            // Ensure the initial capacity is greater than zero to prevent creating an empty pool.
            if (initialCapacity <= 0)
            {
                Debug.LogWarningFormat("Attempted to initialize SpawnablePool with invalid capacity: {0} | ID: {1} " +
                    "\nCapacity must be greater than zero. Given capacity: {2}" +
                    "\nSetting capacity to default: {3}",
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId(),
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
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId(),
                    initialCapacity,
                    DefaultInitialCapacityPerSpawnableType);
                }

                initialCapacity = Mathf.Max(initialCapacity, DefaultInitialCapacityPerSpawnableType);
            }

            // Populate the pool with the initial capacity and verify that each instance was successfully created and registered.
            for (int i = 0; i < initialCapacity; i++)
            {
                if (!_factory.TryCreate(data, out ISpawnable spawnable))
                {
                    Debug.LogErrorFormat("Failed to create Spawnables of type: {0} | ID: {1} " +
                        "\nfor Spawner: {2} | ID: {3}",
                        data.InstanceName,
                        data.ID,
                        _daddy.gameObject.name,
                        _daddy.gameObject.GetEntityId());
                    return false;
                }

                if (!TryRegister(spawnable)) return false;

                AddToPool(spawnable);
            }

            // Verify that the pool was populated with the expected number of entities for the given type, and that all entities are properly registered in the pool.
            if (_availableSpawnablesByType[data] == null ||
                _availableSpawnablesByType[data].Count == 0 ||
                _availableSpawnablesByType[data].Count < initialCapacity ||
                _registeredEntityIDs == null ||
                _registeredEntityIDs.Count == 0 ||
                _availableSpawnablesByType[data].Any(spawnable =>
                !_registeredEntityIDs.Contains(spawnable.GameObject.GetEntityId())))
            {
                Debug.LogErrorFormat("Failed to initialize Spawnable type: {0} | {1} " +
                    "\nin SpawnablePool: {2} | ID: {3}",
                    data.InstanceName,
                    data.ID,
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to return a registered spawnable to the pool after use, ensuring that it is properly deactivated and reparented to the pool for organization.
        /// </summary>
        /// <param name="spawnable">The registered instance to return to the pool.</param>
        /// <returns>true if the spawnable was successfully returned to the pool; otherwise, false.</returns>
        public bool TryReturn(ISpawnable spawnable)
        {
            // Ensure the Spawnable is registered in the pool.
            if (!_registeredEntityIDs.Contains(spawnable.GameObject.GetEntityId()))
            {
                Debug.LogErrorFormat("Spawnable: {0} | ID: {1}" +
                    "\nnot registered in Pool of Spawner: {2} | ID: {3}",
                    spawnable.GameObject.name,
                    spawnable.GameObject.GetEntityId(),
                    _daddy.gameObject.name,
                    _daddy.gameObject.GetEntityId());
                return false;
            }

            AddToPool(spawnable);

            // Reparent the spawnable to the pool and reset its position for organization.
            if (spawnable.GameObject.transform.parent != _daddy.transform)
                spawnable.GameObject.transform.SetParent(_daddy.transform, false);
            spawnable.GameObject.transform.localPosition = Vector3.zero;

            return true;
        }
        #endregion
    }
}