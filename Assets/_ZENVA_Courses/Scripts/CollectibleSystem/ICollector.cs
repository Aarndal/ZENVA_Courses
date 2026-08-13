namespace CollectibleSystem
{
    /// <summary>
    /// An entity capable of collecting <see cref="ICollectible"/> items.
    /// An entity may hold several collectors (inventory, score keeper, key ring, ...),
    /// each accepting only the collectibles it cares about.
    /// Called by the collectible during the collect handshake — never the other way around.
    /// </summary>
    public interface ICollector
    {
        /// <summary>
        /// Attempts to take in the given collectible. The collector decides whether it
        /// accepts it (type, capacity, ...) and what collecting means (add to inventory,
        /// add points to the score, ...). Rejections may be reported via events, e.g. to
        /// notify the UI that the inventory is full.
        /// </summary>
        /// <returns>true if the collectible was actually taken in; otherwise, false.</returns>
        bool TryAdd(ICollectible collectible);
    }
}