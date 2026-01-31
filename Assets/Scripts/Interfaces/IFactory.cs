using UnityEngine;

public interface IFactory<T1, T2>
{
    bool TryCreate(T2 data, out T1 obj);
}

public interface IGameObjectFactory<T> : IFactory<GameObject, T>
{
}
