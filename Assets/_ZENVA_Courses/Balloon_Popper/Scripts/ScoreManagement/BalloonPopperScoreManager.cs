using Debugging;
using EventSystem;
using System;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager
    {
        [SerializeField]
        private string uniqueKey = default;

        private Subscriber _subscriber;

        public override event Action<int> ScoreUpdated
        {
            add { _scoreUpdated += value; }
            remove { _scoreUpdated -= value; }
        }

        private void Awake()
        {
            if (string.IsNullOrEmpty(uniqueKey))
                uniqueKey = gameObject.name;

            _subscriber = new Subscriber(uniqueKey);
        }

        private void OnEnable()
        {
            if (_subscriber == null)
            {
                DebugLogger.Log(
                    LogMessageType.Error,
                    this,
                    "Subscriber is not initialized. Subscription failed.",
                    true);
                return;
            }

            _subscriber.TryAddHandlerToSubscription<ScoreChangedEventArgs>(OnScoreChanged, IsValidScoreChange);
        }

        private void OnDisable()
        {
            _subscriber?.UnsubscribeAll();
        }

        private void OnDestroy()
        {
            _subscriber?.UnsubscribeAll();
        }

        private void OnScoreChanged(ScoreChangedEventArgs args)
        {
            IncreaseScore(args.ScoreChanger.ScoreChangeValue);
        }

        private bool IsValidScoreChange(ScoreChangedEventArgs args)
        {
            return args.AreValid;
        }

        protected override void IncreaseScore(int scoreChangeValue)
        {
            _currentScore += scoreChangeValue;
            _scoreUpdated?.Invoke(_currentScore);
        }
    }
}
