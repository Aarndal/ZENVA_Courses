using EventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager, ISubscriber
    {
        private readonly HashSet<IEventChannel> _subscribedChannels = new();

        [SerializeField, HideInInspector]
        private string id = default;

        public Guid EventGuid
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                    id = Guid.NewGuid().ToString();
                if (Guid.TryParse(id, out var guid))
                    return guid;
                guid = Guid.NewGuid();
                id = guid.ToString();
                return guid;
            }
        }

        public Dictionary<IEventChannel, (bool hasEventQueue, IEventQueue<IEventArgs> eventQueue)> EventQueuePerChannel => new();

        public override event Action<int> ScoreUpdated
        {
            add { _scoreUpdated += value; }
            remove { _scoreUpdated -= value; }
        }


        private void OnEnable()
        {
            if (!EventTransmitter.TryGetChannel(this, out IEventChannel<ScoreChangedEventArgs> scoreChangeChannel))
            {
                Debug.LogError("ScoreValueChangedEventArgs channel not found. Score updates will not be received.");
                return;
            }

            if (scoreChangeChannel.TrySubscribe(this, OnScoreChanged))
            {
                _subscribedChannels.Add(scoreChangeChannel);
                EventQueuePerChannel.TryAdd(scoreChangeChannel, (false, null));
            }

        }


        private void OnDisable()
        {

            foreach (var channel in _subscribedChannels)
            {
                if (channel is IEventChannel<IEventArgs> eventChannel)
                    eventChannel.TryUnsubscribe(this);
            }
            _subscribedChannels.Clear();
        }


        public void ReceiveEvent(IEventArgs eventArgs)
        {
            if (eventArgs is ScoreChangedEventArgs scoreChangedArgs)
            {
                OnScoreChanged(scoreChangedArgs);
            }
        }

        private void OnScoreChanged(ScoreChangedEventArgs args)
        {
            if (args.ScoreChanger == null)
                return;

            IncreaseScore(args.ScoreChanger.ScoreChangeValue);
        }

        protected override void IncreaseScore(int scoreChangeValue)
        {
            _currentScore += scoreChangeValue;
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }

        public bool Equals(IEventParticipant other)
        {
            if (other == null) return false;

            return EventGuid.Equals(other.EventGuid);
        }
    }
}
