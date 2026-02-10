public interface ICheckable
{
}

public interface ICheckable<T> : ICheckable
{
    bool Check(T parameter);
}