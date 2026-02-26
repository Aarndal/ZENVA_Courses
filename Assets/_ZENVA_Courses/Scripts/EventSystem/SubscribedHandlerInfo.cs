using System;

namespace EventSystem
{
    public readonly struct SubscribedHandlerInfo<TEventArgs> : IEquatable<SubscribedHandlerInfo<TEventArgs>>
        where TEventArgs : IEventArgs
    {
        public Action<TEventArgs> Handler { get; }
        public Predicate<TEventArgs> Predicate { get; }

        public SubscribedHandlerInfo(Action<TEventArgs> handler, Predicate<TEventArgs> predicate = null)
        {
            Handler = handler;
            Predicate = predicate;
        }

        public bool Equals(SubscribedHandlerInfo<TEventArgs> other)
        {
            return Equals(Handler, other.Handler) && Equals(Predicate, other.Predicate);
        }

        public override bool Equals(object obj)
        {
            if(!base.Equals(obj))
                return false;

            if (obj is not SubscribedHandlerInfo<TEventArgs> other)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Handler, Predicate);
        }
    }
}