using BalloonPopper;
using UnityEngine;

[CreateAssetMenu(fileName = "newSpawnInstruction", menuName = "BalloonPopper/SpawnInstructions/SpawnInstruction")]
public class BalloonSpawnInstructionSO : SpawnInstructionSO
{
    [SerializeField]
    private SpawnableDataProviderSO data = null;
    [SerializeField]
    private int amount = 0;
    [SerializeField]
    private SpawnIntervalSO spawnInterval = null;

    public override IDataProvider<ISpawnable> Data => data;
    public override int AmountToSpawn => amount;
    public override ISpawnInterval SpawnInterval => spawnInterval;
}
