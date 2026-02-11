namespace CollectibleSystem
{
    internal interface ICollectible
    {
        bool TryCollect<T>(ICollector<T> collector);
    }

    internal interface ICollectible<T> : ICollectible where T : IDataProvider
    {
        bool TryCollect(ICollector<T> collector);
    }
}