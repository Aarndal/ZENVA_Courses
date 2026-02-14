using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SpawnSystem
{
    /// <summary>
    /// Component responsible for spawning entities based on a set of instructions.
    /// Retrieves the entities from a specified spawn pool and spawns them in accordance with the given instructions.
    /// It also implements IDisposable to ensure proper cleanup of resources when the spawner is destroyed or disabled.
    /// </summary>
    public class Spawner : MonoBehaviour, ISpawner, IDisposable
    {
        private bool _disposed = false;
        private bool _isConfigured = false;
        private bool _isSpawnInProcess = false;
        private int _spawnedCounter = 0;

        private CancellationTokenSource _cts = new();
        private Queue<ISpawnerInstruction> _instructions = new();
        private SpawnablePool _localPool = null;

        [SerializeField,
            Tooltip("List of spawn instructions to be executed by the spawner.")]
        private List<SpawnInstructionSO> givenInstructions = default;

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


        #region Unity Lifecycle Methods
        private void Awake()
        {
            _isConfigured = false;
            _localPool = new(this);

            if (!TrySetInstructions(givenInstructions.Cast<ISpawnerInstruction>()))
            {
                this.gameObject.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            StartSpawning();
        }

        private async void Update()
        {
            await ExecuteNextSpawn();
        }

        private void OnDisable()
        {
            StopSpawning();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        #endregion


        #region Private Methods
        private async UniTask ExecuteNextSpawn()
        {
            if (!_isConfigured)
                return;

            if (_isSpawnInProcess)
                return;

            await SpawnWithDelay();

            if (!TryAdvanceToNextInstruction())
                return;

            if (_instructions.Count == 0)
                this.gameObject.SetActive(false);
        }

        private async UniTask SpawnWithDelay()
        {
            _isSpawnInProcess = true;

            try
            {
                await UniTask.Delay(
                    delayTimeSpan: TimeSpan.FromSeconds(_instructions.Peek().SpawnSequence.GetNextInterval()),
                    ignoreTimeScale: false,
                    cancellationToken: _cts.Token);

                if (_disposed || _cts.Token.IsCancellationRequested)
                {
                    return;
                }

                await Spawn(_instructions.Peek());
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled, just exit gracefully
            }
            finally
            {
                _isSpawnInProcess = false;
            }
        }

        private async UniTask Spawn(ISpawnerInstruction instruction)
        {
            if (_disposed || _cts.Token.IsCancellationRequested)
            {
                return;
            }

            if (!_localPool.TryGet(instruction.SpawnableTypeToSpawn, out var spawnable))
            {
                if (instruction.SpawnableTypeToSpawn.GlobalPool == null ||
                    !instruction.SpawnableTypeToSpawn.GlobalPool.
                    TryGetOrCreate(instruction.SpawnableTypeToSpawn, out spawnable))
                {
                    return;
                }
            }

            spawnable.Spawn(this.transform.position, instruction.SpawnContext);
            _spawnedCounter++;

            await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
        }

        private bool TryAdvanceToNextInstruction()
        {
            if (_disposed || _cts.Token.IsCancellationRequested)
            {
                return false;
            }

            if (_spawnedCounter >= _instructions.Peek().AmountToSpawn)
            {
                _spawnedCounter = 0;
                _instructions.Dequeue();
                return true;
            }

            return false;
        }
        #endregion


        #region Public Methods
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            // Clean up managed resources
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _instructions?.Clear();
            _instructions = null;

            // Clear events to prevent memory leaks
            _spawningStarted = null;
            _spawningStopped = null;
        }

        public bool TrySetInstructions(IEnumerable<ISpawnerInstruction> instructions)
        {
            // Validation if the spawner is already configured
            if (_isConfigured)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat(
                    "Failed to set instructions for spawner because it is already configured: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
                return false;
            }

            _instructions.Clear();

            // Null reference validation
            if (instructions == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat(
                    "Failed to set instructions for spawner because of null reference: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
                return false;
            }

            var instructionList = instructions.ToList();

            int numberOfAllElements = instructionList.Count;

            // Empty collection validation
            if (numberOfAllElements == 0)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat(
                    "Failed to set instructions for spawner because of empty instructions list: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
                return false;
            }

            int numberOfNullElements = instructionList.RemoveAll(instruction => instruction == null);

            // All elements are null validation
            if (numberOfNullElements == numberOfAllElements)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat(
                    "Failed to set instructions for spawner because all instructions were null: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
                return false;

            }

            // Some elements are null validation
            if (numberOfNullElements > 0)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat(
                    "Some instructions were null and have been ignored for spawner: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
            }

            _instructions = new Queue<ISpawnerInstruction>(instructionList);
            return _isConfigured = true;
        }

        public void StartSpawning()
        {
            _cts = new();
            _spawnedCounter = 0;
            _spawningStarted?.Invoke();
        }

        public void StopSpawning()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _spawnedCounter = 0;
            _isSpawnInProcess = false;

            _spawningStopped?.Invoke();
        }
        #endregion
    }
}