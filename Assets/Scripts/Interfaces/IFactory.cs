public interface IFactory<T1, T2> where T1 : class , new()
{
    bool TryCreate(T2 data, out T1 obj);
}
