using Core;
using System;
using UnityEngine;

namespace SpawnSystem
{
    /// <summary>
    /// Factory responsible for creating Spawnable GameObjects from ISpawnableData.
    /// </summary>
    public class SpawnableFactory : IFactory<ISpawnable, ISpawnableData>
    {
        private readonly Spawner _myClient = null;

        public SpawnableFactory(Spawner objectPool)
        {
            if (objectPool != null)
            {
                _myClient = objectPool;
            }
            else
            {
                throw new ArgumentNullException(nameof(objectPool));
            }
        }

        public bool TryCreate(ISpawnableData data, out ISpawnable newSpawnable)
        {
            newSpawnable = null;

            // Validate input data
            if (data == null || data.Prefab == null)
            {
                Debug.LogErrorFormat("Cannot create Spawnable for Pool: {0} | ID: {1}" +
                    "\nDataProvider is invalid: {2} | ID: {3}",
                    _myClient.name,
                    _myClient.GetEntityId(),
                    data?.InstanceName,
                    data?.ID);
                return false;
            }

            // Instantiate the prefab
            var newSpawnableObject = UnityEngine.Object.Instantiate(
                original: data.Prefab,
                position: _myClient.transform.position,
                rotation: _myClient.transform.rotation,
                parent: _myClient.transform);

            // Check if instantiation was successful
            if (newSpawnableObject == null)
            {
                Debug.LogErrorFormat("Failed to instantiate Spawnable from prefab: {0} | ID: {1}",
                    data.Prefab.name,
                    data.Prefab.GetEntityId());
                return false;
            }

            // Ensure the new object has the required component
            if (!TryGetSpawnableComponent(newSpawnableObject, out newSpawnable))
            {
                Debug.LogErrorFormat("No Component implementing ISpawnable found on instantiated prefab: {0} | ID: {1}",
                        data.Prefab.name,
                        data.Prefab.GetEntityId());

                    return false;
            }

            // Try to initialize the new object instance
            if (!newSpawnable.TryInitialize(data))
            {
                // Destroy the instance if initialization fails
                UnityEngine.Object.Destroy(newSpawnableObject);
                return false;
            }

            newSpawnableObject.name = $"{data.InstanceName} ({newSpawnableObject.GetEntityId()})";
            newSpawnableObject.SetActive(false);

            return true;
        }

        private static bool TryGetSpawnableComponent(GameObject obj, out ISpawnable spawnable)
        {
            return obj.TryGetComponent(out spawnable) ||
                (spawnable = obj.GetComponentInChildren<ISpawnable>()) != null;
        }
    }

}