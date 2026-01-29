using System;
using UnityEngine;

public abstract class ScoreManager : MonoBehaviour
{
    protected int _currentScore = 0;
    public abstract event Action<int> ScoreUpdated;
}

public abstract class ScoreManager<T> : ScoreManager
{
    protected abstract void IncreaseScore(T data);
}
