using UnityEngine;

public interface ISpawnable
{
    string TypeName { get; }
    GameObject GameObject { get; }
    IObjectPool Pool { get; }

    void Spawn(Vector3 spawnPosition);
    void Despawn();

    bool TryAssignPool(IObjectPool pool);

    bool TryInitialize(IDataProvider<ISpawnable> data);
}