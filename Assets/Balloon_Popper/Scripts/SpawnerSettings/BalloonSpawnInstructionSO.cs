using BalloonPopper;
using UnityEngine;

[CreateAssetMenu(fileName = "newBalloonSpawnInstruction", menuName = "BalloonPopper/SpawnInstructions/BalloonSpawnInstruction")]
public class BalloonSpawnInstructionSO : SpawnInstructionSO<BalloonDataSO>
{
    [SerializeField]
    private BalloonDataSO data = null;
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private SpawnIntervalSO spawnInterval = null;

    public override BalloonDataSO Data => data;
    public override int Amount => amount;
    public override ISpawnInterval SpawnInterval => spawnInterval;
}
