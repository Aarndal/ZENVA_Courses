namespace EventTransmission
{
    /// <summary>
    /// Marker interface to indicate the participant is a subscriber.
    /// Subscribers can subscribe to events and receive event notifications.
    /// Subscribers can be enabled or disabled, controlling whether they receive events.
    /// </summary>
    public interface ISubscriber : IEventParticipant
    {
        /// <summary>
        /// Provides the enabled/disabled state of the subscriber.
        /// If Disabled, the subscriber will not receive events.
        /// </summary>
        bool IsEnabled { get; }
    }

    /// <summary>
    /// Interface for subscribers that can subscribe to specific event types.
    /// Provides methods to subscribe to and unsubscribe from IEvent of type TEventArgs.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public interface ISubscriber<TEventArgs> : ISubscriber
        where TEventArgs : IEventArgs
    {
        bool Subscribe(IEvent<TEventArgs> @event);
        bool Unsubscribe(IEvent<TEventArgs> @event);
    }
}
