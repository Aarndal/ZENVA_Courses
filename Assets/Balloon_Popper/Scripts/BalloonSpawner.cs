using Cysharp.Threading.Tasks;
using System;
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

        private async void Update()
        {
            if (balloonData == null)
                return;

            if (BalloonPool.Instance.Balloons[balloonData.name].Count == 0)
                return;

            if (_isSpawning)
                return;

            await DelayedSpawn();
        }

        private UniTask SpawnBalloon()
        {
            if (BalloonPool.Instance.TryRetrieveBalloon(balloonData, out GameObject balloon))
            {
                balloon.transform.position = this.transform.position;
            }
            return UniTask.CompletedTask;
        }
        private async UniTask DelayedSpawn()
        {
            _isSpawning = true;

            await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval), ignoreTimeScale: false);

            await SpawnBalloon();

            _isSpawning = false;
        }
    }
}