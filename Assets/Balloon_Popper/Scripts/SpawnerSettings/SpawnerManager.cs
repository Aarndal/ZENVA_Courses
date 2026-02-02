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
        private SpawnerSettingsSO<IDataProvider<ISpawnable>> spawnerSettings;

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
                if(!spawnerSettings.SpawnerSettings.TryGetValue(i, out List<ISpawnInstruction<IDataProvider<ISpawnable>>> instructions))
                {
                    Debug.LogError($"No spawn instructions found for spawner index {i}.");
                    break;
                }

                List<ISpawnInstruction<IDataProvider<ISpawnable>>> castInstructions = instructions.Cast<ISpawnInstruction<IDataProvider<ISpawnable>>>().ToList();

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
