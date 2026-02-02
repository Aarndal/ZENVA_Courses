using System;
using UnityEngine;

public interface IDataProvider
{
    string Name { get; }
}

public interface IDataProvider<T> : IDataProvider where T : class
{
    Type ObjectType => typeof(T);
}
