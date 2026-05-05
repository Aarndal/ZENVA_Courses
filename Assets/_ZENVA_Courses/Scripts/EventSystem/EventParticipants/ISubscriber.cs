using System;

namespace EventSystem
{
    /// <summary>
    /// ISubscribers are IEventParticipants that can subscribe their event handlers to IEventChannels.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// The EventTransmitter will provide a new IEventChannel if the requested IEventChannel doesn't exist.
    /// </summary>
    public interface ISubscriber : IEventParticipant
    {
        /// <summary>
        /// Raised when the subscriber wants to unsubscribe from all channels.
        /// IEventChannels subscribe to this event and handle their own cleanup via TryUnsubscribe.
        /// </summary>
        event Action<ISubscriber> UnsubscribeRequested;
    }
}
