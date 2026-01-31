using UnityEngine;

public interface ISpawnable
{
    IObjectPool Pool { get; }

    void Spawn(Vector3 spawnPosition);
    void Despawn();

    bool TryAssignPool(IObjectPool pool);
}