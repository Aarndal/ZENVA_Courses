using Project.Tools.DictionaryHelp;
using SpawnSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("This class is deprecated.", false)]
public abstract class SpawnerSettingsSO<T> : ScriptableObject, ISpawnerSettings<ISpawnerInstruction<T>> where T : IDataProvider<ISpawnable>
{
    public abstract int NumberOfSpawners { get; }
    public abstract SerializableDictionary<int, List<ISpawnerInstruction<T>>> Instructions { get; }
}
