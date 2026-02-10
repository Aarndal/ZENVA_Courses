using Project.Tools.DictionaryHelp;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnerSettingsSO<T> : ScriptableObject, ISpawnerSettings<ISpawnInstruction<T>> where T : IDataProvider<ISpawnable>
{
    public abstract int NumberOfSpawners { get; }
    public abstract SerializableDictionary<int, List<ISpawnInstruction<T>>> Instructions { get; }
}
