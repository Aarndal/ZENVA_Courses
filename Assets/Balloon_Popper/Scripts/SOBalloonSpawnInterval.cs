using UnityEngine;

public abstract class SOBalloonSpawnInterval : ScriptableObject, ISpawnInterval
{
    public abstract float GetNextInterval();
}
