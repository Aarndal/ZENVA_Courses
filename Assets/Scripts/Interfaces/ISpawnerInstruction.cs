using UnityEngine;

public interface ISpawnerInstruction<T>
{
    T Data { get; }
    int Amount { get; }
    ISpawnInterval SpawnInterval { get; }
}
