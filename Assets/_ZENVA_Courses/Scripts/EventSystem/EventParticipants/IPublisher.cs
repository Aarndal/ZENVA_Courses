using System;

namespace EventSystem
{
    /// <summary>
    /// IPublishers are IEventParticipants that can publish their EventArgs in IEventChannels, which will raise the corresponding events and trigger the subscribed event handlers.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// </summary>
    public interface IPublisher : IEventParticipant
    {
        bool IsAnonymous => string.IsNullOrEmpty(Name) && ID.Equals(Guid.Empty);
        string Name { get; }
    }
}