using UnityEngine;

public interface ISpawnerInstruction<T> where T : ScriptableObject
{
    T Data { get; }
    int Amount { get; }
    ISpawnInterval SpawnInterval { get; }
}
