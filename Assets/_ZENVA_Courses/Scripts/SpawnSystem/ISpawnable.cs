using UnityEngine;

public interface ISpawnable
{
    IDataProvider<ISpawnable> Data { get; }
    GameObject GameObject { get; }
    string SpawnableType { get; }

    void Despawn();
    void Spawn(Vector3 spawnPosition);

    bool TryInitialize(IDataProvider<ISpawnable> data);
}