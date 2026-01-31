using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    // Manages a pool of balloon objects for efficient reuse.
    public class BalloonPool : MonoBehaviour, IObjectPool<Balloon, BalloonDataSO>
    {
        [SerializeField]
        private BalloonFactory factory = null;
        [SerializeField]
        private int initialPoolSizePerType = 10;
        [SerializeField]
        private List<BalloonDataSO> balloonsToGenerate = new();


        public readonly Dictionary<string, Stack<Balloon>> Balloons = new();


        public static BalloonPool Instance { get; private set; }


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
                Debug.LogErrorFormat("Failed to instantiate Pool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }
        #endregion


        #region Public Methods
        public bool TryReturn<T>(T obj) where T : class
        {
            if (obj is Balloon balloon)
            {
                return TryReturn(balloon);
            }

            Debug.LogErrorFormat("Invalid object type ({0}) returned to Pool: {1} | ID: {2}",
                obj.GetType().Name,
                this.gameObject.name,
                this.gameObject.GetEntityId());

            return false;
        }

        public bool TryReturn(Balloon balloon)
        {
            string balloonType = balloon.Data.name;

            // Ensure the balloon type exists in the pool.
            if (!Balloons.ContainsKey(balloonType))
            {
                Debug.LogErrorFormat("Balloon type ({0}) not found in Pool: {1} | ID: {2}",
                    balloonType,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Deactivate and push the balloon back into the pool.
            if (balloon.gameObject.activeInHierarchy)
                balloon.gameObject.SetActive(false);

            Balloons[balloonType].Push(balloon);

            // Reparent the balloon to the pool and reset its position for organization.
            if (balloon.transform.parent != this.transform)
                balloon.transform.SetParent(this.transform);

            balloon.transform.localPosition = Vector3.zero;

            return true;
        }

        public bool TryGet(BalloonDataSO balloonData, out Balloon balloon)
        {
            balloon = null;
            string balloonType = balloonData.name;

            // Check if the balloon type exists in the pool.
            if (!Balloons.ContainsKey(balloonType))
            {
                Debug.LogErrorFormat("Balloon type ({0}) not found in Pool: {1} | ID: {2}",
                    balloonType,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Check if the balloon type has available balloons.
            if (Balloons[balloonType].Count == 0)
            {
                Debug.LogWarningFormat("No available balloons of type ({0}) in Pool: {1} | ID: {2}",
                    balloonType,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            // Get a balloon from the pool and activate it.
            balloon = Balloons[balloonType].Pop();
            balloon.gameObject.SetActive(true);
            return true;
        }
        #endregion


        #region Private Methods
        private bool TryInstantiatePool()
        {
            foreach (var balloonData in balloonsToGenerate)
            {
                //! Balloon type is defined by the name of the BalloonData scriptable object.
                string balloonType = balloonData.name;

                Balloons[balloonType] = new();

                for (int i = 0; i < initialPoolSizePerType; i++)
                {
                    // Create a new balloon using the factory.
                    if (!factory.TryCreate(balloonData, out Balloon newBalloon))
                    {
                        Debug.LogErrorFormat("Failed to create Spawnable: {0} | ID: {1}\nfor Pool: {2} | ID: {3}",
                            balloonData.name,
                            balloonData.GetEntityId(),
                            this.gameObject.name,
                            this.gameObject.GetEntityId());
                        break;
                    }

                    if (!newBalloon.TryAssignPool(this))
                    {
                        Debug.LogErrorFormat("Failed to assign Pool to Spawnable (): {0} | ID: {1}",
                            newBalloon.gameObject.name,
                            newBalloon.gameObject.GetEntityId());
                    }

                    // Make sure the balloon is inactive when added to the pool.
                    newBalloon.gameObject.SetActive(false);

                    // Name and parent the balloon for organization.
                    newBalloon.name = $"{balloonType}_{this.gameObject.name}_{i}";
                    newBalloon.transform.SetParent(this.transform);
                    newBalloon.transform.localPosition = Vector3.zero;

                    // Enqueue the new balloon into the pool.
                    Balloons[balloonType].Push(newBalloon);
                }
            }

            // Return true if all balloon types were instantiated.
            return Balloons.Count == balloonsToGenerate.Count;
        }
        #endregion
    }
}