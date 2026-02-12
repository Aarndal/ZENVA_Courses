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
                Debug.LogErrorFormat("Not enough Spawners assigned in SpawnerManager: {0} | ID: {1}" +
                    "\nRequired: {2} / Assigned: {3}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId(),
                    spawnerSettings.NumberOfSpawners,
                    spawners.Length);
            }

            if (!TryConfigureSpawners())
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Failed to configure Spawners via SpawnerManager: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
#endif
            }
        }
        

        private bool TryConfigureSpawners()
        {
            for (int i = 0; i < spawnerSettings.NumberOfSpawners; i++)
            {
                if(!spawnerSettings.Instructions.TryGetValue(i, out List<ISpawnerInstruction<IDataProvider<ISpawnable>>> instructions))
                {
                    Debug.LogErrorFormat("No SpawnInstructions found for spawner index {0}.", i);
                    break;
                }

                List<ISpawnerInstruction<IDataProvider<ISpawnable>>> castInstructions = instructions.Cast<ISpawnerInstruction<IDataProvider<ISpawnable>>>().ToList();

                var spawnerIndex = i % spawners.Length;

                if (!spawners[spawnerIndex].TrySetInstructions(castInstructions))
                {
                    Debug.LogErrorFormat("Failed to set SpawnInstructions for spawner index {0}.", spawnerIndex);
                    break;
                }
            }

            return true;
        }
    }
}
