using System;
using System.Collections.Generic;

namespace EventSystem
{
    /// <summary>
    /// ISubscribers are IEventParticipants that can subscribe their event handlers to IEventChannels.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// The EventTransmitter will provide a new IEventChannel if the requested IEventChannel doesn't exist.
    /// </summary>
    public interface ISubscriber : IEventParticipant
    {
    }
}