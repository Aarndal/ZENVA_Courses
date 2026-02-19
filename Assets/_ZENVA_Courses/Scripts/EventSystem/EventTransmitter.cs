using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

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
    public class EventChannelFactory : IFactory<IEventChannel<IEventArgs>, NoData>
    {
        public bool TryCreate(NoData _, out IEventChannel<IEventArgs> channel)
        {
            channel = new EventChannel<IEventArgs>();
            return channel != null;
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
    /// ISubscribers are IEventParticipants that can subscribe their event handlers to IEventChannels.
    /// They can also request IEventChannel references through the EventTransmitter.
    /// The EventTransmitter will provide a new IEventChannel if the requested IEventChannel doesn't exist.
    /// </summary>
    public interface ISubscriber : IEventParticipant
    {
        HashSet<IEventChannel> SubscribedChannels { get; }
    }

    /// <summary>
    /// IPublishers are IEventParticipants that can publish their EventArgs in IEventChannels, which will raise the corresponding events and trigger the subscribed event handlers.
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
    /// ISubscribers can subscribe their event handlers to an IEventChannel, which will be triggered when an event is raised in that channel.
    /// </summary>
    public interface IEventChannel : IDisposable
    {
        event Action<IEventChannel> DisposalRequested;
        int SubscriberCount { get; }
    }

    public interface IEventChannel<TEventArgs> : IEventChannel
        where TEventArgs : IEventArgs
    {
        /// <summary>
        /// Tries to publish the provided EventArgs in this typed IEventChannel.
        /// When published, the corresponding event is raised and the subscribed event handlers are triggered.
        /// </summary>
        /// <param name="args">The event arguments to publish.</param>
        /// <param name="publisher">The publisher of the event.</param>
        /// <returns>true if the event was successfully published; otherwise, false.</returns>
        bool TryPublish(
            TEventArgs args,
            IPublisher publisher = null);

        bool TrySubscribe(
            ISubscriber subscriber,
            Action<TEventArgs> handler,
            Func<TEventArgs, bool> filter = null);

        bool TryUnsubscribe(
            ISubscriber subscriber,
            Action<TEventArgs> handler);
    }

    public class EventChannel<TEventArgs> : IEventChannel<TEventArgs>
        where TEventArgs : IEventArgs
    {
        private readonly HashSet<(ISubscriber Subscriber, Action<TEventArgs> Handler, Func<TEventArgs, bool> Filter)> _subscribedHandlers = new();

        public int SubscriberCount => _subscribedHandlers.Count;

        public event Action<IEventChannel> DisposalRequested;

        public void Dispose()
        {
            _subscribedHandlers?.Clear();
        }
        public bool TryPublish(TEventArgs args, IPublisher publisher = null)
        {
            if (SubscriberCount == 0)
                return false;
            foreach (var subscriber in _subscribedHandlers.Select(sh => sh.Subscriber))
            {
                // Here you would typically invoke the subscriber's event handler with the provided args.
                // This is a placeholder for demonstration purposes.
                Console.WriteLine($"Event published to subscriber {subscriber.ID} with args: {args}");
            }
            return true;
        }
        public bool TrySubscribe(ISubscriber subscriber, Action<TEventArgs> handler, Func<TEventArgs, bool> filter = null)
        {
            if (subscriber == null || handler == null)
                return false;

            if (!_subscribedHandlers.Add((subscriber, handler, filter)))
                return false;

            // Here you would typically store the handler and filter for later invocation when an event is published.
            // This is a placeholder for demonstration purposes.
            Console.WriteLine($"Subscriber {subscriber.ID} subscribed with handler and filter.");
            return true;
        }
        public bool TryUnsubscribe(ISubscriber subscriber, Action<TEventArgs> handler)
        {
            if (subscriber == null || handler == null)
                return false;
            var handlerToRemove = _subscribedHandlers.FirstOrDefault(sh => sh.Subscriber.ID == subscriber.ID && sh.Handler == handler);

            if (handlerToRemove.Equals(default))
                return false;

            _subscribedHandlers.Remove(handlerToRemove);
            Console.WriteLine($"Subscriber {subscriber.ID} unsubscribed from handler.");

            if(_subscribedHandlers.Count == 0)
            {
                DisposalRequested?.Invoke(this);
            }

            return true;
        }
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