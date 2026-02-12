using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newRandomizedIntervalSequence", menuName = "Sequences/Randomized Interval Sequence", order = 2)]
public class RandomizedIntervalSequenceSO : IntervalSequenceSO, IRandomizedIntervalSequence
{
    [SerializeField, Min(1)]
    private int totalIntervals = 1;
    [SerializeField]
    private float minInterval = 0f;
    [SerializeField]
    private float maxInterval = 1f;
    
    public float MinInterval => minInterval;
    public float MaxInterval => maxInterval;
    public override int TotalIntervals => totalIntervals;

    public override float GetNextInterval()
    {
        if(!TryResetSequence())
        {
            _currentInterval++;
        }

        return UnityEngine.Random.Range(minInterval, maxInterval);
    }
    
}