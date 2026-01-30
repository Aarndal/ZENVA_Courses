using BalloonPopper;
using UnityEngine;

// Factory responsible for creating Balloon GameObjects from SOBalloonData.
public class BalloonFactory : MonoBehaviour, IFactory<GameObject, SOBalloonData>
{
    public bool TryCreate(SOBalloonData data, out GameObject newBalloonObject)
    {
        newBalloonObject = null;

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
        newBalloonObject = Instantiate(data.Prefab, this.transform.position, Quaternion.identity, this.transform);

        if (newBalloonObject == null)
        {
            Debug.LogErrorFormat("Failed to instantiate Balloon from prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());
            return false;
        }

        // Ensure the new balloon object has a Balloon component
        if (!newBalloonObject.TryGetComponent(out Balloon balloonComponent))
        {
            Debug.LogErrorFormat("Balloon component not found on instantiated prefab: {0} | ID: {1}",
                data.name,
                data.GetEntityId());

            balloonComponent = newBalloonObject.AddComponent<Balloon>();
        }

        // Try to initialize the balloon instance
        if (!balloonComponent.TryInitialize(data))
        {
            Destroy(newBalloonObject);
            newBalloonObject = null;
            return false;
        }

        return true;
    }
}
