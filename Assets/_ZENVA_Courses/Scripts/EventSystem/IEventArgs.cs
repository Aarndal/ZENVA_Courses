namespace EventSystem
{
    /// <summary>
    /// IEventArgs are used to create custom event arguments that can be passed through the corresponding IEventChannel when an event is triggered.
    /// They are published by IPublishers and received by ISubscribers.
    /// </summary>
    public interface IEventArgs : IDataProvider
    {
        EventFlag Flag { get; }
        IPublisher Publisher { get; }
    }
}