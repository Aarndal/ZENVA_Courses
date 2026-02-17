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
    /// <summary>
    /// Manages global event transmission through a publish/subscribe system.
    /// Registers EventChannels for different EventArgs types and allows subscribers to register handlers for those events.
    /// If no EventChannel exists for a given EventArgs type, one will be created automatically upon subscription.
    /// If a publisher attempts to publish an event for which no EventChannel exists, the publication will be ignored and false will be returned.
    /// </summary>
    public static class GlobalEventTransmitter
    {
        // Constants
        private const ushort MAX_ITERATION_COUNT = 60;


        // Private Members
        private static readonly ConcurrentDictionary<Type, ImmutableHashSet<IEventChannel>> _eventChannels = new();
        private static readonly ConcurrentDictionary<Type, object> _locks = new();


        // Properties
        public static IReadOnlyDictionary<Type, ImmutableHashSet<IEventChannel>> EventChannels => new ReadOnlyDictionary<Type, ImmutableHashSet<IEventChannel>>(_eventChannels);


        #region Pub/Sub System Methods
        /// <summary>
        /// Subscribes the specified handler to the global event system for the specified EventArgs type.
        /// Handlers must be unique per subscriber. If a handler is already subscribed by the same subscriber, the subscription will be ignored and false will be returned.
        /// If no EventChannel exists for the specified EventArgs type, a new one will be created. If multiple EventChannels exist for the same EventArgs type and handler type, an error will be logged and the subscription will be ignored, returning false.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler">Must be a compatible delegate type for the specified EventArgs type.</param>
        /// <param name="subscriber">Must implement ISubscriber to provide a unique Guid for the subscription.</param>
        /// <returns>true if the handler was successfully subscribed; otherwise, false.</returns>
        public static bool Subscribe<TEventArgs, THandler>(THandler handler, ISubscriber subscriber = null)
            where TEventArgs : IEventArgs
            where THandler : Delegate
        {
            if (handler is null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{0} attempted to subscribe a null handler for {1}.", subscriber, typeof(TEventArgs).Name);
#endif
                return false;
            }

            if (TryGetEventChannel<TEventArgs, THandler>(out var eventChannel))
                return eventChannel.Subscribe(handler, subscriber);

            if (!EventChannelFactory.TryCreate(out eventChannel))
                return false;

            return TryRegisterEventChannel(eventChannel) && eventChannel.Subscribe(handler, subscriber);
        }

        /// <summary>
        /// Unsubscribes the specified handler from the global event system for the specified EventArgs type.
        /// Returns false if no EventChannel exists for the specified EventArgs and handler type, or if the handler was not registered for the subscriber.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <param name="subscriber"></param>
        /// <returns>true if the handler was successfully unsubscribed; otherwise, false.</returns>
        public static bool Unsubscribe<TEventArgs, THandler>(THandler handler, ISubscriber subscriber = null)
            where TEventArgs : IEventArgs
            where THandler : Delegate
        {
            if (handler is null)
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{0} attempted to unsubscribe a null handler for {1}.", subscriber, typeof(TEventArgs).Name);
#endif
                return false;
            }

            return TryGetEventChannel<TEventArgs, THandler>(out var eventChannel) &&
                   eventChannel.Unsubscribe(handler, subscriber);
        }

        public static bool Publish<TEventArgs>(TEventArgs eventArgs, object publisher = null)
            where TEventArgs : IEventArgs
        {
            if (!eventArgs.IsValid)
                return false;

            var lockObject = _locks.GetOrAdd(typeof(TEventArgs), _ => new object());

            lock (lockObject)
            {
                return TryGetEventChannel<TEventArgs, EventHandler<TEventArgs>>(out var eventChannel) && eventChannel.RaiseEvent(eventArgs, publisher);
            }
        }

        public static async UniTask<bool> PublishAsync<TEventArgs>(TEventArgs eventArgs, object publisher = null, CancellationToken externalToken = default)
            where TEventArgs : IEventArgs
        {
            if (!eventArgs.IsValid)
                return false;

            if (!TryGetEventChannel<TEventArgs, AsyncEventHandler<TEventArgs>>(out var eventChannel) ||
               eventChannel is not IAsyncEventChannel<TEventArgs> asyncChannel)
                return false;

            return await asyncChannel.RaiseEventAsync(eventArgs, publisher, externalToken);
        }

        public static async UniTask<bool> PublishParallel<TEventArgs>(TEventArgs eventArgs, object publisher = null, CancellationToken externalToken = default)
            where TEventArgs : IEventArgs
        {
            if (!eventArgs.IsValid)
                return false;

            if (!TryGetEventChannel<TEventArgs, AsyncEventHandler<TEventArgs>>(out var eventChannel) ||
                eventChannel is not IAsyncEventChannel<TEventArgs> asyncChannel)
                return false;

            return await asyncChannel.RaiseEventParallel(eventArgs, publisher, externalToken);
        }
        #endregion


        #region EventChannel System Methods

        // Public Methods

        /// <summary>
        /// Requests the disposal of the specified EventChannel from the global event system.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="eventChannel"></param>
        public static void RequestEventChannelDisposal<TEventArgs, THandler>(IEventChannel eventChannel)
            where TEventArgs : IEventArgs
            where THandler : Delegate
        {
            if (eventChannel is null)
                return; // Nothing to dispose of.

            // Disposes of the EventChannel resources.
            eventChannel.Dispose();

            // Checks if the EventChannel is registered.
            if (!TryGetEventChannel<TEventArgs, THandler>(out var registeredEventChannel))
                return;

            ushort iterationCount = 0;

            // Attempts to remove the EventChannel from the registered channels.
            while (iterationCount < MAX_ITERATION_COUNT)
            {
                var eventType = typeof(TEventArgs);
                if (!_eventChannels.TryGetValue(eventType, out var registeredChannels))
                    break; // No registered channels to remove.

                if (registeredChannels is null ||
                    registeredChannels.Count <= 0 ||
                    !registeredChannels.Contains(registeredEventChannel))
                    break; // Nothing to remove.

                var updatedRegisteredChannels = registeredChannels.Remove(registeredEventChannel);

                if (updatedRegisteredChannels.Count == 0)
                {
                    // Remove the whole key for empty set
                    _eventChannels.TryRemove(eventType, out _);
                    break;
                }

                if (_eventChannels.TryUpdate(eventType, updatedRegisteredChannels, registeredChannels))
                    break;

                iterationCount++;
            }
        }

        /// <summary>
        /// Tries to create and register a new EventChannel of the specified type.
        /// If the EventChannel already exists, it will be returned instead.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="eventChannel"></param>
        /// <returns>true if a new EventChannel was created and registered; false if it already existed or otherwise.</returns>
        public static bool TryCreateAndRegisterEventChannel<TEventArgs, THandler>(out IEventChannel<TEventArgs, THandler> eventChannel)
            where TEventArgs : IEventArgs
            where THandler : Delegate
        {
            if (TryGetEventChannel(out eventChannel))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("EventChannel with event handler already exists for {0}: {1}", typeof(TEventArgs).Name, typeof(THandler).Name);
#endif
                return false;
            }

            return EventChannelFactory.TryCreate(out eventChannel) && TryRegisterEventChannel(eventChannel);
        }

        /// <summary>
        /// Tries to get a registered EventChannel of the specified type.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="eventChannel">A registered EventChannel of the requested type if found; otherwise, null.</param>
        /// <returns>true if exactly one registered EventChannel is found; otherwise, false.</returns>
        public static bool TryGetEventChannel<TEventArgs, THandler>(out IEventChannel<TEventArgs, THandler> eventChannel)
            where TEventArgs : IEventArgs
            where THandler : Delegate
        {
            var lockObject = _locks.GetOrAdd(typeof(TEventArgs), _ => new object());

            lock (lockObject)
            {
                var eventType = typeof(TEventArgs);

                // Checks if there are any EventChannels registered for the given EventArgs type.
                if (!_eventChannels.TryGetValue(eventType, out var registeredChannels))
                {
                    eventChannel = null;
                    return false;
                }

                HashSet<IEventChannel<TEventArgs, THandler>> typedChannels = new(registeredChannels.Count);

                // Gathers all registered EventChannels of the requested type.
                foreach (var channel in registeredChannels)
                {
                    if (channel is not IEventChannel<TEventArgs, THandler> typedChannel)
                        continue;

                    typedChannels.Add(typedChannel);
                }

                // Checks how many EventChannels of the requested type were found. Only one is allowed!
                switch (typedChannels.Count)
                {
                    // Exactly one EventChannel for EventArgs found that is of the requested type.
                    case 1:
                        {
                            eventChannel = typedChannels.First();
                            return true;
                        }
                    // No EventChannel for EventArgs found that is of the requested type.
                    case 0:
                        {
                            eventChannel = null;
                            return false;
                        }
                    // More than one EventChannel for EventArgs found that is of the requested type. Hard fail: This should never happen!
                    default:
                        {
#if DEBUG
                            System.Diagnostics.Debug.Fail(
                                $"Multiple EventChannels of the same (handler) type found for {eventType.Name}, but there should only be one: {typeof(THandler).Name}");
#endif
#if UNITY_EDITOR
                            Debug.LogErrorFormat(
                                "FATAL: Multiple EventChannels of the same (handler) type found for {0}, but there should only be one: {1}\nRegistered channels: {2}",
                                eventType.Name, typeof(THandler).Name, typedChannels.Count);
#else
                        Console.WriteLine(
                            $"FATAL: Multiple EventChannels of the same (handler) type found for {eventType.Name}, but there should only be one: {typeof(THandler).Name}. Registered channels: {typedChannels.Count}");
#endif
                            throw new InvalidOperationException(
                                $"Multiple EventChannels of type {eventType.Name} and handler {typeof(THandler).Name} were found. This indicates a critical bug.");
                        }
                }
            }
        }


        // Private Methods

        /// <summary>
        /// Attempts to register the specified EventChannel with the global event system.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="eventChannel"></param>
        /// <returns></returns>
        private static bool TryRegisterEventChannel<TEventArgs, THandler>(IEventChannel<TEventArgs, THandler> eventChannel)
            where TEventArgs : IEventArgs where THandler : Delegate
        {
            var eventType = typeof(TEventArgs);
            var lockObject = _locks.GetOrAdd(eventType, _ => new object());

            lock (lockObject)
            {
                _eventChannels.AddOrUpdate(
                    eventType,
                    ImmutableHashSet<IEventChannel>.Empty.Add(eventChannel),
                    (_, existingEventChannels) => existingEventChannels.Add(eventChannel));
            }

            return _eventChannels[eventType].Contains(eventChannel);
        }
        
        #endregion
    }
}
