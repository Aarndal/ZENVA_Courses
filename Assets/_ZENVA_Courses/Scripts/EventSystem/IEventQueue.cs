namespace EventSystem
{
    public interface IEventQueue<TEventArgs> : IEventParticipant
        where TEventArgs : IEventArgs
    {
        int PendingEventsCount { get; }

        void EnqueueEvent(TEventArgs args);
    }
}