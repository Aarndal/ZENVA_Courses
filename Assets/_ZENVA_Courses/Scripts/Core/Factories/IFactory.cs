namespace Core
{
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

    public interface IGenericFactory<T> : IFactory
    {
        public bool TryCreate<TData>(out T obj)
            where TData : IDataProvider;
    }
}