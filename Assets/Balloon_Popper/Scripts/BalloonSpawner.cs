using System.Collections;
using UnityEngine;

namespace BalloonPopper
{     
    // Spawns balloons from the BalloonPool at the spawner's position.
    public class BalloonSpawner : MonoBehaviour
    {
        [SerializeField]
        private SOBalloonData balloonData;
        [SerializeField]
        private float spawnInterval = 2f;
        
        private bool _isSpawning = false;

        private void Start()
        {
            _isSpawning = false;
        }

        private void Update()
        {
            if (balloonData == null)
                return;

            if (BalloonPool.Instance.Balloons[balloonData.name].Count == 0)
                return;

            if (_isSpawning)
                return;

            StartCoroutine(DelayedSpawn());
        }

        private void SpawnBalloon()
        {
            if (BalloonPool.Instance.TryRetrieveBalloon(balloonData, out GameObject balloon))
            {
                balloon.transform.position = this.transform.position;
            }
        }
        private IEnumerator DelayedSpawn()
        {
            _isSpawning = true;

            yield return new WaitForSeconds(spawnInterval);

            SpawnBalloon();

            _isSpawning = false;
        }
    }
}