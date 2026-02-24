using Debugging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    /// <summary>
    /// This ScriptableObject serves as a container for references to multiple event channels. 
    /// It allows Designers to subscribe to events without needing direct references to the individual channels.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEventChannel", menuName = "Event System/Event Channel")]
    public class EventChannelSubscriberSO : ScriptableObject, ISubscriber
    {
        private Subscriber _subscriber = default;

        [HideInInspector, SerializeField]
        private string id = default;

        public Guid ID
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                }
                return Guid.Parse(id);
            }
        }

        private void Awake()
        {
            _subscriber = new(ID);
        }

        private void OnDestroy()
        {
            _subscriber.UnsubscribeAll();
        }

        public bool Equals(IEventParticipant other)
        {
            if (other is ISubscriber subscriber)
            {
                return ID.Equals(subscriber.ID);
            }
            return false;
        }

        public bool TryAddListener(Action<IEventArgs> handler, Predicate<IEventArgs> filter = null)
        {
            return _subscriber.TryAddHandlerToSubscription(handler, filter);
        }
    }
}