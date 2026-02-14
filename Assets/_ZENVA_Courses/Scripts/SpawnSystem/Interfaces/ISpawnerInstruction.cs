namespace SpawnSystem
{
    public interface ISpawnerInstruction
    {
        int AmountToSpawn { get; }
        ISpawnableData SpawnableTypeToSpawn { get; }
        ISpawnContext SpawnContext { get; }
        IIntervalSequence SpawnSequence { get; }
    }
}