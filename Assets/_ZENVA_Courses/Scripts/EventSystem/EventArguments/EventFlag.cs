using System;

namespace EventSystem
{
    /// <summary>
    /// EventFlags are used to provide additional information about the event being raised in an IEventChannel.
    /// They can be combined using bitwise operations to represent multiple states or conditions.
    /// </summary>
    [Flags, Serializable]
    public enum EventFlag : ulong
    {
        // Core & System
        None = 0,
        Critical = 1UL << 0,       // For events that require immediate attention, like errors or important state changes
        System = 1UL << 1,       // For system-level events, like game state changes or errors
        Debug = 1UL << 2,       // For events that are useful for debugging purposes
        Analytics = 1UL << 3,       // For events that are used for analytics or tracking user behavior
        User = 1UL << 4,       // For user-generated events, like input or user actions
        Settings = 1UL << 5,       // For events related to game settings or configurations
        SaveLoad = 1UL << 6,       // For events related to saving or loading game data

        // UI & UX
        UI = 1UL << 7,       // For events related to user interface interactions or updates
        UX = 1UL << 8,       // For events related to user experience or feedback
        Menu = 1UL << 9,       // For events related to menu interactions or navigation

        // Audio/Visual/Physics
        Camera = 1UL << 10,      // For events related to camera movements or changes
        Visual = 1UL << 11,      // For events related to visual effects or graphics
        Audio = 1UL << 12,      // For events related to audio playback or sound effects
        Physics = 1UL << 13,      // For events related to physics interactions or collisions

        // Gameplay & World
        Interaction = 1UL << 14,      // For events related to player interactions with objects or NPCs
        Gameplay = 1UL << 15,      // For events related to core gameplay mechanics or features
        PlayerCharacter = 1UL << 16,      // For events specifically related to the player characters
        NonPlayerCharacter = 1UL << 17,      // For events specifically related to non-player characters (NPCs)
        Environment = 1UL << 18,      // For events related to the game environment or world
        Inventory = 1UL << 19,      // For events related to inventory management or item interactions

        Combat = 1UL << 20,      // For events related to combat mechanics or actions
        Effect = 1UL << 21,      // For events related to status effects or buffs/debuffs
        Progression = 1UL << 22,      // For events related to player, quest, or game progression
        Quest = 1UL << 23,      // For events related to quests or missions
        Dialogue = 1UL << 24,      // For events related to dialogues or conversations
        Cinematic = 1UL << 25,      // For events related to cinematic sequences or cutscenes

        // Community & Online
        GameMaster = 1UL << 26,      // For events related to game master or admin actions
        Network = 1UL << 27,      // For events related to networking or online interactions
        Social = 1UL << 28,      // For events related to social features or interactions
        Achievement = 1UL << 29,      // For events related to achievements or milestones

        Modding = 1UL << 30,      // For events related to modding or custom content

        // Composite Flags
        UIInput = User | UI,                                            // For user interface input events

        CombatInteraction = Combat | Interaction,                       // For combat-related interaction events
        NonPlayerInteraction = NonPlayerCharacter | Interaction,        // For NPC interaction events
        EnvironmentInteraction = Environment | Interaction,             // For environment interaction events
    }
}
