using System;

namespace BalloonPopper
{
    public interface IScoreChanger
    {
        static Action<IScoreChanger> ScoreChanged;

        int ScoreChangeValue { get; }
    }
}
