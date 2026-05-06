using Debugging;
using EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager, ISubscriber
    {
        // Private Member Variables
        private readonly HashSet<IEventChannel> _subscribedChannels = new();

        private Guid _eventGuid = Guid.Empty;
        private uint _eventID = 0;

        // Serialized Fields
        [SerializeField]
        private string uniqueKey = default;

        public event Action<ISubscriber> UnsubscribeRequested;

        // Properties
        public Guid EventGuid
        {
            get
            {
                if (_eventGuid == Guid.Empty)
                {
                    _eventGuid = EventSystemIDManager.GetParticipantGuid(this);
                }
                return _eventGuid;
            }
        }
        public uint EventID
        {
            get
            {
                if (_eventID == 0)
                {
                    _eventID = EventSystemIDManager.GetParticipantID(this);
                }
                return _eventID;
            }
        }
        public string UniqueKey
        {
            get
            {
                if (string.IsNullOrEmpty(uniqueKey))
                {
                    uniqueKey = this.gameObject.name;
                }
                return uniqueKey;
            }
        }

        // Events
        public override event Action<int> ScoreUpdated
        {
            add { _scoreUpdated += value; }
            remove { _scoreUpdated -= value; }
        }


        #region Unity Lifecycle Methods
        private void Awake()
        {
            if (string.IsNullOrEmpty(uniqueKey))
                uniqueKey = this.gameObject.name;

            _eventGuid = EventSystemIDManager.GetParticipantGuid(this);
            _eventID = EventSystemIDManager.GetParticipantID(this);
        }

        private void OnEnable()
        {
            if (_subscribedChannels == null)
                return;

            if (_subscribedChannels.Count > 0)
            {
                var channel = _subscribedChannels.FirstOrDefault(channel => channel is IEventChannel<ScoreChangedEventArgs>);

                if (channel != default)
                {
                    var typedChannel = channel as IEventChannel<ScoreChangedEventArgs>;
                    typedChannel.TrySubscribe(this, OnScoreChanged, CheckScoreChangedArgs);
                    return;
                }
            }

            if (!EventTransmitter.TryGetChannel(this, out IEventChannel<ScoreChangedEventArgs> scoreChangeChannel))
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Failed to subscribe to event channel for event arguments: {0}" +
                    "\nNo such channel found.",
                    true,
                    typeof(ScoreChangedEventArgs).Name);
                return;
            }

            if (scoreChangeChannel.TrySubscribe(this, OnScoreChanged, CheckScoreChangedArgs))
            {
                _subscribedChannels.Add(scoreChangeChannel);
            }
        }

        private void OnDisable()
        {
            foreach (var channel in _subscribedChannels)
            {
                if (channel is IEventChannel<IEventArgs> eventChannel)
                    eventChannel.TryUnsubscribe(this);
            }
        }

        private void OnDestroy()
        {
            _subscribedChannels.Clear();
        }
        #endregion


        private void OnScoreChanged(ScoreChangedEventArgs args)
        {
            IncreaseScore(args.ScoreChanger.ScoreChangeValue);
        }

        private bool CheckScoreChangedArgs(ScoreChangedEventArgs args)
        {
            return args.ScoreChanger != null && args.AreValid;
        }

        protected override void IncreaseScore(int scoreChangeValue)
        {
            _currentScore += scoreChangeValue;
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }


        //! For SubscriberComponent, if used
        public void ReceiveEvent(IEventArgs eventArgs)
        {
            if (eventArgs is ScoreChangedEventArgs scoreChangedArgs)
            {
                OnScoreChanged(scoreChangedArgs);
            }
        }


        #region IEquatable Implementation
        public bool Equals(IEventParticipant other)
        {
            if (other == null || other is not BalloonPopperScoreManager) return false;

            return EventGuid.Equals(other.EventGuid) && EventID.Equals(other.EventID);
        }

        public override bool Equals(object other)
        {
            return other is BalloonPopperScoreManager otherScoreManager && Equals(otherScoreManager);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EventGuid, EventID);
        }
        #endregion
    }
}
