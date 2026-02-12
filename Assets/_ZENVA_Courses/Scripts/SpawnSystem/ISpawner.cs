using System;
using System.Collections.ObjectModel;

namespace SpawnSystem
{
    public interface ISpawner
    {
        ReadOnlyCollection<ISpawnerInstruction> Instructions { get; }
        
        event Action SpawningStarted;
        event Action SpawningStopped;

        void StartSpawning();
        void StopSpawning();

        bool TrySetInstructions(Collection<ISpawnerInstruction> instructions);
    }
}
