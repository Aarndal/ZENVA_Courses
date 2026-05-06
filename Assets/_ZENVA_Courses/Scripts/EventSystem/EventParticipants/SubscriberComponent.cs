using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    public class SubscriberComponent : MonoBehaviour
    {
        [SerializeField] 
        private EventChannelSubscriberSO _eventChannelSubscriber = default;
        [SerializeField] 
        private UnityEvent<MyEventArgs> _classHandler = default;
        [SerializeField] 
        private UnityEvent<MyEventArgsStruct> _structHandler = default;

        private void OnEnable()
        {
            if (_eventChannelSubscriber != null)
            {
                _eventChannelSubscriber.TryAddListener<MyEventArgs>(OnClassEventRaised);
                _eventChannelSubscriber.TryAddListener<MyEventArgsStruct>(OnStructEventRaised);
            }
        }

        private void OnDisable()
        {
            if (_eventChannelSubscriber != null)
            {
                _eventChannelSubscriber.TryRemoveListener<MyEventArgs>(OnClassEventRaised);
                _eventChannelSubscriber.TryRemoveListener<MyEventArgsStruct>(OnStructEventRaised);
            }
        }

        private void OnClassEventRaised(MyEventArgs args)
        {
            _classHandler?.Invoke(args);
        }

        private void OnStructEventRaised(MyEventArgsStruct args)
        {
            _structHandler?.Invoke(args);
        }
    }
}
