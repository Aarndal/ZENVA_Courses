using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BalloonPopper
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField]
        private Spawner[] spawners;

        [SerializeField]
        private SpawnerSettingsSO<SpawnableDataProviderSO> spawnerSettings;

        private void Awake()
        {
            if (spawners.Length < spawnerSettings.NumberOfSpawners)
            {
                AddSpawner(spawnerSettings.NumberOfSpawners - spawners.Length);
            }

            if (!TryConfigureSpawners())
            {
                Debug.LogError("Failed to configure spawners.");
            }
        }
        private void AddSpawner(int value)
        {
            throw new NotImplementedException();
        }

        private bool TryConfigureSpawners()
        {
            for (int i = 0; i < spawnerSettings.NumberOfSpawners; i++)
            {
                if(!spawnerSettings.SpawnerSettings.TryGetValue(i, out List<SpawnInstructionSO<SpawnableDataProviderSO>> instructions))
                {
                    Debug.LogError($"No spawn instructions found for spawner index {i}.");
                    break;
                }

                //! Currently, we cannot directly use List<SpawnInstructionSO<SpawnableDataProviderSO>> where List<ISpawnInstruction<SpawnableDataProviderSO>> is expected.
                List<ISpawnInstruction<SpawnableDataProviderSO>> castInstructions = instructions.Cast<ISpawnInstruction<SpawnableDataProviderSO>>().ToList();

                if (!spawners[i].TrySetInstructions(castInstructions))
                {
                    Debug.LogError($"Failed to set instructions for spawner index {i}.");
                    break;
                }
            }

            return true;
        }
    }
}
