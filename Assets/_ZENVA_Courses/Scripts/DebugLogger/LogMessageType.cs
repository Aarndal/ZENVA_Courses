namespace DebugLogger
{
    /// <summary>
    /// Defines the type of log message and whether strict formatting is applied.
    /// </summary>
    public enum LogMessageType : byte
    {
        Message,
        MessageFormatted,
        Warning,
        WarningFormatted,
        Error,
        ErrorFormatted,
        ErrorWithPause,
        ErrorFormattedWithPause
    }
}