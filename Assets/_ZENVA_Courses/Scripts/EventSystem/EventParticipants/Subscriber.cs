using System;
using System.Collections.Generic;

namespace EventSystem
{
    public class Subscriber : ISubscriber
    {
        // Private Members
        private readonly HashSet<IEventChannel> _subscribedChannels = new();

        // Properties
        public Guid EventGuid { get; private set; }
        public uint EventID { get; private set; }
        public string UniqueKey { get; }

        // Events
        public event Action<ISubscriber> UnsubscribeRequested;

        // Constructor
        public Subscriber(string uniqueKey)
        {
            UniqueKey = uniqueKey;

            EventID = EventSystemIDManager.GetParticipantID(this);
            EventGuid = EventSystemIDManager.GetParticipantGuid(this);
        }


        #region Private Methods
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
            UnsubscribeRequested?.Invoke(this);
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
