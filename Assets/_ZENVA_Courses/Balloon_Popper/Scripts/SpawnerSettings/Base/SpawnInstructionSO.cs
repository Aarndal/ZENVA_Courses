using UnityEngine;

public abstract class SpawnInstructionSO : ScriptableObject, ISpawnInstruction<IDataProvider<ISpawnable>>
{
    public abstract IDataProvider<ISpawnable> Data { get; }
    public abstract int AmountToSpawn { get; }
    public abstract ISpawnInterval SpawnInterval { get; }
}
