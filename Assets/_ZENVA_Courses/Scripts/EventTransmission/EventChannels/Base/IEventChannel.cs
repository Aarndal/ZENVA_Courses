using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace EventTransmission
{
    /// <summary>
    /// Base EventChannel interface
    /// </summary>
    public interface IEventChannel : IDisposable
    {
        /// <summary>
        /// Gets the type of the EventArgs associated with this EventChannel.
        /// </summary>
        Type EventArgsType { get; }

        /// <summary>
        /// Returns true if the EventChannel supports asynchronous event raising.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// The name of the EventChannel, typically derived from the EventArgs type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The subscribers to this EventChannel and their associated handlers.
        /// </summary>
        IReadOnlyDictionary<ulong, ImmutableHashSet<Delegate>> Subscribers { get; }
    }

    public interface IEventChannel<in TEventArgs, in THandler> : IEventChannel
        where TEventArgs : IEventArgs
        where THandler : Delegate
    {
        /// <summary>
        /// Subscribes a handler to this event channel.
        /// </summary>
        /// <param name="handler">The event handler that is subscribed.</param>
        /// <param name="subscriber">The subscriber to the EventChannel that provides a unique Guid.</param>
        /// <returns>True if successfully subscribed, false otherwise.</returns>
        bool Subscribe(THandler handler, ISubscriber subscriber);

        /// <summary>
        /// Unsubscribes a handler from this event channel.
        /// </summary>
        /// <param name="handler">The event handler that is unsubscribed.</param>
        /// <param name="subscriber">The subscriber to the EventChannel that provides a unique Guid.</param>
        /// <returns>True if successfully unsubscribed, false otherwise.</returns>
        bool Unsubscribe(THandler handler, ISubscriber subscriber);

        bool RaiseEvent(TEventArgs eventArgs, object publisher = null);
    }

    public interface IAsyncEventChannel<TEventArgs> : IEventChannel<TEventArgs, AsyncEventHandler<TEventArgs>>
        where TEventArgs : IEventArgs
    {
        UniTask<bool> RaiseEventAsync(TEventArgs eventArgs, object publisher = null, CancellationToken externalToken = default);
        UniTask<bool> RaiseEventParallel(TEventArgs eventArgs, object publisher = null, CancellationToken externalToken = default);
    }
}
