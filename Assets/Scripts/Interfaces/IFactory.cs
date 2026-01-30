public interface IFactory<T1, T2>
{
    bool TryCreate(T2 data, out T1 @object);
}

//public interface IGenericFactory
//{
//    T1 Create<T1, T2>(T2 data) where T1 : new();
//}
