using UnityEngine;

public interface ISpawnable
{
    void Spawn(Vector3 spawnPosition);
    void Despawn();
}