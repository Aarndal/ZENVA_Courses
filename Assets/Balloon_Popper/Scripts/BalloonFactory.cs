using BalloonPopper;
using UnityEngine;

// Factory responsible for creating Balloon GameObjects from SOBalloonData.
public class BalloonFactory : MonoBehaviour, IFactory<Balloon, SOBalloonData>
{
    public bool TryCreate(SOBalloonData data, out Balloon newBalloon)
    {
        newBalloon = null;

        // Validate the input data
        if (data == null || data.Prefab == null)
        {
            Debug.LogError("SOBalloonData is null. Cannot create Balloon.");

            Debug.LogErrorFormat("Cannot create Balloon. Provided data is invalid: {0} | ID: {1}",
               data.name,
               data.GetEntityId());
            return false;
        }

        // Ensure it has a Balloon component
        if (!data.Prefab.TryGetComponent<Balloon>(out _))
        {
            Debug.LogErrorFormat("Balloon component not found on instantiated prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());
            return false;
        }

        // Instantiate the balloon prefab
        var newBalloonObject = Instantiate(data.Prefab, this.transform.position, Quaternion.identity, this.transform);

        if (newBalloonObject == null)
        {
            Debug.LogErrorFormat("Failed to instantiate Balloon from prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());
            return false;
        }

        // Ensure the new balloon object has a Balloon component
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

        // Try to initialize the balloon instance
        if (!newBalloon.TryInitialize(data))
        {
            Destroy(newBalloonObject);
            return false;
        }

        return true;
    }
}
