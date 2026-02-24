using System;

namespace EventSystem
{
    [Serializable]
    public abstract class MyEventArgs : EventArgs, IEventArgs
    {
        public abstract bool AreValid { get; }
        public abstract EventFlag Flag { get; }
        public abstract Guid ID { get; }
        public abstract IPublisher Publisher { get; }


        public abstract bool Equals(IDataProvider other);
    }

    [Serializable]
    public readonly struct MyEventArgsStruct : IEventArgs
    {
        public bool AreValid => true;
        public EventFlag Flag => EventFlag.None;
        public Guid ID => Guid.Empty;
        public IPublisher Publisher => default;

        public bool Equals(IDataProvider other)
        {
            if (other is MyEventArgsStruct otherStruct)
            {
                return AreValid == otherStruct.AreValid &&
                       Flag == otherStruct.Flag &&
                       ID == otherStruct.ID &&
                       Publisher.Equals(otherStruct.Publisher);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is MyEventArgsStruct other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(AreValid, Flag, ID, Publisher);
        }
    }
}
