using System;
using UnityEngine;

namespace EventTransmission
{
    public abstract class SubscriberComponent : MonoBehaviour, ISubscriber
    {
        [SerializeField]
        private bool _isEnabled = true;
        [SerializeField]
        private ushort _priority = 0;

        public ulong EventParticipantID { get; private set; }
        public bool IsEnabled => _isEnabled && gameObject.activeInHierarchy;
        public string EventParticipantName => gameObject.name;
        public ushort EventParticipantPriority => _priority;

        protected virtual void Awake()
        {
            EventParticipantID = EventParticipantIDManager.GenerateID(this);
        }

        protected virtual void OnDestroy()
        {
            EventParticipantIDManager.ReleaseID(this);
        }
    }
}
