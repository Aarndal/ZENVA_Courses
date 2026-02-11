public interface IChecker
{
}

public interface IChecker<T> : IChecker
{
    bool Check(T parameter);
}