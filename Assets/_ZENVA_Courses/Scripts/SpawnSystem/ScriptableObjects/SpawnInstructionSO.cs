using UnityEngine;

namespace SpawnSystem
{
    [CreateAssetMenu(fileName = "newSpawnInstruction", menuName = "Spawn System/Spawn Instruction")]
    public class SpawnInstructionSO : ScriptableObject, ISpawnerInstruction
    {
        [SerializeField]
        private int amountToSpawn = default;
        [SerializeField]
        private SpawnableDataSO spawnableTypeToSpawn = default;
        [SerializeField]
        private SpawnContextSO spawnContext = default;
        [SerializeField]
        private IntervalSequenceSO spawnSequence = default;

        public int AmountToSpawn => amountToSpawn;
        public ISpawnableData SpawnableTypeToSpawn => spawnableTypeToSpawn;
        public ISpawnContext SpawnContext => spawnContext;
        public IIntervalSequence SpawnSequence => spawnSequence;
    }
}