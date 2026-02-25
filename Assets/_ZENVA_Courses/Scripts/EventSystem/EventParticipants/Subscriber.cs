using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSystem
{
    public class Subscriber : ISubscriber
    {
        private readonly HashSet<IEventChannel> _subscribedChannels = new();

        public Guid EventGuid { get; }

        public Subscriber(Guid id)
        {
            EventGuid = id;
        }

        public bool TryAddHandlerToSubscription<TEventArgs>(Action<TEventArgs> handler, Predicate<TEventArgs> filter = null)
            where TEventArgs : IEventArgs
        {
            if (TryGetChannel<TEventArgs>(out var channel))
            {
                if (channel.TrySubscribe(this, handler, filter))
                {
                    _subscribedChannels.Add(channel);
                    return true;
                }
            }

            return false;
        }

        public void UnsubscribeAll()
        {
            foreach (var channel in _subscribedChannels)
            {
                if(channel is IEventChannel<IEventArgs> genericChannel)
                {
                    genericChannel.TryUnsubscribe(this);
                }
            }
            _subscribedChannels.Clear();
        }

        private bool TryGetChannel<T>(out IEventChannel<T> channel)
            where T : IEventArgs
        {
            channel = _subscribedChannels.FirstOrDefault(
                c => c is IEventChannel<T>) as IEventChannel<T>;

            if (channel == null || channel == default)
            {
                EventTransmitter.TryGetChannel<T>(this, out var newChannel);
                channel = newChannel;
            }

            return channel != null;
        }

        public bool Equals(IEventParticipant other)
        {
            if (other is ISubscriber subscriber)
            {
                return EventGuid == subscriber.EventGuid;
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            return obj is Subscriber other && Equals(other);
        }
        public override int GetHashCode()
        {
            return EventGuid.GetHashCode();
        }
    }
}
