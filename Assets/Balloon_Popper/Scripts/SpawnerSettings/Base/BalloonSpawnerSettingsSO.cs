using Project.Tools.DictionaryHelp;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    /// <summary>
    /// ScriptableObject that defines the spawner settings for balloon levels.
    /// </summary>
    [CreateAssetMenu(fileName = "newBalloonSpawnerSettings", menuName = "BalloonPopper/SpawnerSettings/BalloonSpawnerSettings")]
    public class BalloonSpawnerSettingsSO : SpawnerSettingsSO<SpawnableDataProviderSO>
    {
        [SerializeField, Min(1)]
        private int numberOfSpawners = 1;
        [SerializeField]
        private SerializableDictionary<int, List<SpawnInstructionSO<SpawnableDataProviderSO>>> spawnerSettings = new();

        private int _cachedNumberOfSpawners = 0;

        public override int NumberOfSpawners => numberOfSpawners;
        public override SerializableDictionary<int, List<SpawnInstructionSO<SpawnableDataProviderSO>>> SpawnerSettings => spawnerSettings;

        private void OnValidate()
        {
            if (_cachedNumberOfSpawners == numberOfSpawners && spawnerSettings.Count == numberOfSpawners) return;

            if(_cachedNumberOfSpawners < numberOfSpawners)
            {
                for (int i = _cachedNumberOfSpawners; i < numberOfSpawners; i++)
                {
                    spawnerSettings.TryAdd(i, new List<SpawnInstructionSO<SpawnableDataProviderSO>>());
                }
            }

            if (_cachedNumberOfSpawners > numberOfSpawners)
            {
                for (int i = numberOfSpawners; i < _cachedNumberOfSpawners; i++)
                {
                    spawnerSettings.Remove(i);
                }
            }
            
            if(spawnerSettings.Count != numberOfSpawners)
            {
                numberOfSpawners = spawnerSettings.Count;
            }

            _cachedNumberOfSpawners = numberOfSpawners;
        }
    }
}