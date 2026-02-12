using SpawnSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BalloonPopper
{
    [Obsolete("This class is deprecated and will be removed in future versions.", false)]
    // Factory responsible for creating Object Pools for ISpawnables.
    public class SpawnablePoolFactory : MonoBehaviour, IFactory<SpawnablePool, KeyValuePair<GameObject, Stack<SpawnableDataSO>>>
    {
        [SerializeField]
        private List<SpawnableDataSO> spawnableData = new();

        public readonly Dictionary<GameObject, Stack<SpawnableDataSO>> SpawnablesToPool = new();

        private void Awake()
        {
            if (spawnableData == null || spawnableData.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("No SpawnableData assigned in SpawnablePoolFactory: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
            }

            foreach (SpawnableDataSO data in spawnableData)
            {
                if (data.Prefab == null)
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("SpawnableDataProviderSO has no Prefab assigned: {0} | ID: {1}",
                        data.InstanceName,
                        data.GetEntityId());
#endif
                    continue;
                }

                SpawnablesToPool.TryAdd(data.Prefab, new Stack<SpawnableDataSO>());
                SpawnablesToPool[data.Prefab].Push(data);
            }

            int i = -1;

            foreach (var prefab in SpawnablesToPool)
            {
                if (!TryCreate(prefab, out SpawnablePool newSpawnablePool))
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("Failed to create SpawnablePool for Prefab: {0} | ID: {1}",
                        prefab.Key.name,
                        prefab.Key.GetEntityId());
#endif
                    continue;
                }

                newSpawnablePool.transform.position = this.transform.position + new Vector3(0, i, 0);
                i--;
            }
        }


        public bool TryCreate(KeyValuePair<GameObject, Stack<SpawnableDataSO>> dataCollection, out SpawnablePool newSpawnablePool)
        {
            newSpawnablePool = null;

            // Validate input data
            if (dataCollection.Key == null || dataCollection.Value.Any(data => data == null || data.Prefab == null))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Cannot create SpawnablePool. Provided data is invalid for Prefab: {0} | ID: {1}",
                   dataCollection.Key.name,
                   dataCollection.Key.GetEntityId());
#endif
                return false;
            }

            // Instantiate the prefab
            var newSpawnableObject = Instantiate(new GameObject(), this.transform.position, Quaternion.identity, this.transform);

            // Check if instantiation was successful
            if (newSpawnableObject == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Failed to instantiate SpawnablePool for Prefab: {0} | ID: {1}",
                    dataCollection.Key.name,
                    dataCollection.Key.GetEntityId());
#endif
                return false;
            }

            newSpawnableObject.name = $"{dataCollection.Key.name}_Pool";

            if (newSpawnableObject.AddComponent<SpawnableFactory>() == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Failed to add SpawnableFactory component to SpawnablePool for Prefab: {0} | ID: {1}",
                    newSpawnableObject.name,
                    dataCollection.Key.GetEntityId());
#endif
                return false;
            }

            newSpawnablePool = newSpawnableObject.AddComponent<SpawnablePool>();

            // Try to initialize the new object instance
            if (!newSpawnablePool.TryInitializePool(dataCollection.Value))
            {
                // Destroy the instance if initialization fails
                Destroy(newSpawnableObject);
                return false;
            }

            //! Dirty hack to set the Pool reference in each SpawnableDataProviderSO for Spawner reference
            foreach (var data in dataCollection.Value)
            {
                data.Pool = newSpawnablePool;
            }

            return newSpawnableObject != null && newSpawnablePool != null;
        }
    }
}