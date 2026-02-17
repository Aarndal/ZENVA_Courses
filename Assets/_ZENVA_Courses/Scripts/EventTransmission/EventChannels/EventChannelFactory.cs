using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace EventTransmission
{
    /// <summary>
    /// A factory for creating event channels based on event argument and handler types.
    /// Allows registration of custom channel creators for extensibility.
    /// </summary>
    public static class EventChannelFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<Type, object>> _channelCreators = new();

        static EventChannelFactory()
        {
            // Register creator for synchronous channels (EventHandler<TEventArgs>)
            RegisterCreator(typeof(EventHandler<>), eventArgsType =>
                Activator.CreateInstance(typeof(EventChannel<>).MakeGenericType(eventArgsType)));

            // Register creator for asynchronous channels (AsyncEventHandler<TEventArgs>)
            RegisterCreator(typeof(AsyncEventHandler<>), eventArgsType =>
                Activator.CreateInstance(typeof(AsyncEventChannel<>).MakeGenericType(eventArgsType)));
        }

        #region PublicMethods

        /// <summary>
        /// Registers a creator function for the specified open generic handler type.
        /// Overwrites any existing creator for that handler type.
        /// </summary>
        /// <param name="openGenericHandlerType">The open generic delegate type, e.g. typeof(EventHandler&lt;&gt;)</param>
        /// <param name="creatorFunction">Function that receives the event argument type and returns a new event channel instance.</param>
        public static void RegisterCreator(Type openGenericHandlerType, Func<Type, object> creatorFunction)
        {
            if (creatorFunction is null || openGenericHandlerType is null)
            {
#if UNITY_EDITOR
                Debug.LogError("Channel creator function and handler type cannot be null.");
#endif
                return;
            }
            _channelCreators.AddOrUpdate(openGenericHandlerType, creatorFunction, (_, _) => creatorFunction);
        }

        /// <summary>
        /// Attempts to register a creator function for the specified open generic handler type.
        /// Registration fails if a creator for that handler type already exists or the function is null.
        /// </summary>
        public static bool TryRegisterCreator(Type openGenericHandlerType, Func<Type, object> creatorFunction)
        {
            if (creatorFunction is null || openGenericHandlerType is null)
            {
#if UNITY_EDITOR
                Debug.LogError("Channel creator function and handler type cannot be null.");
#endif
                return false;
            }
            return _channelCreators.TryAdd(openGenericHandlerType, creatorFunction);
        }

        /// <summary>
        /// Attempts to create an event channel for the specified event argument and handler types.
        /// Returns false if the handler type is not supported or the channel instance could not be created.
        /// </summary>
        public static bool TryCreate<TEventArgs, THandler>(out IEventChannel<TEventArgs, THandler> eventChannel)
            where TEventArgs : IEventArgs
            where THandler : Delegate
        {
            eventChannel = null;
            var handlerType = typeof(THandler);

            // Use open generic type for lookup
            if (handlerType.IsGenericType)
                handlerType = handlerType.GetGenericTypeDefinition();

            if (!_channelCreators.TryGetValue(handlerType, out var creatorFunction))
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("No event channel creator registered for EventHandler: {0}",
                    typeof(THandler).Name);
#endif
                return false;
            }

            var createdObject = creatorFunction(typeof(TEventArgs));

            if (createdObject is IEventChannel<TEventArgs, THandler> channel)
            {
                eventChannel = channel;
                return true;
            }

#if UNITY_EDITOR
            Debug.LogErrorFormat("Registered creator for handler did not produce a valid channel instance for {1}: {0}",
                typeof(THandler).Name, typeof(TEventArgs).Name);
#endif
            return false;
        }

        #endregion
    }
}