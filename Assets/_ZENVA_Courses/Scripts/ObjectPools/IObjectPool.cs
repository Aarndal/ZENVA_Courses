using System;

namespace ObjectPools
{
    /// <summary>
    /// Value indicating the scope of an object pool.
    /// Local pools manage entities for a single object.
    /// Global pools can be accessed by multiple objects.
    /// </summary>
    public enum PoolScope : byte
    {
        Local,
        Global
    }

    /// <summary>
    /// Base interface for object pools.
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        /// Value indicating whether the pool has been initialized and is ready to manage objects.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// The scope of the pool, indicating whether it is a local or global pool.
        /// </summary>
        PoolScope Scope { get; }

        /// <summary>
        /// Event invoked when the pool is requested to return all of its objects.
        /// </summary>
        event Action ReturnAllRequested;
    }

    /// <summary>
    /// Object pool with data requirement.
    /// Use NoData as TData or IObjectPool<T> interface for pools managing simple objects without additional data.
    /// </summary>
    public interface IObjectPool<TObj, TData> : IObjectPool
        where TData : IDataProvider
    {
        const int DefaultInitialCapacity = 10;

        bool TryGetOrCreate(TData data, out TObj obj);
        bool TryGet(Guid id, out TObj obj);
        bool TryGet(TData data, out TObj obj);
        bool TryInitializeTypePool(TData data, int initialCapacity = DefaultInitialCapacity);
        bool TryReturn(TObj obj);
    }

    /// <summary>
    /// Simple object pool without data requirement.
    /// </summary>
    public interface IObjectPool<T> : IObjectPool<T, NoData>
    {
        bool TryGet(out T obj) => TryGet(id: default, out obj);
        bool TryGetOrCreate(out T obj) => TryGetOrCreate(default, out obj);
    }
}