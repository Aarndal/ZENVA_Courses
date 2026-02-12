using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newPredefinedIntervalSequence", menuName = "Sequences/Predefined Interval Sequence", order = 1)]
public class PredefinedIntervalSequenceSO : IntervalSequenceSO, IPredefinedIntervalSequence
{
    private const float _DefaultInterval = 1f;

    [SerializeField]
    private float[] intervals;

    public int CurrentInterval => _currentInterval;
    public float[] Intervals => intervals;
    public override int TotalIntervals => intervals.Length;


    public override float GetNextInterval()
    {
        if (intervals == null || intervals.Length == 0)
            return _DefaultInterval;

        if (!TryResetSequence())
        {
            _currentInterval++;
        }

        return intervals[_currentInterval % intervals.Length];
    }
    
}
