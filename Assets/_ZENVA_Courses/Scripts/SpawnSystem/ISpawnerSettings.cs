using Project.Tools.DictionaryHelp;
using System.Collections.Generic;

public interface ISpawnerSettings<T>
{
    int NumberOfSpawners { get; }
    SerializableDictionary<int, List<T>> Instructions { get; }
}
