using ObjectPools;

/// <summary>
/// Base interface for factories.
/// </summary>
public interface IFactory { }

/// <summary>
/// Factory that creates objects using provided data.
/// </summary>
public interface IFactory<TObj, TData> : IFactory 
    where TData : IDataProvider
{
    bool TryCreate(TData data, out TObj obj);
}

/// <summary>
/// Factory that creates objects without requiring data.
/// </summary>
public interface IFactory<T> : IFactory<T, NoData>
{
    bool TryCreate(out T obj) => TryCreate(default, out obj);
}
