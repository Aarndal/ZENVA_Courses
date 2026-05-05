namespace EventSystem
{
    /// <summary>
    /// An EventChannelFactory is responsible to create IEventChannel instances when requested by the EventTransmitter.
    /// </summary>
    public class EventChannelFactory
    {
        public bool TryCreate<TEventArgs>(out IEventChannel<TEventArgs> channel)
            where TEventArgs : IEventArgs
        {
            channel = new EventChannel<TEventArgs>();
            return true;
        }
    }
}
