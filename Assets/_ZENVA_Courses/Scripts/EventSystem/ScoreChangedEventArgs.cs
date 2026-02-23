using System;

namespace EventSystem
{
    public readonly struct ScoreChangedEventArgs : IEventArgs
    {
        public EventFlag Flag { get; }
        public IScoreChanger ScoreChanger { get; }
        public readonly Guid ID => this.GetType().GUID;
        public IPublisher Publisher { get; }

        public ScoreChangedEventArgs(IScoreChanger scoreChanger, EventFlag flag = EventFlag.None, IPublisher publisher = null)
        {
            ScoreChanger = scoreChanger;
            Flag = flag;

            if (publisher != null && !publisher.IsAnonymous)
                Publisher = publisher;
            else
                Publisher = null;
        }

        public readonly bool Equals(IDataProvider other)
        {
            if (other == null) return false;
            return ID == other.ID;
        }
    }
}