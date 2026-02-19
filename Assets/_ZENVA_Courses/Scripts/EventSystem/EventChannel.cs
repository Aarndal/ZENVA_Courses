using Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EventSystem
{
    public class EventChannel<TEventArgs> : IEventChannel<TEventArgs>
        where TEventArgs : IEventArgs
    {
        // Private Members
        private readonly HashSet<(ISubscriber Subscriber, Action<TEventArgs> Handler, Func<TEventArgs, bool> Filter)> _subscribedHandlers = new();

        // Properties
        public int SubscriberCount => _subscribedHandlers.Count;

        // Events
        //public event Action<TEventArgs> EventRaised;
        public event Action<IEventChannel> DisposalRequested;


        #region Public Methods
        public void Dispose()
        {
            if(SubscriberCount > 0)
            {
                DebugLogger.Log(
                    LogMessageType.Warning, 
                    this, 
                    "Attempting to dispose EventChannel while there are still handlers subscribed: {0} subscribers", 
                    true, 
                    SubscriberCount);
                return;
            }

            _subscribedHandlers?.Clear();

            if(DisposalRequested != null)
            {
                DisposalRequested = null;
            }
        }

        public bool TryPublish(TEventArgs args, IPublisher publisher = null)
        {
            if (SubscriberCount == 0)
                return false;
            foreach (var (Subscriber, Handler, _) in _subscribedHandlers)
            {
                Debug.Log($"Invoking handler {Handler.Method.Name} for subscriber {Subscriber.ID} with event args: {args}");
                DebugLogger.Log(
                    LogMessageType.Message,
                    this,
                    "Invoking handler {0} for subscriber {1} with event args: {2}",
                    true,
                    Handler.Method.Name,
                    Subscriber.ID,
                    args);
            }
            return true;
        }

        public bool TrySubscribe(ISubscriber subscriber, Action<TEventArgs> handler, Func<TEventArgs, bool> filter = null)
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

            if (!_subscribedHandlers.Add((subscriber, handler, filter)))
            {
                Debugging.DebugLogger.Log(
                    LogMessageType.Warning, 
                    this, 
                    "Subscriber is already subscribed with the same handler: {0} (Handler: {1})" +
                    "\nSubscription ignored.", 
                    true, 
                    subscriber.ID,
                    handler.Method.Name);
                return false;
            }

            //EventRaised += handler;

            Debug.Log($"Subscriber {subscriber.ID} subscribed with handler {handler.Method.Name}.");
            return true;
        }


        public bool TryUnsubscribe(ISubscriber subscriber)
        {
            if (subscriber == null)
                return false;

            var handlerToRemove = _subscribedHandlers.FirstOrDefault(sh => sh.Subscriber.Equals(subscriber));

            if (handlerToRemove.Equals(default))
                return false;

            _subscribedHandlers.Remove(handlerToRemove);
            Console.WriteLine($"Subscriber {subscriber.ID} unsubscribed from handler.");

            if (_subscribedHandlers.Count == 0)
            {
                DisposalRequested?.Invoke(this);
            }

            return true;
        }
        #endregion
    }
}