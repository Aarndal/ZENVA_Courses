
namespace EventTransmission
{
    /// <summary>
    /// A global singleton async event used when no explicit event is provided.
    /// </summary>
    [EventParticipantType(EventParticipantIDManager.ParticipantType.Event)]
    internal sealed class DefaultAsyncEvent : IEvent, IDefaultEventParticipant
    {
        public static DefaultAsyncEvent Instance { get; } = new();

        public ulong EventParticipantID { get; private set; }
        public bool IsAsync => true;
        public string EventParticipantName => "Default AsyncEvent";
        public ushort EventParticipantPriority => 0;


        private DefaultAsyncEvent() { EventParticipantID = EventParticipantIDManager.GenerateDefaultParticipantID(this); }
    }
}
