using Debugging;
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
            if (Count <= 0)
            {
                args = default;
                return false;
            }

            args = _queue.Dequeue();

            if (args == null || !args.AreValid)
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "Dequeued item was null or invalid and has been discarded. EventArgs: {0}",
                    true,
                    args?.ToString() ?? "null");
                args = default;
                return false;
            }

            return true;
        }

        public bool TryEnqueue(TEventArgs args)
        {
            if (args == null) return false;

            if (!args.AreValid)
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "Attempting to enqueue invalid event arguments. Enqueue failed. EventArgs: {0}",
                    true,
                    args.ToString());
                return false;
            }

            _queue.Enqueue(args);
            EventEnqueued?.Invoke(args);
            return true;
        }
    }
}
