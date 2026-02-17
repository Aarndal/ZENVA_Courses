using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace EventTransmission
{
    public abstract class SOAsyncEvent<TEventArgs> : SOEvent<TEventArgs>, IAsyncEvent<TEventArgs>
        where TEventArgs : IEventArgs
    {
        // Events
        public event AsyncEventHandler<TEventArgs> AsyncEventRaised // Event to manage subscriptions. No invoke here.
        {
            add
            {
                if (value.Target is ISubscriber subscriber)
                    GlobalEventTransmitter.Subscribe<TEventArgs, AsyncEventHandler<TEventArgs>>(value, subscriber);
                else
                    GlobalEventTransmitter.Subscribe<TEventArgs, AsyncEventHandler<TEventArgs>>(value);
            }
            remove
            {
                if (value.Target is ISubscriber subscriber)
                    GlobalEventTransmitter.Unsubscribe<TEventArgs, AsyncEventHandler<TEventArgs>>(value, subscriber);
                else
                    GlobalEventTransmitter.Unsubscribe<TEventArgs, AsyncEventHandler<TEventArgs>>(value);
            }
        }


        // Public Methods
        public async UniTask<bool> RaiseAsync(bool publishParallel, TEventArgs args, IPublisher publisher = null, CancellationToken externalToken = default)
        {
            publisher ??= DefaultPublisher.Instance;

            using var cts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, externalToken);
            var token = linkedCts.Token;

            try
            {
                token.ThrowIfCancellationRequested();

                if (!publishParallel)
                    return await GlobalEventTransmitter.PublishAsync(args, publisher, token);

                return await GlobalEventTransmitter.PublishParallel(args, publisher, token);
            }
            catch (System.OperationCanceledException)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Event publishing was cancelled: {0}", name);
#endif
            }
            catch (System.ObjectDisposedException)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Event publishing was cancelled (object disposed): {0}", name);
#endif
            }

            return false;
        }
    }
}
