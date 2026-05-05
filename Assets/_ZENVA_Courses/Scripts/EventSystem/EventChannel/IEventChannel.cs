using System;

namespace EventSystem
{
    public interface IEventChannel : IDisposable
    {
        event Action<IEventChannel> DisposalRequested;
        int SubscriberCount { get; }
    }

    /// <summary>
    /// IEventChannels are the channels through which events are transmitted from <see cref="IPublisher"/> to <see cref="ISubscriber"/>.
    /// They are created and managed by the <see cref="EventTransmitter"/>.
    /// An IEventChannel can request the <see cref="EventTransmitter"/> to execute its destruction, freeing up resources.
    /// </summary>
    public interface IEventChannel<TEventArgs> : IEventChannel
        where TEventArgs : IEventArgs
    {
        /// <summary>
        /// Tries to publish the provided <see cref="IEventArgs"/> in this typed IEventChannel.
        /// When published, the corresponding event is raised and the subscribed event handlers are invoked.
        /// </summary>
        /// <param name="args">The event arguments to publish.</param>
        /// <returns>true if the event was successfully published; otherwise, false.</returns>
        bool TryPublish(TEventArgs args);

        /// <summary>
        /// Tries to subscribe the provided <see cref="ISubscriber"/> with its event handler to this typed IEventChannel.
        /// When subscribed, the event handler can be invoked when an event is raised in this channel.
        /// </summary>
        /// <param name="subscriber">The subscriber requesting to subscribe to this channel.</param>
        /// <param name="handler">The event handler to be invoked when an event is raised.</param>
        /// <param name="predicate">An optional predicate to evaluate if the handler should be invoked.</param>
        /// <returns>true if the subscription was successful; otherwise, false.</returns>
        bool TrySubscribe(
            ISubscriber subscriber,
            Action<TEventArgs> handler,
            Predicate<TEventArgs> predicate = null);

        /// <summary>
        /// Tries to unsubscribe the provided <see cref="ISubscriber"/> from this typed IEventChannel.
        /// </summary>
        /// <param name="subscriber">The subscriber requesting to unsubscribe from this channel.</param>
        /// <returns>true if the unsubscription was successful; otherwise, false.</returns>
        bool TryUnsubscribe(
            ISubscriber subscriber);

        bool TryUnsubscribe(
            ISubscriber subscriber,
            Action<TEventArgs> handler);
    }
}
