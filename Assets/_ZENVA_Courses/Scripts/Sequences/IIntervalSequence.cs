using System;

/// <summary>
/// Base interface for interval sequences.
/// </summary>
public interface IIntervalSequence
{
    /// <summary>
    /// Raised when the sequence has completed one full cycle.
    /// </summary>
    event Action SequenceCompleted;

    /// <summary>
    /// The total number of intervals until the sequence completes a full cycle. 
    /// For fixed sequences, this is the length of the interval array. 
    /// For random sequences, this can be a predefined count.
    /// If the sequence is infinite, it returns a negative value of -1.
    /// </summary>
    int TotalIntervals { get; }

    /// <summary>
    /// Gets the next interval in the sequence.
    /// </summary>
    float GetNextInterval();

    /// <summary>
    /// Resets the sequence to its initial state.
    /// </summary>
    bool TryResetSequence();
}

/// <summary>
/// Interval sequence with predefined intervals to choose from.
/// </summary>
public interface IPredefinedIntervalSequence : IIntervalSequence
{
    /// <summary>
    /// Current position in the interval array.
    /// </summary>
    int CurrentInterval { get; }

    /// <summary>
    /// The array of predefined interval values to cycle through.
    /// </summary>
    float[] Intervals { get; }
}

/// <summary>
/// Interval sequence with randomized intervals.
/// </summary>
public interface IRandomizedIntervalSequence : IIntervalSequence
{
    float MinInterval { get; }
    float MaxInterval { get; }
}
