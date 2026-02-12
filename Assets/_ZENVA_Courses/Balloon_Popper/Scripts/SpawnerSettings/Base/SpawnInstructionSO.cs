using UnityEngine;

public abstract class SpawnInstructionSO : ScriptableObject, ISpawnerInstruction<IDataProvider<ISpawnable>>
{
    public abstract IDataProvider<ISpawnable> Data { get; }
    public abstract int AmountToSpawn { get; }
    public abstract IIntervalSequence SpawnInterval { get; }
}
