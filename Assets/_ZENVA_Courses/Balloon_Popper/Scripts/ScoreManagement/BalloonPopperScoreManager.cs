using EventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager, ISubscriber
    {
        [SerializeField, HideInInspector]
        private string id = default;

        public HashSet<IEventChannel> SubscribedChannels { get; private set; } = new();

        public Guid ID
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
                SubscribedChannels.Add(scoreChangeChannel);
            }

            //IScoreChanger.ScoreChanged += OnScoreChanged;
        }


        private void OnDisable()
        {
            //IScoreChanger.ScoreChanged -= OnScoreChanged;

            foreach (var channel in SubscribedChannels)
            {
                if (channel is IEventChannel<IEventArgs> eventChannel)
                    eventChannel.TryUnsubscribe(this);
            }
            SubscribedChannels.Clear();
        }

        private void OnScoreChanged(ScoreChangedEventArgs args)
        {
            if (args.ScoreChanger == null)
                return;

            IncreaseScore(args.ScoreChanger.ScoreChangeValue);
        }

        private void OnScoreChanged(IScoreChanger scoreChanger)
        {
            if (scoreChanger == null)
                return;

            IncreaseScore(scoreChanger.ScoreChangeValue);
        }

        protected override void IncreaseScore(int scoreChangeValue)
        {
            _currentScore += scoreChangeValue;
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }
    }
}
