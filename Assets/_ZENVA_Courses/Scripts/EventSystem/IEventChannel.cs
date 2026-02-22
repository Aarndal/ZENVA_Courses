using System;

namespace EventSystem
{
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

    public partial interface IEventChannel<TEventArgs> : IEventChannel
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
            ISubscriber subscriber);
    }
}