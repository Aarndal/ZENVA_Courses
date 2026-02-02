using System.Collections.Generic;
using UnityEngine;

public interface ISpawner<T> : ISpawner where T : IDataProvider
{
    bool TrySetInstructions(List<ISpawnInstruction<T>> instructions);
}

public interface ISpawner
{
    void StartSpawning();
    void StopSpawning();
}

public abstract class SpawnerComponent : MonoBehaviour, ISpawner
{
    public abstract void StartSpawning();
    public abstract void StopSpawning();
}

public abstract class SpawnerComponent<T> : SpawnerComponent, ISpawner<T> where T : IDataProvider
{
    public abstract bool TrySetInstructions(List<ISpawnInstruction<T>> instructions);
}