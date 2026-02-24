using System;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem
{
    public class SubscriberComponent : MonoBehaviour
    {
        public EventChannelSubscriberSO EventChannelSubscriber = default;
        public UnityEvent<MyEventArgs> ClassHandler = default;
        public UnityEvent<MyEventArgsStruct> StructHandler = default;

        private void OnEnable()
        {
            if (EventChannelSubscriber != null)
                EventChannelSubscriber.TryAddListener(OnEventRaised);
        }
        
        private void OnEventRaised(IEventArgs args)
        {
            if (args is MyEventArgs myArgs)
            {
                ClassHandler?.Invoke(myArgs);
            }
            else if (args is MyEventArgsStruct myArgsStruct)
            {
                StructHandler?.Invoke(myArgsStruct);
            }
        }
    }
}
