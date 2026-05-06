namespace EventSystem
{
    /// <summary>
    /// IEventParticipants are the base type for both ISubscribers and IPublishers.
    /// They can request IEventChannel references through the EventTransmitter.
    /// </summary>
    public interface IEventParticipant : System.IEquatable<IEventParticipant>
    {
        uint EventID { get; }

        /// <summary>
        /// Return a unique and stable key for this participant (e.g. "Player1", "Enemy/42", path, GUID, etc).
        /// </summary>
        string UniqueKey { get; }
    }
}
