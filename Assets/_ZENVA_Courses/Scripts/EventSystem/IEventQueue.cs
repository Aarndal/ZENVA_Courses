using System;

namespace EventSystem
{
    public interface IEventQueue<TEventArgs>
        where TEventArgs : IEventArgs
    {
        event Action<TEventArgs> EventEnqueued;

        int Count { get; }
        bool TryDequeue(out TEventArgs args);
        bool TryEnqueue(TEventArgs args);
    }
}