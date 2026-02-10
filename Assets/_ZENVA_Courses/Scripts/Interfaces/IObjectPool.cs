public interface IObjectPool
{
    bool TryReturn<T>(T obj) where T : class;
}

public interface IGenericObjectPool : IObjectPool
{
    bool TryGet<T>(out T obj) where T : class;
    bool TryGetByData<TData, TObj>(TData data, out TObj obj) where TObj : class;
}

public interface IObjectPool<T1, T2> : IObjectPool where T1 : class
{
    bool TryGet(T2 key, out T1 obj);
    bool TryReturn(T1 obj);
}

public interface IObjectPool<T> : IObjectPool where T : class
{
    bool TryGet(out T obj);
    bool TryReturn(T obj);
}
