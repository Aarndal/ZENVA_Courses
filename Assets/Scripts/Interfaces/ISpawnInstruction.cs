using BalloonPopper;

public interface ISpawnInstruction
{
    ISpawnInterval SpawnInterval { get; }
}

public interface ISpawnInstruction<T> : ISpawnInstruction where T : IDataProvider
{
    T Data { get; }
    int Amount { get; }
}
