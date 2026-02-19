using System;
using System.Collections.Generic;

namespace EventSystem
{
    /// <summary>
    /// The EventTransmitter is responsible to manage IEventChannels.
    /// IEventParticipants can request a reference to an IEventChannel through the EventTransmitter.
    /// If the EventTransmitter doesn't have a reference to the requested IEventChannel, it will create a new IEventChannel through its EventChannelFactory, if the requester is an ISubscriber, otherwise it will return null.
    /// </summary>
    public static class EventTransmitter
    {
    }

    /// <summary>
    /// An EventChannelFactory is responsible to create IEventChannel instances when requested by the EventTransmitter.
    /// </summary>
    public class EventChannelFactory : IFactory<IEventChannel, IEventArgs>
    {
        public bool TryCreate(IEventArgs args, out IEventChannel channel)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// IEventArgs are used to create custom event arguments that can be passed through the corresponding IEventChannel when an event is triggered.
    /// They are published by IPublishers and received by ISubscribers.
    /// </summary>
    public interface IEventArgs : IDataProvider<IEventArgs>
    {
        EventFlag Flag { get; }
    }

    public struct DefaultEventArgs : IEventArgs
    {
        public readonly EventFlag Flag => EventFlag.None;

        public readonly string InstanceName => "Anonymous Publisher";

        public readonly Guid ID => Guid.Empty;

        public bool Equals(IDataProvider other)
        {
            throw new NotImplementedException();
        }
    }

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

    /// <summary>
    /// ISubscribers are IEventParticipants that can subscribe their EventHandlers to IEventChannels.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// The EventTransmitter will provide a new IEventChannel if the requested IEventChannel doesn't exist.
    /// </summary>
    public interface ISubscriber : IEventParticipant
    {
        HashSet<IEventChannel> SubscribedChannels { get; }
    }

    /// <summary>
    /// IPublishers are IEventParticipants that can publish their EventArgs in IEventChannels, which will raise the corresponding events and trigger the subscribed EventHandlers.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// If the requested IEventChannel doesn't exist, the EventTransmitter will return null.
    /// </summary>
    public interface IPublisher : IEventParticipant
    {
    }

    public struct DefaultPublisher : IPublisher
    {
        public readonly Guid ID => Guid.Empty;
    }

    /// <summary>
    /// IEventChannels are the channels through which events are transmitted from IPublishers to ISubscribers.
    /// They are created and managed by the EventTransmitter.
    /// If an IEventChannel has no ISubscribers left, it will request the EventTransmitter to execute its destruction, freeing up resources.
    /// IPublishers can publish their EventArgs, a reference to themselves, and an EventFlag value to an IEventChannel, raising the corresponding event.
    /// ISubscribers can subscribe their EventHandlers to an IEventChannel, which will be triggered when an event is raised in that channel.
    /// </summary>
    public interface IEventChannel : IDisposable
    {
        HashSet<ISubscriber> Subscribers { get; }
    }

    public interface IEventChannel<TPublisher, TEventArgs> : IEventChannel 
        where TPublisher: IPublisher 
        where TEventArgs : IEventArgs
    {
    }

    public interface IEventChannel<TEventArgs> : IEventChannel<DefaultPublisher, TEventArgs> 
        where TEventArgs : IEventArgs
    {
    }

    /// <summary>
    /// EventFlags are used to provide additional information about the event being raised in an IEventChannel.
    /// They can be combined using bitwise operations to represent multiple states or conditions.
    /// </summary>
    [Flags]
    public enum EventFlag : byte
    {
        None = 0,
    }
}