using BalloonPopper;
using UnityEngine;

// Factory responsible for creating Balloon GameObjects from SOBalloonData.
public class BalloonFactory : MonoBehaviour, IFactory<Balloon, BalloonDataSO>
{
    public bool TryCreate(BalloonDataSO data, out Balloon newBalloon)
    {
        newBalloon = null;

        // Validate input data
        if (data == null || data.Prefab == null)
        {
            Debug.LogErrorFormat("Cannot create Balloon. Provided data is invalid: {0} | ID: {1}",
               data.name,
               data.GetEntityId());
            return false;
        }

        // Check if prefab has the required component
        if (!data.Prefab.TryGetComponent<Balloon>(out _))
        {
            Debug.LogErrorFormat("Balloon component not found on instantiated prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());
            return false;
        }

        // Instantiate the prefab
        var newBalloonObject = Instantiate(data.Prefab, this.transform.position, Quaternion.identity, this.transform);

        // Check if instantiation was successful
        if (newBalloonObject == null)
        {
            Debug.LogErrorFormat("Failed to instantiate Balloon from prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());
            return false;
        }

        // Ensure the new object has the required component
        if (!newBalloonObject.TryGetComponent(out newBalloon))
        {
            Debug.LogErrorFormat("Balloon component not found on instantiated prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());

            newBalloon = newBalloonObject.AddComponent<Balloon>();

            Debug.LogWarningFormat("Added Balloon component to instantiated prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());
        }

        // Try to initialize the new object instance
        if (!newBalloon.TryInitialize(data))
        {
            // Destroy the instance if initialization fails
            Destroy(newBalloonObject);
            return false;
        }

        return true;
    }
}
