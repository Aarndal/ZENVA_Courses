namespace InteractableSystem
{
    /// <summary>
    /// An object an entity (player, NPC, dynamic object) interacts with actively and deliberately
    /// (flip a light switch, pick up an item, open a door via a lever, sit on a chair).
    /// The defining criterion is intent: the interactor consciously chooses to interact.
    /// Merely colliding with an object or entering a trigger volume is NOT an interaction —
    /// such passive world events are handled elsewhere (e.g. via the EventSystem).
    /// 
    /// Call direction: a driver component on the interactor's side (mouse raycast,
    /// interact key while in range, NPC AI decision, ...) detects the intent and calls
    /// <see cref="TryInteract"/> on this interactable, passing its <see cref="IInteractor"/>.
    /// The interactable itself stays passive: it never polls for or searches interactors,
    /// it only reacts to incoming calls.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Whether the object can currently be interacted with.</summary>
        bool CanBeInteractedWith { get; }

        /// <summary>
        /// Attempts to perform the interaction initiated by the given interactor.
        /// </summary>
        /// <param name="interactor">The entity deliberately initiating the interaction.</param>
        /// <returns>true if the interaction was performed; otherwise, false.</returns>
        bool TryInteract(IInteractor interactor);
    }
}