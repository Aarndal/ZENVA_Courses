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

        private void OnEnable()
        {
            _subscriber = new(uniqueKey);
        }

        private void OnDestroy()
        {
            _subscriber?.UnsubscribeAll();
        }

        public bool TryAddListener<TEventArgs>(Action<TEventArgs> handler, Predicate<TEventArgs> filter = null)
            where TEventArgs : IEventArgs
        {
            if (_subscriber == null) return false;
            return _subscriber.TryAddHandlerToSubscription(handler, filter);
        }

        public bool TryRemoveListener<TEventArgs>(Action<TEventArgs> handler)
            where TEventArgs : IEventArgs
        {
            if (_subscriber == null) return false;
            return _subscriber.TryRemoveHandlerFromSubscription(handler);
        }
    }
}
