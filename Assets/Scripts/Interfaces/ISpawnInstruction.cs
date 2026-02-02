public interface ISpawnInstruction
{
    int AmountToSpawn { get; }
    ISpawnInterval SpawnInterval { get; }
}

public interface ISpawnInstruction<T> : ISpawnInstruction where T : IDataProvider
{
    T Data { get; }
}
