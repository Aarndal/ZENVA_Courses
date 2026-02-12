using System;
using UnityEngine;

public abstract class IntervalSequenceSO : ScriptableObject, IIntervalSequence
{
    protected const int InfiniteSequence = -1;

    protected int _currentInterval = 0;

    public abstract int TotalIntervals { get; }

    public event Action SequenceCompleted;

    private void Awake()
    {
        _currentInterval = 0;
    }

    public abstract float GetNextInterval();
    public virtual bool TryResetSequence()
    {
        if (_currentInterval >= TotalIntervals)
        {
            SequenceCompleted?.Invoke();
            _currentInterval = 0;
            return true;
        }

        return false;
    }
}
