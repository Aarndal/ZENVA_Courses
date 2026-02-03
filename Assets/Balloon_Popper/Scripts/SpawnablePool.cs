using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace BalloonPopper
{
    // Manages a pool of balloon objects for efficient reuse.
    [RequireComponent(typeof(SpawnableFactory))]
    [DisallowMultipleComponent]
    public class SpawnablePool : MonoBehaviour, IObjectPool<ISpawnable, IDataProvider<ISpawnable>>
    {
        private const int INITIAL_CAPACITY_PER_TYPE = 20;

        public readonly Dictionary<string, Stack<ISpawnable>> Spawnables = new();

        private bool _isInitialized = false;
        private SpawnableFactory _factory = null;


        #region Unity Lifecycle Methods
        private void Awake()
        {
            if (_factory == null)
            {
                if (!this.TryGetComponent(out _factory))
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat("No Factory component added to SpawnablePool: {0} | ID: {1}",
                        this.gameObject.name,
                        this.gameObject.GetEntityId());
#endif
                    this.gameObject.AddComponent<SpawnableFactory>();
                }
            }
        }
        #endregion


        #region Public Methods
        public bool TryReturn<T>(T obj) where T : class
        {
            if (!_isInitialized)
            {
                Debug.LogErrorFormat("Attempted to return object to uninitialized Pool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (obj is ISpawnable spawnable)
            {
                return TryReturn(spawnable);
            }

            Debug.LogErrorFormat("Invalid object type ({0}) returned to Pool: {1} | ID: {2}",
                obj.GetType().Name,
                this.gameObject.name,
                this.gameObject.GetEntityId());

            return false;
        }

        public bool TryReturn(ISpawnable spawnable)
        {
            if (!_isInitialized)
            {
                Debug.LogErrorFormat("Attempted to return object to uninitialized Pool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            string spawnableType = spawnable.Data.InstanceName;

            // Ensure the balloon type exists in the pool.
            if (!Spawnables.ContainsKey(spawnableType))
            {
                Debug.LogErrorFormat("Balloon type ({0}) not found in Pool: {1} | ID: {2}",
                    spawnableType,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Deactivate and push the balloon back into the pool.
            if (spawnable.GameObject.activeInHierarchy)
                spawnable.GameObject.SetActive(false);

            Spawnables[spawnableType].Push(spawnable);

            // Reparent the balloon to the pool and reset its position for organization.
            if (spawnable.GameObject.transform.parent != this.transform)
                spawnable.GameObject.transform.SetParent(this.transform);

            spawnable.GameObject.transform.localPosition = Vector3.zero;

            return true;
        }

        public bool TryGet(IDataProvider<ISpawnable> spawnableData, out ISpawnable spawnable)
        {
            spawnable = null;

            if (!_isInitialized)
            {
                Debug.LogErrorFormat("Attempted to get object from uninitialized Pool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            string spawnableType = spawnableData.InstanceName;

            // Check if the balloon type exists in the pool.
            if (!Spawnables.ContainsKey(spawnableType))
            {
                Debug.LogErrorFormat("Balloon type ({0}) not found in Pool: {1} | ID: {2}",
                    spawnableType,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Check if the balloon type has available balloons.
            if (Spawnables[spawnableType].Count == 0)
            {
                Debug.LogWarningFormat("No available balloons of type ({0}) in Pool: {1} | ID: {2}",
                    spawnableType,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());

                if(!TryPopulateSpawnablePool((SpawnableDataProviderSO)spawnableData))
                    return false;
            }

            // Get a balloon from the pool and activate it.
            spawnable = Spawnables[spawnableType].Pop();
            spawnable.GameObject.SetActive(true);
            return true;
        }

        public bool TryInitializePool(IEnumerable<SpawnableDataProviderSO> spawnableDataProviders)
        {
            // Make sure the balloon pool is clean before initialization.
            if (Spawnables.Count != 0)
            {
                Spawnables.Clear();
            }

            _isInitialized = false;

            foreach (var spawnableData in spawnableDataProviders)
            {
                //! Balloon type is defined by the name of the BalloonData scriptable object.
                string spawnableType = spawnableData.InstanceName;

                Spawnables[spawnableData.InstanceName] = new();

                if(!TryPopulateSpawnablePool(spawnableData))
                    return false;
            }

            // Is initialized if all balloon types were instantiated.
            _isInitialized = Spawnables.Count == spawnableDataProviders.Count();

            return _isInitialized;
        }
        #endregion


        #region Private Methods
        private bool TryPopulateSpawnablePool(SpawnableDataProviderSO spawnableData, int capacity = INITIAL_CAPACITY_PER_TYPE)
        {
            if(capacity != INITIAL_CAPACITY_PER_TYPE)
                capacity = Mathf.Max(capacity, INITIAL_CAPACITY_PER_TYPE);

            for (int i = 0; i < capacity; i++)
            {
                // Create a new balloon using the factory.
                if (!_factory.TryCreate(spawnableData, out ISpawnable newSpawnable))
                {
                    Debug.LogErrorFormat("Failed to create Spawnable: {0} | ID: {1}\nfor Pool: {2} | ID: {3}",
                        spawnableData.InstanceName,
                        spawnableData.GetEntityId(),
                        this.gameObject.name,
                        this.gameObject.GetEntityId());
                    return false;
                }

                // Try to assign this pool to the new balloon for future returns.
                if (!newSpawnable.TryAssignPool(this))
                {
                    Debug.LogErrorFormat("Failed to assign Pool to Spawnable (): {0} | ID: {1}",
                        newSpawnable.GameObject.name,
                        newSpawnable.GameObject.GetEntityId());
                    return false;
                }

                // Make sure the balloon is inactive when added to the pool.
                newSpawnable.GameObject.SetActive(false);

                // Name and parent the balloon for organization.
                newSpawnable.GameObject.name = $"{spawnableData.InstanceName}_{this.gameObject.name}_{i}";
                newSpawnable.GameObject.transform.SetParent(this.transform, false);

                // Enqueue the new balloon into the pool.
                Spawnables[spawnableData.InstanceName].Push(newSpawnable);
            }
            return true;
        }
        #endregion
    }
}