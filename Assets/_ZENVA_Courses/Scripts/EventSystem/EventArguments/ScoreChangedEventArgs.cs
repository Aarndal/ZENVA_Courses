using Debugging;
using System;

namespace EventSystem
{
    public readonly struct ScoreChangedEventArgs : IEventArgs
    {
        public EventFlag Flag { get; }
        public IScoreChanger ScoreChanger { get; }
        public readonly Guid ID => this.GetType().GUID;
        public IPublisher Publisher { get; }

        public bool AreValid => Validate();

        private bool Validate()
        {
            if (ScoreChanger == null || ScoreChanger.ScoreChangeValue == 0)
            {
                DebugLogger.Log(
                    LogMessageType.Warning,
                    this,
                    "ScoreChangedEventArgs is invalid: {0}" +
                    "\nPublisher: {1}",
                    true,
                    (ScoreChanger == null ?
                    "ScoreChanger is null." :
                    ScoreChanger.ScoreChangeValue.GetType().ToString() + " is 0."),
                    (Publisher != null ? Publisher.ToString() : "Anonymous")
                    );

                return false;
            }
            return true;
        }

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
