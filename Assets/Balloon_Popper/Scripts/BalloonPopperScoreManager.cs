using System;
using UnityEngine;

namespace BalloonPopper
{
    public sealed class BalloonPopperScoreManager : ScoreManager<SOBalloonData>
    {
        public override event Action<int> ScoreUpdated
        {
            add { _scoreUpdated += value; }
            remove { _scoreUpdated -= value; }
        }

        private Action<int> _scoreUpdated;


        private void OnEnable()
        {
            Balloon.BalloonPopped += OnBalloonPopped;
        }

        private void Start()
        {
            _scoreUpdated?.Invoke(_currentScore);
        }

        private void OnDisable()
        {
            Balloon.BalloonPopped -= OnBalloonPopped;
        }

        void OnBalloonPopped(Balloon balloon, SOBalloonData balloonData)
        {
            if (balloon == null || balloonData == null)
                return;

            IncreaseScore(balloonData);
        }

        protected override void IncreaseScore(SOBalloonData data)
        {
            _currentScore += data.ScoreValue; // Increase score by 10 for each balloon popped
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }
    }
}
