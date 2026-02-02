using System;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager<BalloonDataProviderSO>
    {
        public override event Action<int> ScoreUpdated
        {
            add { _scoreUpdated += value; }
            remove { _scoreUpdated -= value; }
        }

        private void OnEnable()
        {
            Balloon.BalloonPopped += OnBalloonPopped;
        }

        private void OnDisable()
        {
            Balloon.BalloonPopped -= OnBalloonPopped;
        }

        void OnBalloonPopped(Balloon balloon, BalloonDataProviderSO balloonData)
        {
            if (balloon == null || balloonData == null)
                return;

            IncreaseScore(balloonData);
        }

        protected override void IncreaseScore(BalloonDataProviderSO data)
        {
            _currentScore += data.ScoreValue;
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }
    }
}
