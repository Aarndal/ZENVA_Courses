namespace EventTransmission
{
    public interface IEventArgs
    {
        /// <summary>
        /// Returns true if the event arguments are valid and c
        /// an be processed; otherwise, false.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Defines the event category as well as various flags for filtering and processing.
        /// </summary>
        //EventTagsContainer Tags { get; }
    }
}