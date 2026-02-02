using UnityEngine;

namespace BalloonPopper
{
    // Factory responsible for creating Balloon GameObjects from SOBalloonData.
    public class SpawnableFactory : MonoBehaviour, IFactory<ISpawnable, SpawnableDataProviderSO>
    {
        public bool TryCreate(SpawnableDataProviderSO data, out ISpawnable newSpawnable)
        {
            newSpawnable = null;

            // Validate input data
            if (data == null || data.Prefab == null)
            {
                Debug.LogErrorFormat("Cannot create Spawnable. Provided data is invalid: {0} | ID: {1}",
                   data.Name,
                   data.GetEntityId());
                return false;
            }

            // Check if prefab has the required component
            if (!data.Prefab.TryGetComponent<ISpawnable>(out _))
            {
                Debug.LogErrorFormat("No Component implementing ISpawnable found on instantiated prefab: {0} | ID: {1}",
                    data.Prefab.name,
                    data.Prefab.GetEntityId());
                return false;
            }

            // Instantiate the prefab
            var newSpawnableObject = Instantiate(data.Prefab, this.transform.position, Quaternion.identity, this.transform);

            // Check if instantiation was successful
            if (newSpawnableObject == null)
            {
                Debug.LogErrorFormat("Failed to instantiate Spawnable from prefab: {0} | ID: {1}",
                    data.Prefab.name,
                    data.Prefab.GetEntityId());
                return false;
            }

            // Ensure the new object has the required component
            if (!newSpawnableObject.TryGetComponent(out newSpawnable))
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
                Destroy(newSpawnableObject);
                return false;
            }

            return true;
        }
    }

}