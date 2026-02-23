using Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

namespace EventSystem
{
    /// <summary>
    /// A communication channel for a specific type of event arguments (<see cref="IEventArgs"/>).
    /// It allows <see cref="ISubscriber"/> to register or deregister their interest in events of type TEventArgs and <see cref="IPublisher"/> to raise events of that type.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event arguments (<see cref="IEventArgs"/>) that this channel handles.</typeparam>
    public class EventChannel<TEventArgs> : IEventChannel<TEventArgs>
        where TEventArgs : IEventArgs
    {
        // Private Members
        private readonly Dictionary<ISubscriber, SubscriberInfo<TEventArgs>> _subscriberInfo = new();

        // Properties
        public int SubscriberCount => _subscriberInfo.Count;

        // Events
        //public event Action<TEventArgs> EventRaised;
        public event Action<IEventChannel> DisposalRequested;


        #region Public Methods
        public void Dispose()
        {
            if (SubscriberCount > 0)
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "Attempting to dispose EventChannel while there are still handlers subscribed: {0} subscribers",
                    true,
                    SubscriberCount);
                return;
            }

            _subscriberInfo?.Clear();

            if (DisposalRequested != null)
            {
                DisposalRequested = null;
            }
        }

        /// <summary>
        /// Tries to publish an event with the given arguments. 
        /// It checks all subscribed handlers and their filters before invoking them.
        /// </summary>
        /// <param name="args">The <see cref="IEventArgs"/> to publish.</param>
        /// <param name="publisher">The <see cref="IPublisher"/> of the event.</param>
        /// <returns>true if the event was successfully published; otherwise, false.</returns>
        public bool TryPublish(TEventArgs args)
        {
            if (SubscriberCount == 0)
            {
                DebugLogger.Log(
                    LogMessageType.WarningFormatted,
                    this,
                    "Publish attempt of an event with no subscribers: {0} | PublisherID: {1}" +
                    "\nEvent will not be raised: {2} | EventID: {3}",
                    true,
                    args?.Publisher?.Name,
                    args?.Publisher?.ID,
                    args?.ToString(),
                    args?.ID);
                return false;
            }

            foreach (var subscriber in _subscriberInfo)
            {
                if (subscriber.Value.Predicate != null && !subscriber.Value.Predicate.Invoke(args))
                    continue;

                //if (subscriber.Key.EventQueuePerChannel[this] != null)
                //{
                //    subscriber.Key.EventQueuePerChannel[this].EnqueueEvent(args);
                //    continue;
                //}

                subscriber.Value.Handler?.Invoke(args);
            }
            return true;
        }

        public bool TrySubscribe(ISubscriber subscriber, Action<TEventArgs> handler, Predicate<TEventArgs> predicate = null)
        {
            if (subscriber == null || handler == null)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Attempting to subscribe with null subscriber or handler. Subscription failed.",
                    true);
                return false;
            }

            if (!_subscriberInfo.TryAdd(subscriber, new SubscriberInfo<TEventArgs>(handler, predicate)))
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "Subscriber is already subscribed with the same handler: {0} (Handler: {1})" +
                    "\nSubscription ignored.",
                    true,
                    subscriber.ID,
                    handler.Method.Name);
                return false;
            }

            Debug.Log($"Subscriber {subscriber.ID} subscribed with handler {handler.Method.Name}.");

            return true;
        }


        public bool TryUnsubscribe(ISubscriber subscriber)
        {
            if (subscriber == null)
                return false;

            if (!_subscriberInfo.Remove(subscriber))
            {
                DebugLogger.Log(     
                    LogMessageType.Warning,     
                    this,     
                    "Attempting to unsubscribe a subscriber that is not currently subscribed: {0}" +
                    "\nUnsubscription ignored.",
                    true,
                    subscriber.ID);
                return false;
            }

            if (_subscriberInfo.Count == 0)
            {
                DisposalRequested?.Invoke(this);
            }

            return true;
        }
        #endregion
    }
}