namespace CollectibleSystem
{
    /// <summary>
    /// An item that can be collected by an <see cref="ICollector"/>.
    /// The collectible is the entry point of the collect handshake: a driver calls
    /// <see cref="TryCollect"/>, which validates its own availability and the collector's
    /// permission, hands the collectible over to the collector, and — only if the collector
    /// accepted — reacts (despawn, VFX, sound).
    ///
    /// ICollectible is agnostic about HOW collection is initiated. Typical paths:
    /// - Deliberate interaction: the collectible also implements IInteractable and calls
    ///   <see cref="TryCollect"/> from TryInteract when the interactor is an ICollector.
    /// - Passive pickup: a driver component on the collectible (e.g. a trigger volume the
    ///   collector walks through) detects the collector and calls <see cref="TryCollect"/> directly.
    /// </summary>
    public interface ICollectible
    {
        /// <summary>
        /// Whether this collectible is currently available to be collected at all
        /// (not already collected, not locked, not still spawning, ...).
        /// Independent of any specific collector.
        /// </summary>
        bool CanBeCollected { get; }

        /// <summary>
        /// Attempts to be collected by the given collector.
        /// Checks <see cref="CanBeCollected"/>, then whether this collector is permitted
        /// to collect it, then offers itself via <c>ICollector.TryAdd</c>.
        /// </summary>
        /// <param name="collector">The collector attempting to collect this item.</param>
        /// <returns>true if the collector accepted it; otherwise, false.</returns>
        bool TryCollect(ICollector collector);
    }
}