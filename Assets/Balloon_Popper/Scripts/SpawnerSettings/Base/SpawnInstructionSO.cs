using UnityEngine;

public abstract class SpawnInstructionSO<T> : ScriptableObject, ISpawnInstruction<T> where T : IDataProvider
{
    public abstract T Data { get; }
    public abstract int Amount { get; }
    public abstract ISpawnInterval SpawnInterval { get; }
}
