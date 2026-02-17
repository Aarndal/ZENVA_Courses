using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace EventTransmission
{
    /// <summary>
    /// Marker interface to indicate the participant is an event.
    /// Defines whether the event is asynchronous or synchronous.
    /// </summary>
    public interface IEvent : IEventParticipant
    {
        /// <summary>
        /// Value that indicates whether the event is asynchronous.
        /// </summary>
        bool IsAsync { get; }
    }

    /// <summary>
    /// Interface for events that can be raised and handled.
    /// Does not specify a concrete event handler type.
    /// For specific event handler types, see ISyncEvent and IAsyncEvent, or create custom event interfaces.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public interface IEvent<TEventArgs> : IEvent
        where TEventArgs : IEventArgs
    {
        /// <summary>
        /// Default EventArgs instance used for debugging or when no specific data is needed.
        /// </summary>
        TEventArgs DefaultEventArgs { get; }
        /// <summary>
        /// The type of EventArgs associated with this event.
        /// </summary>
        Type EventArgsType { get; }

        /// <summary>
        /// Raises the event with the provided arguments and an optional publisher.
        /// The publisher can be null, in which case a default publisher is used.
        /// </summary>
        /// <param name="args">Event arguments to pass to subscribers.</param>
        /// <param name="publisher">Publisher of the event; can be null.</param>
        /// <returns>true if the event was successfully raised; false otherwise.</returns>
        bool Raise(TEventArgs args, IPublisher publisher = null);
        /// <summary>
        /// Default method to raise the event using DefaultEventArgs.
        /// Used primarily for debugging or when no specific data is needed.
        /// </summary>
        /// <param name="publisher">Publisher of the event; can be null.</param>
        /// <returns></returns>
        bool RaiseDefault(IPublisher publisher = null);
    }

    /// <summary>
    /// Interface for synchronous events that can be raised and handled synchronously.
    /// Event handlers are expected to return void.
    /// Event handlers are expected to follow the standard EventHandler pattern with two parameters: sender and eventArgs.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public interface ISyncEvent<TEventArgs> : IEvent<TEventArgs>
        where TEventArgs : IEventArgs
    {
        /// <summary>
        /// Event Handler for subscribers to listen to.
        /// </summary>
        event EventHandler<TEventArgs> EventRaised;
    }

    /// <summary>
    /// Interface for asynchronous events that can be raised and handled asynchronously.
    /// Async events can be raised in parallel or sequentially based on the publishParallel flag.
    /// Event handlers are expected to return a UniTask, allowing for asynchronous processing.
    /// Event handlers are expected to follow the custom AsyncEventHandler pattern of this namespace with three parameters: sender, eventArgs, and cancellationToken.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public interface IAsyncEvent<TEventArgs> : IEvent<TEventArgs>
        where TEventArgs : IEventArgs
    {
        /// <summary>
        /// Async Event Handler for subscribers to listen to.
        /// </summary>
        event AsyncEventHandler<TEventArgs> AsyncEventRaised;

        /// <summary>
        /// Raises the event asynchronously with the provided arguments and an optional publisher.
        /// The publisher can be null, in which case a default publisher is used.
        /// The publishParallel flag indicates whether to invoke subscribers in parallel or sequentially.
        /// </summary>
        /// <param name="publishParallel">Indicates if subscribers should be invoked in parallel or sequentially.</param>
        /// <param name="args">Event arguments to pass to subscribers.</param>
        /// <param name="publisher">Publisher of the event; can be null.</param>
        /// <param name="externalToken">Cancellation token that is passed to subscribers to allow for cooperative cancellation.</param>
        /// <returns></returns>
        UniTask<bool> RaiseAsync(bool publishParallel, TEventArgs args, IPublisher publisher = null, CancellationToken externalToken = default);
    }
}
