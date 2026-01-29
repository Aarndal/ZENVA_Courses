using System;
using UnityEngine;

namespace BalloonPopper
{
    public class BalloonPopperScoreManager : ScoreManager
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

            IncreaseScore();
        }

        protected override void IncreaseScore()
        {
            _currentScore += 10; // Increase score by 10 for each balloon popped
            Debug.Log("Score: " + _currentScore);

            _scoreUpdated?.Invoke(_currentScore);
        }
    }
}
