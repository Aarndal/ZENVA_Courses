using System;

namespace EventSystem
{
    public readonly struct ScoreChangedEventArgs : IEventArgs 
    {
        public EventFlag Flag { get; }
        public IScoreChanger ScoreChanger { get; }
        public readonly Guid ID => this.GetType().GUID;

        public ScoreChangedEventArgs(IScoreChanger scoreChanger, EventFlag flag = EventFlag.None)
        {
            ScoreChanger = scoreChanger;
            Flag = flag;
        }

        public readonly bool Equals(IDataProvider other)
        {
            if (other == null) return false;
            return ID == other.ID;
        }
    }
}