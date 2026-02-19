using System;

namespace EventSystem
{
    /// <summary>
    /// IEventParticipants are the base type for both ISubscribers and IPublishers.
    /// They can request IEventChannel references through the EventTransmitter.
    /// </summary>
    public interface IEventParticipant
    {
        Guid ID
        {
            get;
        }
    }
}