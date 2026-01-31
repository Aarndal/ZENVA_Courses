using System;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager<BalloonDataSO>
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

        void OnBalloonPopped(Balloon balloon, BalloonDataSO balloonData)
        {
            if (balloon == null || balloonData == null)
                return;

            IncreaseScore(balloonData);
        }

        protected override void IncreaseScore(BalloonDataSO data)
        {
            _currentScore += data.ScoreValue;
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }
    }
}
