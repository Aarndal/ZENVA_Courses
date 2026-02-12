using System;

/// <summary>
/// Base interface for all data providers.
/// </summary>
public interface IDataProvider : IEquatable<IDataProvider>
{
    Guid Id { get; }
}

/// <summary>
/// Typed data provider that provides data for a specific type.
/// </summary>
/// <typeparam name="T">The type this provider supplies data for.</typeparam>
public interface IDataProvider<T> : IDataProvider
{
    string InstanceName { get; }
    Type ProvidedType => typeof(T);
}
