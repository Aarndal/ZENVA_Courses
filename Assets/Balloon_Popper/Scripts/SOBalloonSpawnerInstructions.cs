using BalloonPopper;
using UnityEngine;

[CreateAssetMenu(fileName = "newBalloonSpawnerInstructions", menuName = "BalloonPopper/SpawnerInstructions", order = 3)]
public class SOBalloonSpawnerInstructions : ScriptableObject, ISpawnerInstruction<SOBalloonData>
{
    [SerializeField]
    private SOBalloonData data = null;
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private SOBalloonSpawnInterval spawnInterval = null;


    public SOBalloonData Data => data;
    public int Amount => amount;
    public ISpawnInterval SpawnInterval => spawnInterval;
}
