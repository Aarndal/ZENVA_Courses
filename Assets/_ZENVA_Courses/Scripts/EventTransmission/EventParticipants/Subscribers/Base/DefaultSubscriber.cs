
namespace EventTransmission
{
    /// <summary>
    /// A global singleton subscriber used when no explicit subscriber is provided.
    /// </summary>
    [EventParticipantType(EventParticipantIDManager.ParticipantType.Subscriber)]
    internal sealed class DefaultSubscriber : ISubscriber, IDefaultEventParticipant
    {
        public static DefaultSubscriber Instance { get; } = new();

        public ulong EventParticipantID { get; private set; }
        public bool IsEnabled => true;
        public string EventParticipantName => "Default Subscriber";
        public ushort EventParticipantPriority => 0;


        private DefaultSubscriber() { EventParticipantID = EventParticipantIDManager.GenerateDefaultParticipantID(this); }
    }
}