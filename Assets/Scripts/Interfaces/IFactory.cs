public interface IFactory<T1, T2>
{
    bool TryCreate(T2 data, out T1 obj);
}
