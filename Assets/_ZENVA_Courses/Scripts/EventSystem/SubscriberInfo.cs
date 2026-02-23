using System;

namespace EventSystem
{
    public readonly struct SubscriberInfo<TEventArgs> : IEquatable<SubscriberInfo<TEventArgs>>
        where TEventArgs : IEventArgs
    {
        public Action<TEventArgs> Handler { get; }
        public Predicate<TEventArgs> Predicate { get; }

        public SubscriberInfo(Action<TEventArgs> handler, Predicate<TEventArgs> predicate = null)
        {
            Handler = handler;
            Predicate = predicate;
        }

        public bool Equals(SubscriberInfo<TEventArgs> other)
        {
            return Handler.Equals(other.Handler) && Predicate.Equals(other.Predicate);
        }

        public override bool Equals(object obj)
        {
            return obj is SubscriberInfo<TEventArgs> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Handler, Predicate);
        }
    }
}