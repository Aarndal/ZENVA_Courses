using System;

namespace EventTransmission
{
    internal abstract class Subscriber : ISubscriber
    {
        public ulong EventParticipantID { get; private set; }
        public abstract bool IsEnabled { get; }
        public abstract string EventParticipantName { get; }
        public abstract ushort EventParticipantPriority { get; }


        public Subscriber()
        {
            EventParticipantID = EventParticipantIDManager.GenerateID(this);
        }

        ~Subscriber()
        {
            EventParticipantIDManager.ReleaseID(this);
        }
    }
}