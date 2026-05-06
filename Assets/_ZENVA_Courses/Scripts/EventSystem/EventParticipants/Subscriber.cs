using System;
using System.Collections.Generic;

namespace EventSystem
{
    public class Subscriber : ISubscriber
    {
        private readonly HashSet<IEventChannel> _subscribedChannels = new();

        public uint EventID { get; private set; }
        public string UniqueKey { get; }

        public event Action<ISubscriber> UnsubscribeRequested;

        public Subscriber(string uniqueKey)
        {
            UniqueKey = uniqueKey;
            EventID = EventSystemIDManager.GetParticipantID(this);
        }

        private bool TryGetChannel<T>(out IEventChannel<T> channel)
            where T : IEventArgs
        {
            channel = null;

            foreach (var c in _subscribedChannels)
            {
                if (c is IEventChannel<T> typedChannel)
                {
                    channel = typedChannel;
                    return true;
                }
            }

            EventTransmitter.TryGetChannel<T>(this, out var newChannel);
            channel = newChannel;

            return channel != null;
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

        public bool TryRemoveHandlerFromSubscription<TEventArgs>(Action<TEventArgs> handler)
            where TEventArgs : IEventArgs
        {
            bool result = false;

            foreach (var channel in _subscribedChannels)
            {
                if (channel is not IEventChannel<TEventArgs> typedChannel)
                    continue;

                if (!typedChannel.TryUnsubscribe(this, handler))
                    continue;

                result = true;
            }

            return result;
        }

        public void UnsubscribeAll()
        {
            UnsubscribeRequested?.Invoke(this);
            _subscribedChannels.Clear();
        }

        public bool Equals(IEventParticipant other)
        {
            return other != null && EventID == other.EventID;
        }

        public override bool Equals(object obj)
        {
            return obj is Subscriber other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EventID);
        }
    }
}
