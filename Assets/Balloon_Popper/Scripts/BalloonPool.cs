using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    // Manages a pool of balloon objects for efficient reuse.
    public class BalloonPool : MonoBehaviour
    {
        [SerializeField]
        private BalloonFactory factory = null;
        [SerializeField]
        private int initialPoolSizePerType = 10;
        [SerializeField]
        private List<SOBalloonData> balloonsToGenerate = new();


        public readonly Dictionary<string, Queue<GameObject>> Balloons = new();


        public static BalloonPool Instance { get; private set; }


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

            factory = factory == null ? this.GetComponentInParent<BalloonFactory>() : factory;

            if (factory == null)
            {
                Debug.LogErrorFormat("BalloonFactory reference is missing in BalloonPool: {0} | ID: {1}",
                this.gameObject.name,
                this.gameObject.GetEntityId());
            }

            // Make sure the balloon pool is clean before instantiation.
            if (Balloons.Count != 0)
            {
                Balloons.Clear();
            }

            // Instantiate the balloon pool.
            if (!TryInstantiatePool())
            {
                Debug.LogErrorFormat("Failed to instantiate BalloonPool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }


        public void ReturnToPool(Balloon balloon)
        {
            string balloonType = balloon.Data.name;

            // Ensure the balloon type exists in the pool.
            if (!Balloons.ContainsKey(balloonType))
            {
                Balloons[balloonType] = new Queue<GameObject>();
            }

            // Deactivate and enqueue the balloon back into the pool.
            if (balloon.gameObject.activeInHierarchy)
                balloon.gameObject.SetActive(false);

            Balloons[balloonType].Enqueue(balloon.gameObject);

            // Reparent the balloon to the pool and reset its position for organization.
            if (balloon.transform.parent != this.transform)
                balloon.transform.SetParent(this.transform);

            balloon.transform.localPosition = Vector3.zero;
        }

        public bool TryRetrieveFromPool(SOBalloonData balloonData, out GameObject balloon)
        {
            balloon = null;
            string balloonType = balloonData.name;

            // Check if the balloon type exists in the pool.
            if (!Balloons.ContainsKey(balloonType))
            {
                Debug.LogErrorFormat("Balloon type not found in Balloon Pool: {0} | ID: {1}",
                    balloonType,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Check if the balloon type has available balloons.
            if (Balloons[balloonType].Count == 0)
            {
                Debug.LogWarningFormat("No available balloons of type: {0} | ID: {1}",
                    balloonType,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Dequeue a balloon from the pool and activate it.
            balloon = Balloons[balloonType].Dequeue();
            balloon.SetActive(true);
            return true;
        }


        private bool TryInstantiatePool()
        {
            foreach (var balloonData in balloonsToGenerate)
            {
                //! Balloon type is defined by the name of the BalloonData scriptable object.
                string balloonType = balloonData.name;

                Balloons[balloonType] = new Queue<GameObject>();

                for (int i = 0; i < initialPoolSizePerType; i++)
                {
                    // Create a new balloon using the factory.
                    if (!factory.TryCreate(balloonData, out GameObject newBalloonObject))
                    {
                        break;
                    }

                    // Make sure the balloon is inactive when added to the pool.
                    newBalloonObject.SetActive(false);

                    // Name and parent the balloon for organization.
                    newBalloonObject.name = $"{balloonType}_Pooled_{i}";
                    newBalloonObject.transform.SetParent(this.transform);
                    newBalloonObject.transform.localPosition = Vector3.zero;

                    // Enqueue the new balloon into the pool.
                    Balloons[balloonType].Enqueue(newBalloonObject);
                }
            }

            // Return true if all balloon types were instantiated.
            return Balloons.Count == balloonsToGenerate.Count;
        }
    }
}