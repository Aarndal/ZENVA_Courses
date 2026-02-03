using System;

public interface IDataProvider 
    //: IEquatable<IDataProvider>
{
    Guid Id { get; }
    string InstanceName { get; }
}

public interface IDataProvider<T> : IDataProvider where T : class
{
    Type ProvidedType => typeof(T);
}

public interface ISpawnableDataProvider: IDataProvider<ISpawnable>
{
    IObjectPool<ISpawnable> Pool { get; }
}
