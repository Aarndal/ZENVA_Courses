public interface IObjectPool
{
    bool TryReturn<T>(T obj);
}

public interface IGenericObjectPool : IObjectPool
{
    bool TryGetOrCreate<TObj>(out TObj obj);
    bool TryGetOrCreateByData<TData, TObj>(TData data, out TObj obj);
}

public interface IObjectPool<TObj, TData> : IObjectPool
{
    bool TryGetOrCreate(TData data, out TObj obj);
    bool TryReturn(TObj obj);
}

public interface IObjectPool<T> : IObjectPool
{
    bool TryGetOrCreate(out T obj);
    bool TryReturn(T obj);
}
