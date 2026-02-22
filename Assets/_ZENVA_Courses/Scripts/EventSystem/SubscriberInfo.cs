using System;

namespace EventSystem
{
    public readonly struct SubscriberInfo<TEventArgs> : IEquatable<SubscriberInfo<TEventArgs>>
        where TEventArgs : IEventArgs
    {
        public ISubscriber Subscriber { get; }
        public Action<TEventArgs> Handler { get; }
        public Func<TEventArgs, bool> Filter { get; }

        public SubscriberInfo(ISubscriber subscriber, Action<TEventArgs> handler, Func<TEventArgs, bool> filter = null)
        {
            Subscriber = subscriber;
            Handler = handler;
            Filter = filter;
        }

        public bool Equals(SubscriberInfo<TEventArgs> other)
        {
            return Subscriber.Equals(other.Subscriber) && Handler.Equals(other.Handler) && Filter.Equals(other.Filter);
        }

        public override bool Equals(object obj)
        {
            return obj is SubscriberInfo<TEventArgs> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Subscriber.GetHashCode();
        }
    }
}