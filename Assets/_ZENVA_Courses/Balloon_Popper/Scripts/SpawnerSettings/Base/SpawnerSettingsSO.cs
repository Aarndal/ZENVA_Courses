using Project.Tools.DictionaryHelp;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnerSettingsSO<T> : ScriptableObject, ISpawnerSettings<ISpawnerInstruction<T>> where T : IDataProvider<ISpawnable>
{
    public abstract int NumberOfSpawners { get; }
    public abstract SerializableDictionary<int, List<ISpawnerInstruction<T>>> Instructions { get; }
}
