namespace EventTransmission
{
    public interface IPublisher : IEventParticipant
    {
        bool IsActive { get; }
    }
}
