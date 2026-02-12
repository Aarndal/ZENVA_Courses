using System;

namespace ObjectPools
{
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
        bool IsInitialized { get; }
        PoolScope Scope { get; }

        bool TryReturnAll();
    }

    /// <summary>
    /// Object pool with data requirement.
    /// Use NoData as TData for simple pools.
    /// </summary>
    public interface IObjectPool<TObj, TData> : IObjectPool
        where TData : IDataProvider
    {
        const int DefaultInitialCapacity = 10;

        TObj GetOrCreate(TData data);
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
        T GetOrCreate() => GetOrCreate(default);
    }
}