using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newFixedSpawnInterval", menuName = "BalloonPopper/SpawnInterval/FixedSpawnInterval", order = 1)]
public class FixedSpawnIntervalSO : SpawnIntervalSO, IFixedSpawnInterval
{
    private const float DEFAULT_INTERVAL = 1f;


    [SerializeField]
    private float[] fixedIntervals;

    private int currentIndex = 0;


    public float[] FixedIntervals => fixedIntervals;


    public event Action ReachedEndOfIntervals;


    public override float GetNextInterval()
    {
        if (fixedIntervals == null || fixedIntervals.Length == 0)
            return DEFAULT_INTERVAL;

        var nextInterval = fixedIntervals[currentIndex % fixedIntervals.Length];

        if (currentIndex >= fixedIntervals.Length - 1)
        {
            ReachedEndOfIntervals?.Invoke();
            currentIndex = 0;
        }
        else
        {
            currentIndex++;
        }

        return nextInterval;
    }
}
