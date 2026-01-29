using System;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    // Manages a pool of balloon objects for efficient reuse.
    public class BalloonPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject balloonPrefab = null;
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

            if (!InstanciatePool())
            {
                Debug.LogErrorFormat("Failed to instantiate BalloonPool: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }

        // Returns a balloon to the pool for reuse.
        public void ReturnBalloon(Balloon balloon)
        {
            string balloonType = balloon.BalloonData.name;

            if (!Balloons.ContainsKey(balloonType))
            {
                Balloons[balloonType] = new Queue<GameObject>();
            }

            if (balloon.gameObject.activeInHierarchy)
                balloon.gameObject.SetActive(false);

            Balloons[balloonType].Enqueue(balloon.gameObject);

            if (balloon.transform.parent != this.transform)
                balloon.transform.SetParent(this.transform);

            balloon.transform.position = this.transform.position;
        }

        public bool TryRetrieveBalloon(SOBalloonData balloonData, out GameObject balloon)
        {
            balloon = null;
            string balloonType = balloonData.name;

            if (!Balloons.ContainsKey(balloonType))
            {
                Debug.LogErrorFormat("Balloon type not found in Balloon Pool: {0} | ID: {1}",
                    balloonType,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (Balloons[balloonType].Count == 0)
            {
                Debug.LogWarningFormat("No available balloons of type: {0} | ID: {1}",
                    balloonType,
                    this.gameObject.GetEntityId());
                return false;
            }

            balloon = Balloons[balloonType].Dequeue();
            balloon.SetActive(true);
            return true;
        }


        private bool InstanciatePool()
        {
            Balloon balloonComponent = balloonPrefab.GetComponentInChildren<Balloon>(true);

            if (balloonComponent == null)
            {
                Debug.LogErrorFormat("Balloon prefab missing Balloon component: {0} | ID: {1}",
                    balloonPrefab.name,
                    balloonPrefab.GetEntityId());
                return false;
            }

            foreach (var balloonData in balloonsToGenerate)
            {
                //! Balloon type is defined by the name of the BalloonData scriptable object.
                string balloonType = balloonData.name;

                Balloons[balloonType] = new Queue<GameObject>();

                for (int i = 0; i < initialPoolSizePerType; i++)
                {
                    GameObject newBalloon = Instantiate(balloonPrefab, this.transform.position, Quaternion.identity, this.transform);

                    if (newBalloon == null)
                    {
                        Debug.LogErrorFormat("Failed to instantiate Balloon from prefab: {0} | ID: {1}",
                            balloonData.name,
                            balloonData.GetEntityId());
                        return false;
                    }

                    newBalloon.name = $"{balloonType}_Pooled_{i}";
                    newBalloon.SetActive(false);
                    Balloons[balloonType].Enqueue(newBalloon);
                }

            }

            if (Balloons.Count != balloonsToGenerate.Count)
                return false;

            return true;
        }
    }
}