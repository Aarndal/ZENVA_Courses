using System;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager
    {
        public override event Action<int> ScoreUpdated
        {
            add { _scoreUpdated += value; }
            remove { _scoreUpdated -= value; }
        }

        private void OnEnable()
        {
            IScoreChanger.ScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            IScoreChanger.ScoreChanged -= OnScoreChanged;
        }

        void OnScoreChanged(IScoreChanger scoreChanger)
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
