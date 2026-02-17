using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using UnityEngine;

namespace EventTransmission
{
    /// <summary>
    /// Synchronous EventChannel implementation for the GlobalEventTransmitter.
    /// Handles EventHandler<TEventArgs> subscriptions and invocation.
    /// </summary>
    public sealed class EventChannel<TEventArgs> : IEventChannel<TEventArgs, EventHandler<TEventArgs>>
        where TEventArgs : IEventArgs
    {
        // Private Members
        private static readonly DefaultSubscriber _defaultSubscriber = DefaultSubscriber.Instance;

        private readonly object _lock = new();
        private readonly ConcurrentDictionary<ulong, ImmutableHashSet<Delegate>> _subscribers = new();

        private bool _disposed = false;
        private EventHandler<TEventArgs> _eventRaised = null;


        // Properties
        public Type EventArgsType => typeof(TEventArgs);
        public bool IsAsync => false;
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
        ~EventChannel()
        {
            Dispose(false);
        }


        #region PublicMethods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Subscribe(EventHandler<TEventArgs> handler, ISubscriber subscriber)
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

        public bool Unsubscribe(EventHandler<TEventArgs> handler, ISubscriber subscriber)
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
        #endregion


        #region PrivateMethods
        /// <summary>
        /// Generates a name for the EventChannel based on the type TEventArgs.
        /// </summary>
        private static string GenerateName()
        {
            var typeName = typeof(TEventArgs).Name;

            if (typeName.EndsWith("Args", StringComparison.OrdinalIgnoreCase))
            {
                typeName = typeName[..^"Args".Length];
            }

            if (typeName.EndsWith("EventRaised", StringComparison.OrdinalIgnoreCase))
            {
                typeName += "Channel";
            }

            if (!typeName.EndsWith("EventChannel", StringComparison.OrdinalIgnoreCase))
            {
                typeName += "EventChannel";
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
                        _eventRaised = null;
                        _subscribers?.Clear();
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
                GlobalEventTransmitter.RequestEventChannelDisposal<TEventArgs, EventHandler<TEventArgs>>(this);
            }
        }

        /// <summary>
        /// Prevents operations on a disposed EventChannel by throwing an ObjectDisposedException.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EventChannel<TEventArgs>));
        }

        /// <summary>
        /// Gets the event handler associated with the specified event arguments.
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        private bool TryGetEventHandler(TEventArgs eventArgs, out EventHandler<TEventArgs> eventHandler)
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
