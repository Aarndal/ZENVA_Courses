using System;

namespace EventSystem
{
    public interface IEventQueue<TEventArgs>
        where TEventArgs : IEventArgs
    {
        event Action<TEventArgs> EventEnqueued;
        int PendingEventsCount { get; }
        void EnqueueEvent(TEventArgs args);
    }
}