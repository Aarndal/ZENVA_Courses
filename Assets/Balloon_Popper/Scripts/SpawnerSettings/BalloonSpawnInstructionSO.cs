using BalloonPopper;
using UnityEngine;

[CreateAssetMenu(fileName = "newBalloonSpawnInstruction", menuName = "BalloonPopper/SpawnInstructions/BalloonSpawnInstruction")]
public class BalloonSpawnInstructionSO : SpawnInstructionSO<BalloonDataProviderSO>
{
    [SerializeField]
    private BalloonDataProviderSO data = null;
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private SpawnIntervalSO spawnInterval = null;

    public override BalloonDataProviderSO Data => data;
    public override int AmountToSpawn => amount;
    public override ISpawnInterval SpawnInterval => spawnInterval;
}
