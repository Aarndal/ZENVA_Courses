using System;

public interface IFixedSpawnInterval : ISpawnInterval
{
    float[] FixedIntervals { get; }
    
    event Action ReachedEndOfIntervals;
}
