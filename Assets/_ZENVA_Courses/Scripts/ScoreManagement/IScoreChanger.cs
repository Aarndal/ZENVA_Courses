using System;


public interface IScoreChanger
{
    static Action<IScoreChanger> ScoreChanged;

    int ScoreChangeValue { get; }
}
