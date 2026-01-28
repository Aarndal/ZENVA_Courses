using UnityEngine;

public abstract class ScoreManager : MonoBehaviour
{
    protected int _score = 0;

    protected abstract void IncreaseScore();
}
