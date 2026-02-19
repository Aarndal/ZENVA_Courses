using UnityEngine;

namespace Debugging
{
    /// <summary>
    /// Custom ILogHandler that wraps Unity's default log handler.
    /// This integrates our logger into Unity's logging pipeline so that
    /// features like log filtering via Logger.filterLogType, stacktrace
    /// settings, and Console click-to-highlight all work natively.
    /// </summary>
    public class DebugLogHandler : ILogHandler
    {
        private readonly ILogHandler _defaultHandler;

        public DebugLogHandler()
        {
            // Capture Unity's built-in handler before we replace anything
            _defaultHandler = UnityEngine.Debug.unityLogger.logHandler;
        }

        /// <summary>
        /// Called by Unity's Logger for formatted log output.
        /// Delegates to Unity's default handler to preserve native Console behaviour.
        /// </summary>
        public void LogFormat(
            LogType logType,
            Object context,
            string format,
            params object[] args)
        {
            _defaultHandler.LogFormat(logType, context, format, args);
        }

        /// <summary>
        /// Called by Unity's Logger for exception output.
        /// </summary>
        public void LogException(
            System.Exception exception,
            Object context)
        {
            _defaultHandler.LogException(exception, context);
        }
    }
}