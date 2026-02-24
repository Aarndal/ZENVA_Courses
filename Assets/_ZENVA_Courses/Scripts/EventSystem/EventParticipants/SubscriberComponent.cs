using System;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    public class SubscriberComponent : MonoBehaviour, ISubscriber
    {
        public EventChannelSO MyEventChannel;
        public UnityEvent<MyEventArgs> UnityEvent;

        public Guid ID => Guid.Parse(this.GetEntityId().ToString());

        public bool Equals(IEventParticipant other)
        {
            if (other is ISubscriber subscriber)
            {
                return ID == subscriber.ID;
            }
            return false;
        }

        private void Awake()
        {
            for (int i = 0; i < UnityEvent.GetPersistentEventCount(); i++)
            {
                var target = UnityEvent.GetPersistentTarget(i);
                if (target == null)
                    continue;
                if (target is ISubscriber subscriber)
                {
                    if (!subscriber.Equals(this))
                    {
                        Debug.LogWarning($"SubscriberComponent on {gameObject.name} has a UnityEvent listener that does not reference itself. This may lead to unexpected behavior.");
                    }
                }
                else
                {
                    Debug.LogWarning($"SubscriberComponent on {gameObject.name} has a UnityEvent listener that does not implement ISubscriber. This may lead to unexpected behavior.");
                }
            }

        }
    }

    public abstract class MyEventArgs : EventArgs, IEventArgs
    {
        public bool AreValid => throw new NotImplementedException();

        public EventFlag Flag => throw new NotImplementedException();

        public IPublisher Publisher => throw new NotImplementedException();

        public Guid ID => throw new NotImplementedException();

        public bool Equals(IDataProvider other)
        {
            throw new NotImplementedException();
        }
    }
}
