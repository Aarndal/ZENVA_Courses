namespace CollectibleSystem
{
    /// <summary>
    /// Simple collector interface for any collectible type, allowing for polymorphic collection without type constraints.
    /// Implementations should handle type checking and casting as needed.
    /// </summary>
    public interface ICollector
    {
        bool CanCollect(ICollectible collectible);
        void Collect(ICollectible collectible);
    }

    /// <summary>
    /// Strongly-typed collector interface for a specific collectible type, ensuring type safety and reducing the need for casting.
    /// </summary>
    /// <typeparam name="T">The type of collectible this collector can handle.</typeparam>
    public interface ICollector<in T> : ICollector where T : ICollectible
    {
        bool CanCollect(T collectible);
        void Collect(T collectible);
    }

}