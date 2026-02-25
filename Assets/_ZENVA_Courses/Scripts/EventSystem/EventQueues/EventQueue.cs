using System;
using System.Collections.Generic;

namespace EventSystem
{
    public class EventQueue<TEventArgs> : IEventQueue<TEventArgs>
        where TEventArgs : IEventArgs
    {
        private readonly Queue<TEventArgs> _queue = new();

        public int Count => _queue.Count;
        
        public event Action<TEventArgs> EventEnqueued;


        public bool TryDequeue(out TEventArgs args)
        {
            if(Count <= 0)
            {
                args = default;
                return false;
            }
            args = _queue.Dequeue();

            if(args == null) return false;

            return args.AreValid;
        }
        public bool TryEnqueue(TEventArgs args)
        {
            if(args == null) return false;

            _queue.Enqueue(args);
            EventEnqueued?.Invoke(args);
            return true;
        }
    }
}