using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    // Manages a pool of balloon objects for efficient reuse.
    public class SpawnablePool : MonoBehaviour, IObjectPool<ISpawnable, SpawnableDataProviderSO>
    {
        [SerializeField]
        private IFactory<ISpawnable, SpawnableDataProviderSO> factory = null;
        [SerializeField]
        private int initialPoolSizePerType = 10;
        [SerializeField]
        private List<SpawnableDataProviderSO> spawnableDataProviders = new();


        public readonly Dictionary<string, Stack<ISpawnable>> Spawnables = new();


        public static SpawnablePool Instance { get; private set; }


        #region Unity
        private void Awake()
        {
            // Ensure only one instance of BalloonPool exists.
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            factory ??= this.GetComponentInChildren<IFactory<ISpawnable, SpawnableDataProviderSO>>();

            if (factory == null)
            {
                Debug.LogErrorFormat("BalloonFactory reference is missing in BalloonPool: {0} | ID: {1}",
                this.gameObject.name,
                this.gameObject.GetEntityId());
            }

            // Make sure the balloon pool is clean before instantiation.
            if (Spawnables.Count != 0)
            {
                Spawnables.Clear();
            }

            // Instantiate the balloon pool.
            if (!TryInstantiatePool())
            {
                Debug.LogErrorFormat("Failed to instantiate Pool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }
        #endregion


        #region Public Methods
        public bool TryReturn<T>(T obj) where T : class
        {
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
            string spawnableType = spawnable.TypeName;

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

        public bool TryGet(SpawnableDataProviderSO spawnableData, out ISpawnable spawnable)
        {
            spawnable = null;
            string spawnableType = spawnableData.Name;

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
                return false;
            }

            // Get a balloon from the pool and activate it.
            spawnable = Spawnables[spawnableType].Pop();
            spawnable.GameObject.SetActive(true);
            return true;
        }
        #endregion


        #region Private Methods
        private bool TryInstantiatePool()
        {
            foreach (var spawnableData in spawnableDataProviders)
            {
                //! Balloon type is defined by the name of the BalloonData scriptable object.
                string spawnableType = spawnableData.Name;

                Spawnables[spawnableType] = new();

                for (int i = 0; i < initialPoolSizePerType; i++)
                {
                    // Create a new balloon using the factory.
                    if (!factory.TryCreate(spawnableData, out ISpawnable newSpawnable))
                    {
                        Debug.LogErrorFormat("Failed to create Spawnable: {0} | ID: {1}\nfor Pool: {2} | ID: {3}",
                            spawnableData.name,
                            spawnableData.GetEntityId(),
                            this.gameObject.name,
                            this.gameObject.GetEntityId());
                        break;
                    }

                    if (!newSpawnable.TryAssignPool(this))
                    {
                        Debug.LogErrorFormat("Failed to assign Pool to Spawnable (): {0} | ID: {1}",
                            newSpawnable.GameObject.name,
                            newSpawnable.GameObject.GetEntityId());
                    }

                    // Make sure the balloon is inactive when added to the pool.
                    newSpawnable.GameObject.SetActive(false);

                    // Name and parent the balloon for organization.
                    newSpawnable.GameObject.name = $"{spawnableType}_{this.gameObject.name}_{i}";
                    newSpawnable.GameObject.transform.SetParent(this.transform);
                    newSpawnable.GameObject.transform.localPosition = Vector3.zero;

                    // Enqueue the new balloon into the pool.
                    Spawnables[spawnableType].Push(newSpawnable);
                }
            }

            // Return true if all balloon types were instantiated.
            return Spawnables.Count == spawnableDataProviders.Count;
        }
        #endregion
    }
}