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

        private void Update()
        {
            if (balloonData == null)
                return;

            if (BalloonPool.Instance.Balloons[balloonData.name].Count == 0)
                return;

            StartCoroutine(DelaySpawn());

            SpawnBalloon();
        }

        private void SpawnBalloon()
        {
            if (BalloonPool.Instance.TryRetrieveBalloon(balloonData, out GameObject balloon))
            {
                balloon.transform.position = this.transform.position;
            }
        }
        private IEnumerator DelaySpawn()
        {
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}