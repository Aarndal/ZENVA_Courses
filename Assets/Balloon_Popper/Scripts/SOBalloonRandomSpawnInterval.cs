using UnityEngine;

[CreateAssetMenu(fileName = "newBalloonRandomSpawnInterval", menuName = "BalloonPopper/RandomSpawnInterval", order = 2)]
public class SOBalloonRandomSpawnInterval : SOBalloonSpawnInterval, IRandomSpawnInterval
{
    [SerializeField]
    private float minInterval = 0f;
    [SerializeField]
    private float maxInterval = 1f;


    public float MinInterval => minInterval;
    public float MaxInterval => maxInterval;


    public override float GetNextInterval() =>
        UnityEngine.Random.Range(minInterval, maxInterval);
}