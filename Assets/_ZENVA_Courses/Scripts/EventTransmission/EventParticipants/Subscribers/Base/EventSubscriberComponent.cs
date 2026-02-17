using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;


//TODO: Rework Publisher checking and avoid setting it on every event call.
//TODO: Make IEvent serializable and use it instead of SOEvent.



namespace EventTransmission
{
    public abstract class EventSubscriberComponent<TEventArgs> : SubscriberComponent
        where TEventArgs : IEventArgs
    {
        // Serialized Fields
        [SerializeField]
        [Tooltip("Event to subscribe to.")]
        private SOEvent<TEventArgs> subscribedEvent = null; //TODO: Make IEvent serializable and use it instead of SOEvent.
        
        [SerializeField] //Warning! Not serialized by Unity!
        [Tooltip("Event will be invoked only if the publisher matches the specified object. If null, any publisher is accepted.")]
        private object observedPublisher = null; //TODO: Rework Publisher checking and avoid setting it on every event call.


        // Private Members
        private bool _isSubscribed = false;


        // Events
        public UnityEvent<object, TEventArgs, CancellationToken> OnEventRaised;


        // Unity LifeCycle Methods
        protected virtual void OnEnable()
        {
            _isSubscribed = SubscribeToEvent();

            if (!_isSubscribed)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Couldn't subscribe {0}. Subscribed event has an unsupported type: {1}", this.name, subscribedEvent.name);
#endif
            }
        }

        protected virtual void OnDisable()
        {
            if (!_isSubscribed)
                return;

            _isSubscribed = !UnsubscribeFromEvent();

            if (_isSubscribed)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Couldn't unsubscribe {0} from subscribed event: {1}", this.name, subscribedEvent.name);
#endif
            }
        }

        protected override void OnDestroy()
        {
            OnEventRaised = null;
            base.OnDestroy();
        }


        // Callbacks
        private void OnSyncEventRaised(object publisher, TEventArgs eventArgs)
        {
            if (OnEventRaised == null || subscribedEvent == null)
                return;

            if (observedPublisher != null)
            {
                // If you use IPublisher interface:
                if (observedPublisher is IPublisher expected && publisher is IPublisher actual)
                {
                    if (expected.EventParticipantID != actual.EventParticipantID)
                        return;
                }
                else
                {
                    // Fallback to reference equality
                    if (publisher != observedPublisher)
                        return;
                }
            }

            OnEventRaised.Invoke(publisher, eventArgs, CancellationToken.None);
        }

        private UniTask OnAsyncEventRaised(object publisher, TEventArgs eventArgs, CancellationToken externalToken)
        {
            if (OnEventRaised == null || subscribedEvent == null)
                return UniTask.CompletedTask;

            if (observedPublisher != null)
            {
                if (observedPublisher is IPublisher expected && publisher is IPublisher actual)
                {
                    if (expected.EventParticipantID != actual.EventParticipantID)
                        return UniTask.CompletedTask;
                }
                else
                {
                    if (publisher != observedPublisher)
                        return UniTask.CompletedTask;
                }
            }

            OnEventRaised?.Invoke(publisher, eventArgs, externalToken);
            return UniTask.CompletedTask;
        }


        // Private Methods
        private bool SubscribeToEvent()
        {
            if (subscribedEvent == null)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Subscribed event is null on {0}!", this.name);
#endif
                return false;
            }

            if (subscribedEvent is ISyncEvent<TEventArgs> syncEvent)
            {
                syncEvent.EventRaised += OnSyncEventRaised;
                return true;
            }

            if (subscribedEvent is IAsyncEvent<TEventArgs> asyncEvent)
            {
                asyncEvent.AsyncEventRaised += OnAsyncEventRaised;
                return true;
            }

            return false;
        }

        private bool UnsubscribeFromEvent()
        {
            if (subscribedEvent is ISyncEvent<TEventArgs> syncEvent)
            {
                syncEvent.EventRaised -= OnSyncEventRaised;
                return true;
            }

            if (subscribedEvent is IAsyncEvent<TEventArgs> asyncEvent)
            {
                asyncEvent.AsyncEventRaised -= OnAsyncEventRaised;
                return true;
            }

            return false;
        }
    }
}