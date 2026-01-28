using System;
using UnityEngine;

namespace BalloonPopper
{

    public class BalloonPopperScoreManager : ScoreManager
    {
        private void OnEnable()
        {
            Balloon.BalloonPopped += OnBalloonPopped;
        }

        private void OnDisable()
        {
            Balloon.BalloonPopped -= OnBalloonPopped;
        }

        void OnBalloonPopped(Balloon balloon, SOBalloonData balloonData)
        {
            IncreaseScore();
        }

        protected override void IncreaseScore()
        {
            _score += 10; // Increase score by 10 for each balloon popped
            Debug.Log("Score: " + _score);
        }
    }
}
