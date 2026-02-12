using UnityEngine;

namespace SpawnSystem
{
    public interface ISpawnContext : IDataProvider
    {
        /// <summary>
        /// The time in seconds after which a spawned object should be despawned automatically.
        /// </summary>
        float DespawnTimeOut { get; }

        /// <summary>
        /// The delay in seconds before the object is spawned.
        /// </summary>
        float SpawnDelay { get; }

        /// <summary>
        /// The offset to apply in relation to the spawn position.
        /// </summary>
        Vector3 TargetOffset { get; }
    }
}