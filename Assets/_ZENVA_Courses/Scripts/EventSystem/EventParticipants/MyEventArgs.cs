using System;

namespace EventSystem
{
    [Serializable]
    public abstract class MyEventArgs : EventArgs, IEventArgs
    {
        public abstract bool AreValid { get; }
        public abstract EventFlag Flag { get; }
        public abstract IPublisher Publisher { get; }
    }

    [Serializable]
    public readonly struct MyEventArgsStruct : IEventArgs
    {
        public bool AreValid => true;
        public EventFlag Flag => EventFlag.None;
        public IPublisher Publisher => default;
    }
}
