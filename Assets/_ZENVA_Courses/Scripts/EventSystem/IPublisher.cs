namespace EventSystem
{
    /// <summary>
    /// IPublishers are IEventParticipants that can publish their EventArgs in IEventChannels, which will raise the corresponding events and trigger the subscribed event handlers.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// If the requested IEventChannel doesn't exist, the EventTransmitter will return null.
    /// </summary>
    public interface IPublisher : IEventParticipant
    {
    }
}