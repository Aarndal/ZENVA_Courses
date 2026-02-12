using Project.Tools.DictionaryHelp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Obsolete("This interface is deprecated. Please use the new ISpawner interface instead.")]
public interface ISpawnerSettings<T>
{
    int NumberOfSpawners { get; }
    SerializableDictionary<int, List<T>> Instructions { get; }
}
