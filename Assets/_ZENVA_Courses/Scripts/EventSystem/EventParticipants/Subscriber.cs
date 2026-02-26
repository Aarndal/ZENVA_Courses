using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSystem
{
    public class Subscriber : ISubscriber
    {
        // Private Members
        private readonly HashSet<IEventChannel> _subscribedChannels = new();

        // Properties
        public Guid EventGuid { get; }
        public uint EventID { get; }
        public string UniqueKey { get; }

        // Constructor
        public Subscriber(string uniqueKey)
        {
            UniqueKey = uniqueKey;

            EventSystemIDManager.GetParticipantID(this);
            EventSystemIDManager.GetParticipantGuid(this);
        }


        #region Private Methods
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
        #endregion


        #region Public Methods
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
            foreach (var channel in _subscribedChannels)
            {
                if (channel is IEventChannel<IEventArgs> genericChannel)
                {
                    genericChannel.TryUnsubscribe(this);
                }
            }
            _subscribedChannels.Clear();
        }
        #endregion


        #region IEquatable Implementation
        public bool Equals(IEventParticipant other)
        {
            if (other is ISubscriber subscriber)
            {
                return EventGuid == subscriber.EventGuid && EventID == subscriber.EventID;
            }
            return false;
        }
        public override bool Equals(object obj)
        {
            return obj is Subscriber other && Equals(other);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(EventGuid, EventID);
        }
        #endregion
    }
}
