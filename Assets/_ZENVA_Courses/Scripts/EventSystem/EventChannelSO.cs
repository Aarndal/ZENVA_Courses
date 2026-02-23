using Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(fileName = "NewEventChannel", menuName = "Event System/Event Channel")]
    public class EventChannelSO : ScriptableObject, IEventChannel
    {
        private readonly HashSet<IEventChannel> _channels = new();

        private bool _disposed;

        public int SubscriberCount => _channels.Sum(channel => channel.SubscriberCount);

        public event Action<IEventChannel> DisposalRequested;


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _channels.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Subscribe<TEventArgs>(ISubscriber subscriber, Action<TEventArgs> handler, Func<TEventArgs, bool> filter = null)
            where TEventArgs : IEventArgs
        {
            if (subscriber == null || handler == null)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Attempting to subscribe with null subscriber or handler. Subscription failed.",
                    true);
                return;
            }

            var channel = _channels.FirstOrDefault(channel => channel is IEventChannel<TEventArgs>) as IEventChannel<TEventArgs>;

            if (channel == default || channel is null)
            {
                if (!EventTransmitter.TryGetChannel(subscriber, out channel))
                {
                    return;
                }
            }

            if (!channel.TrySubscribe(subscriber, handler, filter))
            {
                return;
            }

            if (!_channels.Add(channel))
            {
                return;
            }
        }

        public void Unsubscribe<TEventArgs>(ISubscriber subscriber)
            where TEventArgs : IEventArgs
        {
            if (subscriber == null)
                return;

            //if (!_channels.TryGetValue(new EventChannel<TEventArgs>(), out var existingChannel))
            //    return;
            //var channel = existingChannel as IEventChannel<TEventArgs>;

            var channel = _channels.FirstOrDefault(channel => channel is IEventChannel<TEventArgs>) as IEventChannel<TEventArgs>;

            if (channel == default || channel is null)
                return;

            if (!channel.TryUnsubscribe(subscriber))
                return;

            if (channel.SubscriberCount == 0)
            {
                _channels.Remove(channel);
            }

            if (_channels.Count == 0)
            {
                DisposalRequested?.Invoke(this);
            }
        }
    }
}