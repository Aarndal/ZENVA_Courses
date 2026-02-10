using Project.Tools.DictionaryHelp;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonPopper
{
    /// <summary>
    /// ScriptableObject that defines the spawner settings for levels.
    /// </summary>
    [CreateAssetMenu(fileName = "newSpawnerSettings", menuName = "BalloonPopper/Spawner/SpawnerSettings")]
    public class BalloonSpawnerSettingsSO : SpawnerSettingsSO<IDataProvider<ISpawnable>>
    {
        [SerializeField, Min(1)]
        private int numberOfSpawners = 1;
        [SerializeField]
        private SerializableDictionary<int, List<SpawnInstructionSO>> spawnerSettings = new();

        private int _cachedNumberOfSpawners = 0;

        public override int NumberOfSpawners => numberOfSpawners;
        //public override SerializableDictionary<int, List<ISpawnInstruction<IDataProvider<ISpawnable>>>> SpawnerSettings => spawnerSettings;

        public override SerializableDictionary<int, List<ISpawnInstruction<IDataProvider<ISpawnable>>>> Instructions
        {
            get
            {
                var result = new SerializableDictionary<int, List<ISpawnInstruction<IDataProvider<ISpawnable>>>>();
                
                foreach (var kvp in spawnerSettings)
                {
                    var list = new List<ISpawnInstruction<IDataProvider<ISpawnable>>>();
                    
                    foreach (var item in kvp.Value)
                    {
                        list.Add(item);
                    }

                    result.TryAdd(kvp.Key, list);
                }

                return result;
            }
        }






        private void OnValidate()
        {
            if (_cachedNumberOfSpawners == numberOfSpawners && spawnerSettings.Count == numberOfSpawners) return;

            if(_cachedNumberOfSpawners < numberOfSpawners)
            {
                for (int i = _cachedNumberOfSpawners; i < numberOfSpawners; i++)
                {
                    spawnerSettings.TryAdd(i, new ());
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