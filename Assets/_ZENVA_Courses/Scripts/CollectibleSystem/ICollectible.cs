namespace CollectibleSystem
{
    /// <summary>
    /// Simple collectible interface for any collectible type, allowing for polymorphic collection without type constraints.
    /// </summary>
    public interface ICollectible
    {
        bool TryCollect(ICollector collector);
    }

    /// <summary>
    /// Strongly-typed collectible interface for a specific collectible type, ensuring type safety and reducing the need for casting.
    /// </summary>
    /// <typeparam name="TSelf">The type of collectible this interface represents.</typeparam>
    public interface ICollectible<TSelf> : ICollectible where TSelf : ICollectible<TSelf>
    {
        bool TryCollect(ICollector<TSelf> collector);
    }
}