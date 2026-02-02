using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BalloonPopper
{     
    public class Spawner : SpawnerComponent<IDataProvider<ISpawnable>>
    {
        private bool _isConfigured = false;
        private bool _isSpawnInProcess = false;
        private int _currentInstructionIndex = 0;
        private int _spawnedCount = 0;
        

        public List<ISpawnInstruction<IDataProvider<ISpawnable>>> Instructions { get; private set; }


        private void Start()
        {
            _isSpawnInProcess = false;
        }

        private async void Update()
        {
            if(!_isConfigured)
                return;

            if (_isSpawnInProcess)
                return;

            await DelayedSpawn();

            if(!TryAdvanceToNextInstruction())
                return;

            if (HasCompletedAllInstructions())
            {
                StopSpawning();
            }
        }

        private bool HasCompletedAllInstructions()
        {
            if (_currentInstructionIndex >= Instructions.Count)
            {
                return true;
            }
            return false;
        }

        private async UniTask DelayedSpawn()
        {
            _isSpawnInProcess = true;

            await UniTask.Delay(TimeSpan.FromSeconds(Instructions[_currentInstructionIndex].SpawnInterval.GetNextInterval()), ignoreTimeScale: false);

            await SpawnBalloon();

            _isSpawnInProcess = false;

            await UniTask.CompletedTask;
        }

        private async UniTask SpawnBalloon()
        {
            if (SpawnablePool.Instance.TryGet(Instructions[_currentInstructionIndex].Data, out ISpawnable balloon))
            {
                balloon.Spawn(this.transform.position);
                _spawnedCount++;
            }

            await UniTask.CompletedTask;
        }

        private bool TryAdvanceToNextInstruction()
        {
            if (_spawnedCount >= Instructions[_currentInstructionIndex].AmountToSpawn)
            {
                _spawnedCount = 0;
                _currentInstructionIndex++;

                return true;
            }

            return false;
        }

        public override bool TrySetInstructions(List<ISpawnInstruction<IDataProvider<ISpawnable>>> instructions)
        {
            _isConfigured = false;

            if (instructions == null)
                return false;

            if (instructions.Any(instruction => instruction == null))
                return false;

            Instructions = instructions;
            return _isConfigured = true;
        }

        public override void StartSpawning()
        {
            this.gameObject.SetActive(true);
        }

        public override void StopSpawning()
        {
            _spawnedCount = 0;
            _currentInstructionIndex = 0;
            _isSpawnInProcess = false;
            this.gameObject.SetActive(false);
        }
    }
}