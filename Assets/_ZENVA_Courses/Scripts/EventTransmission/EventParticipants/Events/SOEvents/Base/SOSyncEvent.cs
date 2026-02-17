using System;

namespace EventTransmission
{
    public abstract class SOSyncEvent<TEventArgs> : SOEvent<TEventArgs>, ISyncEvent<TEventArgs>
        where TEventArgs : IEventArgs
    {
        // Events
        public event EventHandler<TEventArgs> EventRaised // Event to manage subscriptions. No invoke here.
        {
            add
            {
                if (value.Target is ISubscriber subscriber)
                    GlobalEventTransmitter.Subscribe<TEventArgs, EventHandler<TEventArgs>>(value, subscriber);
                else
                    GlobalEventTransmitter.Subscribe<TEventArgs, EventHandler<TEventArgs>>(value);
            }
            remove
            {
                if (value.Target is ISubscriber subscriber)
                    GlobalEventTransmitter.Unsubscribe<TEventArgs, EventHandler<TEventArgs>>(value, subscriber);
                else
                    GlobalEventTransmitter.Unsubscribe<TEventArgs, EventHandler<TEventArgs>>(value);
            }
        }
    }
}