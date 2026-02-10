using UnityEngine;

[CreateAssetMenu(fileName = "newRandomSpawnInterval", menuName = "BalloonPopper/SpawnInterval/RandomSpawnInterval", order = 2)]
public class RandomSpawnIntervalSO : SpawnIntervalSO, IRandomSpawnInterval
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