using UnityEngine;

public interface ISpawnable
{
    IDataProvider<ISpawnable> Data { get; }
    GameObject GameObject { get; }
    IObjectPool Pool { get; }
    string SpawnableType { get; }

    void Despawn();
    void Spawn(Vector3 spawnPosition);

    bool TryAssignPool(IObjectPool pool);
    bool TryInitialize(IDataProvider<ISpawnable> data);
}