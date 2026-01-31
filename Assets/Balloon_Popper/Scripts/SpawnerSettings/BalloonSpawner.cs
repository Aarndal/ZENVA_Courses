using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace BalloonPopper
{     
    // Spawns balloons from the BalloonPool at the spawner's position.
    public class BalloonSpawner : MonoBehaviour
    {
        [SerializeField]
        private BalloonSpawnInstructionSO instructions;
        
        private bool _isSpawning = false;


        private void Start()
        {
            _isSpawning = false;
        }

        private async void Update()
        {
            if (instructions == null)
                return;

            if (BalloonPool.Instance.Balloons[instructions.Data.name].Count == 0)
                return;

            if (_isSpawning)
                return;

            await DelayedSpawn();
        }

        private async UniTask DelayedSpawn()
        {
            _isSpawning = true;

            await UniTask.Delay(TimeSpan.FromSeconds(instructions.SpawnInterval.GetNextInterval()), ignoreTimeScale: false);

            await SpawnBalloon();

            _isSpawning = false;

            await UniTask.CompletedTask;
        }

        private async UniTask SpawnBalloon()
        {
            if (BalloonPool.Instance.TryGet(instructions.Data, out Balloon balloon))
            {
                balloon.Spawn(this.transform.position);
            }
            await UniTask.CompletedTask;
        }
    }
}