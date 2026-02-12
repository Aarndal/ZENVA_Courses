using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnerComponent : MonoBehaviour, ISpawner
{
    public abstract void StartSpawning();
    public abstract void StopSpawning();
}

public abstract class SpawnerComponent<T> : SpawnerComponent, ISpawner<T> where T : IDataProvider<ISpawnable>
{
    public abstract bool TrySetInstructions(List<ISpawnerInstruction<T>> instructions);
}