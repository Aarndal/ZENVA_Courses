using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnSystem
{
    public class Spawner : MonoBehaviour, ISpawner
    {
        private bool _isConfigured = false;
        private bool _isSpawnInProcess = false;
        private int _spawnedCounter = 0;
        private Queue<ISpawnerInstruction> _instructions = new();

        [SerializeField]
        private List<SpawnInstructionSO> instructionList = default;

        public Queue<ISpawnerInstruction> Instructions => _instructions;


        private Action _spawningStarted;
        private Action _spawningStopped;


        public event Action SpawningStarted
        {
            add
            {
                _spawningStarted -= value;
                _spawningStarted += value;
            }
            remove
            {
                _spawningStarted -= value;
            }
        }
        public event Action SpawningStopped
        {
            add
            {
                _spawningStopped -= value;
                _spawningStopped += value;
            }
            remove
            {
                _spawningStopped -= value;
            }
        }


        private void Awake()
        {
            if (instructionList == null || instructionList.Count == 0)
            {
                return;
            }

            if(_isConfigured)
            {
                return;
            }

            if (!TrySetInstructions(new Queue<ISpawnerInstruction>(instructionList.Cast<ISpawnerInstruction>())))
            {
                return;
            }
        }

        private void Start()
        {
            _isSpawnInProcess = false;
        }

        private async void Update()
        {
            if (!_isConfigured)
                return;

            if (_isSpawnInProcess)
                return;

            await DelayedSpawn();

            if (!TryAdvanceToNextInstruction())
                return;

            if (HasCompletedAllInstructions())
            {
                StopSpawning();
            }
        }

        private bool HasCompletedAllInstructions()
        {
            return _instructions.Count == 0;
        }

        private async UniTask DelayedSpawn()
        {
            _isSpawnInProcess = true;

            await UniTask.Delay(TimeSpan.FromSeconds(_instructions.Peek().SpawnSequence.GetNextInterval()), ignoreTimeScale: false);

            var data = _instructions.Peek().SpawnableTypeToSpawn as SpawnableDataSO;

            if (data.SpawnPool.TryGet(_instructions.Peek().SpawnableTypeToSpawn, out ISpawnable spawnable))
            {
                await Spawn(spawnable);
            }

            _isSpawnInProcess = false;

            await UniTask.CompletedTask;
        }

        private async UniTask Spawn(ISpawnable spawnable)
        {
            spawnable.Spawn(this.transform.position);
            _spawnedCounter++;

            await UniTask.CompletedTask;
        }

        private bool TryAdvanceToNextInstruction()
        {
            if (_spawnedCounter >= _instructions.Peek().AmountToSpawn)
            {
                _spawnedCounter = 0;
                _instructions.Dequeue();
                return true;
            }

            return false;
        }

        public bool TrySetInstructions(Queue<ISpawnerInstruction> instructions)
        {
            if (_isConfigured)
            {
                return false;
            }

            if (instructions == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Failed to set instructions for spawner because of null reference: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
                return false;
            }

            if (instructions.Any(instruction => instruction == null))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Failed to set instructions for spawner because of null reference in instructions: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
                return false;
            }

            _instructions = instructions;
            return _isConfigured = true;
        }

        public void StartSpawning()
        {
            this.gameObject.SetActive(true);
            _spawningStarted?.Invoke();
        }

        public void StopSpawning()
        {
            _spawnedCounter = 0;
            _isSpawnInProcess = false;
            this.gameObject.SetActive(false);
            _spawningStopped?.Invoke();
        }
    }
}