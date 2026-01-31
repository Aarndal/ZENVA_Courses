using UnityEngine;
public interface IObjectPool<T1, T2>
{
    bool TryGet(T2 key, out T1 obj);
    bool TryReturn(T1 obj);
}

public interface IObjectPool<T>
{
    bool TryGet(out T obj);
    bool TryReturn(T obj);
}

public interface IGameObjectPool<T> : IObjectPool<GameObject, T>
{
}