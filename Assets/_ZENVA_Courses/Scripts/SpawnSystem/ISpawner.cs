using System;
using System.Collections.Generic;

namespace SpawnSystem
{
    public interface ISpawner
    {
        Queue<ISpawnerInstruction> Instructions { get; }
        
        event Action SpawningStarted;
        event Action SpawningStopped;

        void StartSpawning();
        void StopSpawning();

        bool TrySetInstructions(IEnumerable<ISpawnerInstruction> instructions);
    }
}
