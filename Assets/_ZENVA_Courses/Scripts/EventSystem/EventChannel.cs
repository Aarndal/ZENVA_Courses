using DebugLogger;
using System;
using System.Collections.Generic;
using System.Linq;

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
                DebugLogger.DebugLogger.Debug(
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
                // Here you would typically invoke the subscriber's event handler with the provided args.
                // This is a placeholder for demonstration purposes.
                Console.WriteLine($"Event published to subscriber {Subscriber.ID} with handler {Handler.Method.Name}, with args: {args}");
            }
            return true;
        }

        public bool TrySubscribe(ISubscriber subscriber, Action<TEventArgs> handler, Func<TEventArgs, bool> filter = null)
        {
            if (subscriber == null || handler == null)
            {
                DebugLogger.DebugLogger.Debug(
                    LogMessageType.Error, 
                    this, 
                    "Attempting to subscribe with null subscriber or handler. Subscription failed.", 
                    true);
                return false;
            }

            if (!_subscribedHandlers.Add((subscriber, handler, filter)))
            {
                DebugLogger.DebugLogger.Debug(
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

            // Here you would typically store the handler and filter for later invocation when an event is published.
            // This is a placeholder for demonstration purposes.
            Console.WriteLine($"Subscriber {subscriber.ID} subscribed with handler {handler.Method.Name}.");
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