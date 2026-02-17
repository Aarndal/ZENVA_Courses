
namespace EventTransmission
{
    /// <summary>
    /// A global singleton publisher used when no explicit publisher is provided.
    /// </summary>
    [EventParticipantType(EventParticipantIDManager.ParticipantType.Publisher)]
    internal sealed class DefaultPublisher : IPublisher, IDefaultEventParticipant
    {
        public static DefaultPublisher Instance { get; } = new();

        public ulong EventParticipantID { get; private set; }
        public bool IsActive => true;
        public string EventParticipantName => "Default Publisher";
        public ushort EventParticipantPriority => 0;


        private DefaultPublisher() { EventParticipantID = EventParticipantIDManager.GenerateDefaultParticipantID(this); }
    }
}