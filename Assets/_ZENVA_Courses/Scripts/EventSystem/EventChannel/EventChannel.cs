using Debugging;
using System;
using System.Collections.Generic;

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
        private readonly Dictionary<ISubscriber, HashSet<SubscribedHandlerInfo<TEventArgs>>> _subscriberInfo = new();
        private readonly List<SubscribedHandlerInfo<TEventArgs>> _publishSnapshot = new();

        public int SubscriberCount => _subscriberInfo.Count;

        public event Action<IEventChannel> DisposalRequested;

        private bool CheckFor(ISubscriber subscriber)
        {
            if (subscriber == null)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Attempting to unsubscribe null." +
                    "\nUnsubscription failed.",
                    true);
                return false;
            }

            if (!_subscriberInfo.ContainsKey(subscriber))
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "Attempting to unsubscribe a subscriber that is not currently subscribed: {0}" +
                    "\nUnsubscription ignored.",
                    true,
                    subscriber.EventID);
                return false;
            }
            return true;
        }

        private void OnUnsubscribeRequested(ISubscriber subscriber)
        {
            TryUnsubscribe(subscriber);
        }

        public bool TryPublish(TEventArgs args)
        {
            if (args == null)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Attempting to publish an event with null arguments. Publish failed.",
                    true);
                return false;
            }

            if (SubscriberCount == 0)
            {
                DebugLogger.Log(
                    LogMessageType.WarningFormatted,
                    this,
                    "Publish attempt of an event with no subscribers: {0} | PublisherID: {1}" +
                    "\nEvent will not be raised: {2} | EventType: {3}",
                    true,
                    args?.Publisher?.UniqueKey,
                    args?.Publisher?.EventID,
                    args?.ToString(),
                    typeof(TEventArgs).Name);
                return false;
            }

            if (!args.AreValid)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Attempting to publish an event with invalid arguments. Publish failed. EventArgs: {0} | EventType: {1}",
                    true,
                    args.ToString(),
                    typeof(TEventArgs).Name);
                return false;
            }

            _publishSnapshot.Clear();
            foreach (var handlerSet in _subscriberInfo.Values)
                foreach (var info in handlerSet)
                    _publishSnapshot.Add(info);

            foreach (var info in _publishSnapshot)
            {
                if (info.Predicate != null && !info.Predicate.Invoke(args))
                    continue;

                info.Handler?.Invoke(args);
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

            if (!_subscriberInfo.ContainsKey(subscriber))
            {
                if (!_subscriberInfo.TryAdd(subscriber, new HashSet<SubscribedHandlerInfo<TEventArgs>>()))
                {
                    DebugLogger.Log(
                        LogMessageType.Error,
                        this,
                        "Failed to add subscriber to the subscriber info dictionary. Subscription failed. SubscriberID: {0}",
                        true,
                        subscriber.EventID);
                    return false;
                }

                subscriber.UnsubscribeRequested += OnUnsubscribeRequested;
            }

            if (!_subscriberInfo[subscriber].Add(new SubscribedHandlerInfo<TEventArgs>(handler, predicate)))
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "Subscriber is already subscribed with the same handler and predicate. Subscription ignored. SubscriberID: {0} | Handler: {1} | Predicate: {2}",
                    true,
                    subscriber.EventID,
                    handler.Method.Name,
                    predicate != null ? predicate.Method.Name : "null");
                return false;
            }

            DebugLogger.Log(
                LogMessageType.Info,
                this,
                "Subscriber {0} subscribed with handler {1}.",
                true,
                subscriber.EventID,
                handler.Method.Name);

            return true;
        }

        public bool TryUnsubscribe(ISubscriber subscriber, Action<TEventArgs> handler)
        {
            if (!CheckFor(subscriber))
                return false;

            if (handler == null)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    subscriber,
                    "Attempting to unsubscribe with null handler." +
                    "\nUnsubscription failed.",
                    true);
                return false;
            }

            var unsubscribedHandlers = _subscriberInfo[subscriber].RemoveWhere(info => info.Handler == handler);

            if (unsubscribedHandlers == 0)
                return false;

            if (_subscriberInfo[subscriber].Count == 0)
            {
                subscriber.UnsubscribeRequested -= OnUnsubscribeRequested;
                _subscriberInfo.Remove(subscriber);

                if (_subscriberInfo.Count == 0)
                    DisposalRequested?.Invoke(this);
            }

            return true;
        }

        public bool TryUnsubscribe(ISubscriber subscriber)
        {
            if (!CheckFor(subscriber))
                return false;

            subscriber.UnsubscribeRequested -= OnUnsubscribeRequested;

            _subscriberInfo[subscriber].Clear();
            _subscriberInfo.Remove(subscriber);

            if (_subscriberInfo.Count == 0)
            {
                DisposalRequested?.Invoke(this);
            }

            return true;
        }

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
            _publishSnapshot?.Clear();

            if (DisposalRequested != null)
            {
                DisposalRequested = null;
            }
        }
    }
}
