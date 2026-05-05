using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventSystem
{
    /// <summary>
    /// The EventTransmitter is responsible to manage IEventChannels.
    /// IEventParticipants can request a reference to an IEventChannel through the EventTransmitter.
    /// If the EventTransmitter doesn't have a reference to the requested IEventChannel, it will create a new IEventChannel through its EventChannelFactory, if the requester is an ISubscriber, otherwise it will return null.
    /// </summary>
    public static class EventTransmitter
    {
        // Private Members
        private static readonly EventChannelFactory _channelFactory = new();
        private static readonly Dictionary<Type, IEventChannel> _channels = new();
        private static readonly Dictionary<IEventChannel, Type> _channelTypes = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        // Callback Functions
        private static void OnSceneUnloaded(Scene scene)
        {
            Reset();
        }

        private static void OnChannelDisposalRequested(IEventChannel channel)
        {
            if (channel == null || channel.SubscriberCount > 0) return;

            channel.DisposalRequested -= OnChannelDisposalRequested;

            if (_channelTypes.TryGetValue(channel, out var key))
            {
                _channels.Remove(key);
                _channelTypes.Remove(channel);
            }

            channel.Dispose();
        }

        #region Public Methods
        /// <summary>
        /// Attempts to retrieve an <see cref="IEventChannel"/> for the specified <see cref="IEventArgs"/> type. 
        /// If a channel already exists, returns it; 
        /// otherwise, if the requester is an <see cref="ISubscriber"/>, creates a new channel using <see cref="EventChannelFactory"/>.
        /// The newly created channel is registered and a disposal handler is attached.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of event arguments for the channel.</typeparam>
        /// <param name="requester">The participant requesting the channel.</param>
        /// <param name="channel">When this method returns, contains the channel instance if successful; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if a channel was successfully retrieved or created; otherwise, <c>false</c>.</returns>
        public static bool TryGetChannel<TEventArgs>(IEventParticipant requester, out IEventChannel<TEventArgs> channel)
            where TEventArgs : IEventArgs
        {
            channel = null;

            if (_channels.TryGetValue(typeof(TEventArgs), out var existingChannel))
            {
                channel = existingChannel as IEventChannel<TEventArgs>;
                return channel != null;
            }

            if (requester is not ISubscriber)
            {
                return false;
            }

            if (!_channelFactory.TryCreate(out channel))
            {
                return false;
            }

            if (!_channels.TryAdd(typeof(TEventArgs), channel))
            {
                return false;
            }

            _channelTypes[channel] = typeof(TEventArgs);
            channel.DisposalRequested += OnChannelDisposalRequested;
            return true;
        }

        /// <summary>
        /// Disposes all active channels and clears internal state.
        /// Called automatically on scene unload to prevent stale references across scenes.
        /// </summary>
        public static void Reset()
        {
            foreach (var channel in _channels.Values)
            {
                channel.DisposalRequested -= OnChannelDisposalRequested;
                channel.Dispose();
            }

            _channels.Clear();
            _channelTypes.Clear();
        }
        #endregion
    }
}
