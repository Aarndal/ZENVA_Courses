namespace Debugging
{
    /// <summary>
    /// Defines the type of log message and whether strict formatting is applied.
    /// </summary>
    public enum LogMessageType : byte
    {
        Info,
        InfoFormatted,
        Warning,
        WarningFormatted,
        Error,
        ErrorFormatted,
        ErrorWithPause,
        ErrorFormattedWithPause
    }
}