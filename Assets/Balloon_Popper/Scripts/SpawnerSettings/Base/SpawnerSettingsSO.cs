using Project.Tools.DictionaryHelp;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnerSettingsSO<T> : ScriptableObject, ISpawnerSettings<SpawnInstructionSO<T>> where T : IDataProvider
{
    public abstract int NumberOfSpawners { get; }
    public abstract SerializableDictionary<int, List<SpawnInstructionSO<T>>> SpawnerSettings { get; }
}
