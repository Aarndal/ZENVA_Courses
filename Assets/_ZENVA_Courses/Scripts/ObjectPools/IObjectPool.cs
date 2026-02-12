using System;
using System.Collections.ObjectModel;

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
        int ActiveCount { get; }
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

        ReadOnlyDictionary<Guid, TObj> AvailableObjects { get; }

        bool TryInitialize(int initialCapacity = DefaultInitialCapacity, TData data = default);
        bool TryGet(Guid id, out TObj obj);
        bool TryGet(TData data, out TObj obj);
        TObj GetOrCreate(TData data);
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