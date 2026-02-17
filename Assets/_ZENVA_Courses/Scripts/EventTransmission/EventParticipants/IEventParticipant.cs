namespace EventTransmission
{
    /// <summary>
    /// Marker interface to indicate the object is an event participant.
    /// </summary>
    public interface IEventParticipant
    {
        ulong EventParticipantID { get; }
        string EventParticipantName { get; }
        ushort EventParticipantPriority { get; }
    }

    /// <summary>
    /// Marker interface for default event participants that are used when no explicit participant is provided.
    /// </summary>
    internal interface IDefaultEventParticipant : IEventParticipant { }
}
