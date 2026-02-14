using ObjectPools;
using System;
using UnityEngine;

namespace SpawnSystem
{
    public interface ISpawnableData : IDataProvider<ISpawnable>
    {
        /// <summary>
        /// The prefab used to generate the instance of the spawnable.
        /// </summary>
        GameObject Prefab { get; }
        
        /// <summary>
        /// The global object pool used to manage instances of the spawnable.
        /// </summary>
        IObjectPool<ISpawnable, ISpawnableData> GlobalPool { get; set; }
    }

    public interface ISpawnable : IToggleable
    {
        /// <summary>
        /// Data used to initialize this spawnable.
        /// </summary>
        ISpawnableData Data { get; }
        
        /// <summary>
        /// The game object instance this spawnable represents.
        /// </summary>
        GameObject GameObject { get; }


        /// <summary>
        /// Function invoked when the spawnable is requested to be despawned.
        /// The function should return true if the spawnable can be returned to its pool; otherwise, false.
        /// </summary>
        event Func<ISpawnable, bool> DespawnRequested;


        /// <summary>
        /// Returns this spawnable to its object pool.
        /// </summary>
        void Despawn();

        /// <summary>
        /// Spawns the object at the specified position with the given context.
        /// </summary>
        void Spawn(Vector3 spawnPosition, ISpawnContext context = default);

        /// <summary>
        /// Initializes the spawnable with the provided data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if the initialization was successful; otherwise, false.</returns>
        bool TryInitialize(ISpawnableData data);
    }
}