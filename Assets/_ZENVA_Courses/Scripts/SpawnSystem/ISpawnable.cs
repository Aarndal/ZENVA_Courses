using UnityEngine;

public interface ISpawnable: IActivatable
{
    ISpawnableDataProvider Data { get; }
    GameObject GameObject { get; }

    void Despawn();
    void Spawn(Vector3 spawnPosition, ISpawnContext context);

    bool TryInitialize(ISpawnableDataProvider data);
}