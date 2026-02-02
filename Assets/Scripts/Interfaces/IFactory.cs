public interface IFactory
{

}

public interface IGenericFactory : IFactory
{
    bool TryCreate<T>(out T obj) where T : class;
}

public interface IFactory<TObject, TData> : IFactory where TObject : class where TData : IDataProvider
{
    bool TryCreate(TData data, out TObject obj);
}
