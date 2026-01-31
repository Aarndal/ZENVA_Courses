using BalloonPopper;
using UnityEngine;

[CreateAssetMenu(fileName = "newBalloonSpawnerInstructions", menuName = "BalloonPopper/SpawnerInstructions", order = 3)]
public class SOBalloonSpawnerInstructions : ScriptableObject, ISpawnerInstruction<BalloonDataSO>
{
    [SerializeField]
    private BalloonDataSO data = null;
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private SpawnIntervalSO spawnInterval = null;


    public BalloonDataSO Data => data;
    public int Amount => amount;
    public ISpawnInterval SpawnInterval => spawnInterval;
}
