using System;
using UnityEngine;

namespace EventSystem
{
    /// <summary>
    /// This ScriptableObject serves as a container for references to multiple event channels. 
    /// It allows Designers to subscribe to events without needing direct references to the individual channels.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEventChannel", menuName = "Event System/Event Channel")]
    public class EventChannelSubscriberSO : ScriptableObject
    {
        private Subscriber _subscriber = default;

        [SerializeField]
        private string uniqueKey = default;

        private void Awake()
        {
            _subscriber = new(uniqueKey);
        }

        private void OnDestroy()
        {
            _subscriber.UnsubscribeAll();
        }

        public bool TryAddListener(Action<IEventArgs> handler, Predicate<IEventArgs> filter = null)
        {
            return _subscriber.TryAddHandlerToSubscription(handler, filter);
        }

        public bool TryRemoveListener(Action<IEventArgs> handler)
        {
            return _subscriber.TryRemoveHandlerFromSubscription(handler);
        }
    }
}