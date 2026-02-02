using BalloonPopper;
using UnityEngine;

[CreateAssetMenu(fileName = "newBalloonSpawnInstruction", menuName = "BalloonPopper/SpawnInstructions/BalloonSpawnInstruction")]
public class BalloonSpawnInstructionSO : SpawnInstructionSO<BalloonPopper.SpawnableDataProviderSO>
{
    [SerializeField]
    private BalloonPopper.SpawnableDataProviderSO data = null;
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private SpawnIntervalSO spawnInterval = null;

    public override BalloonPopper.SpawnableDataProviderSO Data => data;
    public override int AmountToSpawn => amount;
    public override ISpawnInterval SpawnInterval => spawnInterval;
}
