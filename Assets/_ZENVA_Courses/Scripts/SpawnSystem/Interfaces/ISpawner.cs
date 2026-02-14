using System;
using System.Collections.Generic;

namespace SpawnSystem
{
    /// <summary>
    /// Interface representing a spawner that can execute a sequence of spawning instructions.
    /// The spawner can spawn objects based on the provided data.
    /// </summary>
    public interface ISpawner
    {
        /// <summary>Queue of instructions for the spawner to execute.</summary>
        Queue<ISpawnerInstruction> Instructions { get; }

        /// <summary>Event invoked when the spawner starts executing its instructions.</summary>
        event Action SpawningStarted;
        /// <summary>Event invoked when the spawner stops executing its instructions.</summary>
        event Action SpawningStopped;

        /// <summary>
        /// Initiates the execution of the spawner's instructions.
        /// The spawner will process the instructions in the order they were added to the queue.
        /// It stops executing when there are no more instructions in the queue or when StopSpawning is called.
        /// </summary>
        void StartSpawning();
        /// <summary>
        /// Stops the execution of the spawner's instructions.
        /// Is called automatically when there are no more instructions in the queue, but can also be called manually to interrupt the spawning process.
        /// </summary>
        void StopSpawning();

        /// <summary>
        /// Sets the instructions for the spawner to execute.
        /// </summary>
        /// <param name="instructions">The instructions to be executed by the spawner.</param>
        /// <returns>true if the instructions were successfully set; otherwise, false.</returns>
        bool TrySetInstructions(IEnumerable<ISpawnerInstruction> instructions);
    }
}
