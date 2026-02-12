using UnityEngine;

public abstract class SpawnIntervalSO : ScriptableObject, IIntervalSequence
{
    public abstract float GetNextInterval();
}
