using System;

namespace EventSystem
{
    /// <summary>
    /// EventFlags are used to provide additional information about the event being raised in an IEventChannel.
    /// They can be combined using bitwise operations to represent multiple states or conditions.
    /// </summary>
    [Flags]
    public enum EventFlag : byte
    {
        None = 0,
    }
}