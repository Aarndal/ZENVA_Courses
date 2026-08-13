using System;

namespace Core
{
    /// <summary>
    /// Gives an object a switch that can be turned On, Off, or set to Pending during a transition.
    /// Used by Interactables (switchable objects) and Spawnables (available to a Spawner while Off).
    /// Unlike a plain IsActive flag, toggling can be gated by conditions via <see cref="CanToggle"/>.
    /// </summary>
    public interface IToggleable
    {
        public enum ToggleState : byte
        {
            Off = 0,
            On = 1,
            Pending = 2,
        }

        /// <summary>
        /// Whether the switch can currently be toggled.
        /// Allows conditions (locked, cooldown, missing key, ...) and player feedback.
        /// </summary>
        bool CanToggle { get; }

        /// <summary>The current state of the switch.</summary>
        ToggleState CurrentState { get; }

        /// <summary>Raised after a successful state change, carrying the new state.</summary>
        event Action<ToggleState> StateChanged;

        /// <summary>
        /// Attempts to toggle the switch.
        /// </summary>
        /// <param name="requester">
        /// Optional requester initiating the toggle (e.g. an IInteractor or a Spawner).
        /// Implementations may use it to validate whether this requester is allowed to toggle.
        /// </param>
        /// <returns>true if the state changed; otherwise, false.</returns>
        bool TryToggle(object requester = null);
    }
}