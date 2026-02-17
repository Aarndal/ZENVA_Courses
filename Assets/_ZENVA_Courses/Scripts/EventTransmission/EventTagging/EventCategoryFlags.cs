using System;

namespace EventTransmission
{
    /// <summary>
    /// Category flags for events to help categorize and filter them.
    /// </summary>
    [Flags]
    public enum EventCategoryFlags : uint
    {
        // Core & System
        None                = 0,
        Critical            = 1 << 0,       // For events that require immediate attention, like errors or important state changes
        System              = 1 << 1,       // For system-level events, like game state changes or errors
        Debug               = 1 << 2,       // For events that are useful for debugging purposes
        Analytics           = 1 << 3,       // For events that are used for analytics or tracking user behavior
        User                = 1 << 4,       // For user-generated events, like input or user actions
        Settings            = 1 << 5,       // For events related to game settings or configurations
        SaveLoad            = 1 << 6,       // For events related to saving or loading game data

        // UI & UX
        UI                  = 1 << 7,       // For events related to user interface interactions or updates
        UX                  = 1 << 8,       // For events related to user experience or feedback
        Menu                = 1 << 9,       // For events related to menu interactions or navigation

        // Audio/Visual/Physics
        Camera              = 1 << 10,      // For events related to camera movements or changes
        Visual              = 1 << 11,      // For events related to visual effects or graphics
        Audio               = 1 << 12,      // For events related to audio playback or sound effects
        Physics             = 1 << 13,      // For events related to physics interactions or collisions

        // Gameplay & World
        Interaction         = 1 << 14,      // For events related to player interactions with objects or NPCs
        Gameplay            = 1 << 15,      // For events related to core gameplay mechanics or features
        PlayerCharacter     = 1 << 16,      // For events specifically related to the player characters
        NonPlayerCharacter  = 1 << 17,      // For events specifically related to non-player characters (NPCs)
        Environment         = 1 << 18,      // For events related to the game environment or world
        Inventory           = 1 << 19,      // For events related to inventory management or item interactions

        Combat              = 1 << 20,      // For events related to combat mechanics or actions
        Effect              = 1 << 21,      // For events related to status effects or buffs/debuffs
        Progression         = 1 << 22,      // For events related to player, quest, or game progression
        Quest               = 1 << 23,      // For events related to quests or missions
        Dialogue            = 1 << 24,      // For events related to dialogues or conversations
        Cinematic           = 1 << 25,      // For events related to cinematic sequences or cutscenes

        // Community & Online
        GameMaster          = 1 << 26,      // For events related to game master or admin actions
        Network             = 1 << 27,      // For events related to networking or online interactions
        Social              = 1 << 28,      // For events related to social features or interactions
        Achievement         = 1 << 29,      // For events related to achievements or milestones
        
        Modding             = 1 << 30,      // For events related to modding or custom content

        // Composite Flags
        UIInput = User | UI,                                            // For user interface input events
        
        CombatInteraction = Combat | Interaction,                       // For combat-related interaction events
        NonPlayerInteraction = NonPlayerCharacter | Interaction,        // For NPC interaction events
        EnvironmentInteraction = Environment | Interaction,             // For environment interaction events
    }
}
