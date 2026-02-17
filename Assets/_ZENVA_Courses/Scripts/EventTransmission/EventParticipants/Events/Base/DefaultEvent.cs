
namespace EventTransmission
{
    /// <summary>
    /// A global singleton event used when no explicit event is provided.
    /// </summary>
    [EventParticipantType(EventParticipantIDManager.ParticipantType.Event)]
    internal sealed class DefaultEvent : IEvent, IDefaultEventParticipant
    {
        public static DefaultEvent Instance { get; } = new();

        public ulong EventParticipantID { get; private set; }
        public bool IsAsync => false;
        public string EventParticipantName => "Default Event";
        public ushort EventParticipantPriority => 0;


        private DefaultEvent() { EventParticipantID = EventParticipantIDManager.GenerateDefaultParticipantID(this); }
    }
}
