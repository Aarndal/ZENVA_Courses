using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace EventTransmission
{
    public sealed class AsyncEventChannel<TEventArgs> : IAsyncEventChannel<TEventArgs>
        where TEventArgs : IEventArgs
    {
        // Private Members
        private static readonly DefaultSubscriber _defaultSubscriber = DefaultSubscriber.Instance;

        private readonly CancellationTokenSource _channelCts = new();
        private readonly object _lock = new();
        private readonly ConcurrentDictionary<ulong, ImmutableHashSet<Delegate>> _subscribers = new();

        private bool _disposed = false;
        private AsyncEventHandler<TEventArgs> _eventRaised = null;


        // Properties
        public Type EventArgsType => typeof(TEventArgs);
        public bool IsAsync => true;
        public string Name { get; } = GenerateName();
        public IReadOnlyDictionary<ulong, ImmutableHashSet<Delegate>> Subscribers
        {
            get
            {
                ThrowIfDisposed();
                return new ReadOnlyDictionary<ulong, ImmutableHashSet<Delegate>>(_subscribers);
            }
        }


        // Destructor
        ~AsyncEventChannel()
        {
            Dispose(false);
        }


        #region PublicMethods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Subscribe(AsyncEventHandler<TEventArgs> handler, ISubscriber subscriber)
        {
            ThrowIfDisposed();

            if (handler is null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Can't subscribe null handler to {0}.", Name);
#endif
                return false;
            }

            subscriber ??= _defaultSubscriber;

            var id = subscriber.EventParticipantID;

            if (!_subscribers.TryAdd(id, ImmutableHashSet<Delegate>.Empty))
            {
#if UNITY_EDITOR
                Debug.LogFormat("{1} tries to subscribe additional handler to {0}: {2}", Name, handler.Target, handler.Method.Name);
#endif
            }

            while (true)
            {
                var originalSubHandlers = _subscribers[id];

                if (originalSubHandlers.Contains(handler))
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat("Subscriber {1} already subscribed with handler to {0}: {2}", Name, handler.Target, handler.Method.Name);
#endif
                    return false;
                }

                var updatedSubHandlers = originalSubHandlers.Add(handler);

                if (_subscribers.TryUpdate(id, updatedSubHandlers, originalSubHandlers))
                    break;
            }

            lock (_lock)
            {
                _eventRaised += handler;
            }
            return true;
        }

        public bool Unsubscribe(AsyncEventHandler<TEventArgs> handler, ISubscriber subscriber)
        {
            ThrowIfDisposed();

            if (handler is null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Tried to unsubscribe null handler from {0}.", Name);
#endif
                return false;
            }

            subscriber ??= _defaultSubscriber;

            if (!_subscribers.ContainsKey(subscriber.EventParticipantID))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{1} tried to unsubscribe handler, but is not subscribed to {0}: {2}", Name, handler.Target, handler.Method.Name);
#endif
                return false;
            }

            while (true)
            {
                var originalSubHandlers = _subscribers[subscriber.EventParticipantID];

                if (!originalSubHandlers.Contains(handler))
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat("{1} tried to unsubscribe handler, but handler is not subscribed to {0}: {2}", Name, handler.Target, handler.Method.Name);
#endif
                    return false;
                }

                var updatedSubHandlers = originalSubHandlers.Remove(handler);

                if (_subscribers.TryUpdate(subscriber.EventParticipantID, updatedSubHandlers, originalSubHandlers))
                    break;
            }

            lock (_lock)
            {
                _eventRaised -= handler;
            }

            // Remove subscriber if no handlers remain
            if (_subscribers[subscriber.EventParticipantID].Count <= 0)
                RequestDisposal(subscriber);

            return true;
        }

        public bool RaiseEvent(TEventArgs eventArgs, object publisher = null)
        {
            ThrowIfDisposed();

            if (!TryGetEventHandler(eventArgs, out var eventHandler))
                return false;

            try
            {
                eventHandler?.Invoke(publisher ?? this, eventArgs);
                return true;
            }
            catch (Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Error raising event {0} in {1}: {2}", EventArgsType.Name, Name, exception.Message);
#else
                    Console.WriteLine(exception);
#endif
            }
            return false;
        }

        public async UniTask<bool> RaiseEventAsync(TEventArgs eventArgs, object publisher = null, CancellationToken externalToken = default)
        {
            ThrowIfDisposed();

            if (!TryGetEventHandler(eventArgs, out var eventHandler))
                return false;

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, _channelCts.Token);
            var token = linkedCts.Token;

            try
            {
                token.ThrowIfCancellationRequested();

                foreach (var handler in eventHandler.GetInvocationList())
                {
                    if (handler is AsyncEventHandler<TEventArgs> asyncEventHandler)
                        await asyncEventHandler(publisher ?? this, eventArgs, token);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Async event publishing was cancelled: {0}", Name);
#endif
            }
            catch (Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Error raising event {0} in {1}: {2}", EventArgsType.Name, Name, exception.Message);
#else
                Console.WriteLine(exception);
#endif
            }
            return false;
        }

        public async UniTask<bool> RaiseEventParallel(TEventArgs eventArgs, object publisher = null,
            CancellationToken externalToken = default)
        {
            ThrowIfDisposed();

            if (!TryGetEventHandler(eventArgs, out var eventHandler))
                return false;

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, _channelCts.Token);
            var token = linkedCts.Token;

            var invocationList = eventHandler.GetInvocationList();

            // Create a list of tasks to invoke each handler
            var tasks = invocationList.Cast<AsyncEventHandler<TEventArgs>>()
                .Select(handler => InvokeWithCatchAsync(handler, publisher, eventArgs, token))
                .ToArray();

            try
            {
                await UniTask.WhenAll(tasks);
                return true;
            }
            catch (OperationCanceledException)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Async parallel event publishing was cancelled: {0}", Name);
#endif
            }
            catch (Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Error raising event {0} in {1}: {2}", EventArgsType.Name, Name, exception.Message);
#else
                Console.WriteLine(exception);
#endif
            }
            return false;
        }
        #endregion


        #region PrivateMethods
        /// <summary>
        /// This method generates a name for the EventChannel based on the type T.
        /// It is called during the construction of the EventChannel instance.
        /// </summary>
        /// <returns>Returns the name of the EventChannel based on the EventArgs handled by it.</returns>
        private static string GenerateName()
        {
            var typeName = typeof(TEventArgs).Name;

            if (typeName.EndsWith("Args", StringComparison.OrdinalIgnoreCase))
            {
                typeName = typeName[..^"Args".Length]; // Remove "Args" suffix { [..^"Args".Length] == .Substring(0, _name.Length - "Args".Length) }
            }

            if (typeName.EndsWith("EventRaised", StringComparison.OrdinalIgnoreCase))
            {
                typeName += "Channel";
            }

            if (!typeName.EndsWith("EventChannel", StringComparison.OrdinalIgnoreCase))
            {
                typeName += "EventChannel"; // Add "EventChannel" suffix for better clarity
            }

            return typeName;
        }

        /// <summary>
        /// Disposes the EventChannel, unsubscribing all handlers and clearing resources.
        /// </summary>
        /// <param name="disposingManaged">Disposing managed resources if true.</param>
        private void Dispose(bool disposingManaged)
        {
            lock (_lock)
            {
                if (_disposed)
                    return;

                _disposed = true;

                try
                {
                    if (disposingManaged)
                    {
                        _channelCts?.Cancel();
                        _eventRaised = null;
                        _subscribers?.Clear();
                        _channelCts?.Dispose();
                    }
                }
                catch (Exception exception)
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("Error disposing {0}: {1}", Name, exception.Message);
#else
                    Console.WriteLine(exception);
#endif
                }
            }
        }

        /// <summary>
        /// This method invokes an asynchronous event handler with exception handling.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="publisher"></param>
        /// <param name="eventArgs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask InvokeWithCatchAsync(AsyncEventHandler<TEventArgs> handler, object publisher, TEventArgs eventArgs, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await handler(publisher ?? this, eventArgs, token);
            }
            catch (OperationCanceledException)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Async event handler was cancelled in {0}: {1}", Name, handler.Method.Name);
#endif
            }
            catch (Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Error invoking {2} method of {1} in {0}: {3}", Name, handler.Target, handler.Method.Name, exception.Message);
#else
                Console.WriteLine(exception);
#endif
            }
        }

        /// <summary>
        /// Removes the subscriber from the dictionary and requests disposal if there are no more subscribers.
        /// </summary>
        /// <param name="subscriber"></param>
        private void RequestDisposal(ISubscriber subscriber)
        {
            if (!_subscribers.TryRemove(subscriber.EventParticipantID, out _))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Failed to remove subscriber (ID: {1}) from {0} due to concurrent modification.", Name, subscriber.EventParticipantID);
#endif
            }

            if (_subscribers.Count <= 0)
            {
                GlobalEventTransmitter.RequestEventChannelDisposal<TEventArgs, AsyncEventHandler<TEventArgs>>(this);
            }
        }

        /// <summary>
        /// Prevents operations on a disposed EventChannel by throwing an ObjectDisposedException.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AsyncEventChannel<TEventArgs>));
        }

        /// <summary>
        /// Gets the event handler associated with the specified event arguments.
        /// </summary>
        /// <param name="eventArgs">The event arguments to validate.</param>
        /// <param name="eventHandler">When this method returns, contains the event handler associated with the specified event arguments, if they are valid and the event handler has any subscribers; otherwise, null.</param>
        /// <returns>true if the eventArgs are valid and subscribers exist; otherwise, false.</returns>
        private bool TryGetEventHandler(TEventArgs eventArgs, out AsyncEventHandler<TEventArgs> eventHandler)
        {
            if (!eventArgs.IsValid)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("Invalid eventArgs for {0}: {1}", Name, eventArgs);
#endif
                eventHandler = null;
                return false;
            }

            if (_eventRaised is null || _subscribers.Count <= 0)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("No Subscribers found for {0}.", Name);
#endif
                eventHandler = null;
                return false;
            }

            lock (_lock)
            {
                eventHandler = _eventRaised;
            }

            return true;
        }
        #endregion
    }
}
