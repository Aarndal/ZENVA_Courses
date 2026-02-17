using System;
using System.Linq;
using UnityEngine;


//TODO: Consider adding a list of tags or categories to filter events in the future.
//TODO: Rework Publisher assignment to avoid setting it on every publish call.


namespace EventTransmission
{
    [Serializable]
    public abstract class SOEvent<TEventArgs> : ScriptableObject, IEvent<TEventArgs>
        where TEventArgs : IEventArgs
    {
        // Serialized Fields
        [SerializeField]
        [Tooltip("Priority of the event. Higher priority events are processed first.")]
        private ushort _priority = 0;

        //NOTE: Unity only serializes types that are marked with [Serializable] and supported by Unity's serializer!
        //If TEventArgs is not a Unity-serialized type, the field will not show up in Inspector.
        [SerializeField]
        [Tooltip("Default event arguments used when raising the event without specific args.")]
        private TEventArgs _defaultEventArgs;


        // Private Members
        protected IPublisher _publisher;


        // Properties
        public TEventArgs DefaultEventArgs => _defaultEventArgs;
        public Type EventArgsType => typeof(TEventArgs);
        public bool IsAsync => this is IAsyncEvent<TEventArgs>;
        public IPublisher Publisher
        {
            get => _publisher ?? DefaultPublisher.Instance;
            set => _publisher = value;
        }
        [field: SerializeField, HideInInspector]
        public ulong EventParticipantID { get; private set; }
        public string EventParticipantName => this.name;
        public ushort EventParticipantPriority => _priority;


        // Unity LifeCycle Methods
        protected virtual void Awake()
        {
            EventParticipantID = EventParticipantIDManager.GenerateID(this);
        }


        // Public Methods
        public bool Raise(TEventArgs args, IPublisher publisher = null)
        {
            Publisher = publisher;
            return GlobalEventTransmitter.Publish(args, Publisher);
        }

        //NOTE: Helper for publishing default args.
        public bool RaiseDefault(IPublisher publisher = null)
        {
            return Raise(DefaultEventArgs, publisher);
        }
    }
}