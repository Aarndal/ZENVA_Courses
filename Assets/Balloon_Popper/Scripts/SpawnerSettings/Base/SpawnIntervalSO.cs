using UnityEngine;

public abstract class SpawnIntervalSO : ScriptableObject, ISpawnInterval
{
    public abstract float GetNextInterval();
}
