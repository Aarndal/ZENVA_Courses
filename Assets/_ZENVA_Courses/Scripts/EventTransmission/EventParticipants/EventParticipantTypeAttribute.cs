using System;

namespace EventTransmission
{
    /// <summary>
    /// Attribute to specify the participant type of an event participant class.
    /// Necessary for the EventParticipantIDManager to correctly manage IDs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class EventParticipantTypeAttribute : Attribute
    {
        public EventParticipantIDManager.ParticipantType Type { get; }

        public EventParticipantTypeAttribute(EventParticipantIDManager.ParticipantType type)
        {
            Type = type;
        }
    }
}
