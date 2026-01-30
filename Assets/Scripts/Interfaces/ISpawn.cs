using UnityEngine;

public interface ISpawn
{
    void Spawn(Vector3 spawnPosition);
    void Despawn();
}