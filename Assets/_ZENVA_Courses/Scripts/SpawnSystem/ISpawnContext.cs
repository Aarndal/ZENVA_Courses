using UnityEngine;

public interface ISpawnContext
{
    Vector3 TargetOffset { get; }
    float DespawnTimeOut { get; }
}