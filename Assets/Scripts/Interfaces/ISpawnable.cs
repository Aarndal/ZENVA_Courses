using UnityEngine;

public interface ISpawnable
{
    string Name { get; }
    IObjectPool Pool { get; }

    void Spawn(Vector3 spawnPosition);
    void Despawn();

    bool TryAssignPool(IObjectPool pool);
}