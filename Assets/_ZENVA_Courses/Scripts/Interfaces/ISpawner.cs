using System.Collections.Generic;

public interface ISpawner<T> : ISpawner where T : IDataProvider
{
    bool TrySetInstructions(List<ISpawnInstruction<T>> instructions);
}

public interface ISpawner
{
    void StartSpawning();
    void StopSpawning();
}
