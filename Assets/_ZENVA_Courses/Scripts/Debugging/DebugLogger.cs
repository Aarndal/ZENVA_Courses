using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Debugging
{
    /// <summary>
    /// Static Debug Logger utility for Unity 6.x, built on Unity's ILogHandler system.
    ///
    /// ┌──────────────────────────────────────────────────────────────────┐
    /// │  PUBLIC API:  DLogger.Debug(...)                                │
    /// │  — single entry point, routes internally by LogMessageType     │
    /// └──────────────────────────────────────────────────────────────────┘
    ///
    /// Gating levels:
    ///   1. Compile-time  — [Conditional("ENABLE_LOGS")]
    ///   2. Global runtime — GlobalLoggingEnabled + per-severity toggles
    ///   3. Per-call local — isLoggingEnabled (Message/Warning only)
    ///   4. Errors are NEVER locally suppressible.
    ///
    /// Context auto-detection:
    ///   Caller passes 'this' as System.Object. Internally we detect:
    ///   - UnityEngine.Object → name + InstanceID + Console click-highlight
    ///   - Plain C# object   → Type name only
    ///   - null               → no header
    /// </summary>
    public static class DebugLogger
    {
        // ══════════════════════════════════════════════════════════════
        //  Unity Logger instance (backed by custom ILogHandler)
        // ══════════════════════════════════════════════════════════════

        private static readonly UnityEngine.Logger _logger;

        static DebugLogger()
        {
            var handler = new DebugLogHandler();
            _logger = new UnityEngine.Logger(handler)
            {
                logEnabled = true,
                filterLogType = LogType.Log // Allow everything by default
            };
        }

        // ══════════════════════════════════════════════════════════════
        //  Global toggles
        // ══════════════════════════════════════════════════════════════

        /// <summary>Master kill-switch. When false, ALL logging is suppressed.</summary>
        public static bool GlobalLoggingEnabled { get; set; } = true;

        /// <summary>Global toggle for Message / MessageFormatted.</summary>
        public static bool EnableMessages { get; set; } = true;

        /// <summary>Global toggle for Warning / WarningFormatted.</summary>
        public static bool EnableWarnings { get; set; } = true;

        // No toggle for Errors — by design.

        // ══════════════════════════════════════════════════════════════
        //  PUBLIC ENTRY POINT
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Logs a message through the appropriate Unity Debug method.
        ///
        /// <paramref name="caller"/> should be <c>this</c> from the calling script.
        /// If the caller is a UnityEngine.Object, the object's name and EntityID
        /// are automatically prepended and the Console entry is click-linkable.
        /// 
        /// <paramref name="isLoggingEnabled"/> only affects Message and Warning
        /// severity. Errors always log regardless of this flag.
        /// </summary>
        /// <param name="type">Determines severity and whether formatting is applied.</param>
        /// <param name="caller">
        ///     Pass <c>this</c>. Auto-detected as UnityEngine.Object or plain type.
        ///     Pass <c>null</c> for static/global contexts.
        /// </param>
        /// <param name="logMessage">
        ///     Message template. Use <c>{data}</c> as placeholders.
        /// </param>
        /// <param name="isLoggingEnabled">
        ///     Per-call toggle (default true). Ignored for Error severity.
        /// </param>
        /// <param name="args">Up to six data values replacing <c>{data}</c> in order.</param>
        [Conditional("ENABLE_LOGS")]
        public static void Log(
            LogMessageType type,
            object caller,
            string logMessage,
            bool isLoggingEnabled = true,
            params object[] args)
        {
            if (!GlobalLoggingEnabled)
                return;

            // Clamp to 6 parameters
            if (args != null && args.Length > 6)
            {
                var clamped = new object[6];
                System.Array.Copy(args, clamped, 6);
                args = clamped;

                _logger.LogWarning("DLogger",
                    "[DLogger] More than 6 data parameters supplied — extras ignored.");
            }

            // ── Extract context info from the caller ──
            ExtractCallerInfo(
                caller,
                out string objectName,
                out int? instanceId,
                out bool isUnityObject,
                out Object unityContext);

            // ── Route to the correct private handler ──
            switch (type)
            {
                case LogMessageType.Info:
                case LogMessageType.InfoFormatted:
                    LogMessage(type, unityContext, objectName, instanceId,
                               isUnityObject, logMessage, isLoggingEnabled, args);
                    break;

                case LogMessageType.Warning:
                case LogMessageType.WarningFormatted:
                    LogWarning(type, unityContext, objectName, instanceId,
                                isUnityObject, logMessage, isLoggingEnabled, args);
                    break;

                case LogMessageType.Error:
                case LogMessageType.ErrorFormatted:
                case LogMessageType.ErrorWithPause:
                case LogMessageType.ErrorFormattedWithPause:
                    // isLoggingEnabled intentionally NOT forwarded
                    LogError(type, unityContext, objectName, instanceId,
                              isUnityObject, logMessage, args);
                    break;
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  PRIVATE — severity handlers
        // ══════════════════════════════════════════════════════════════

        private static void LogMessage(
            LogMessageType type,
            Object unityContext,
            string objectName,
            int? instanceId,
            bool isUnityObject,
            string logMessage,
            bool isLoggingEnabled,
            object[] args)
        {
            if (!isLoggingEnabled || !EnableMessages)
                return;

            if (type == LogMessageType.InfoFormatted)
            {
                var (format, fmtArgs) = LogFormatter.BuildFormatted(
                    objectName, instanceId, isUnityObject, logMessage, args);

                _logger.LogFormat(LogType.Log, unityContext, format, fmtArgs);
            }
            else
            {
                string final = LogFormatter.BuildUnformatted(
                    objectName, instanceId, isUnityObject, logMessage, args);

                _logger.Log(LogType.Log, (object)final, unityContext);
            }
        }

        private static void LogWarning(
            LogMessageType type,
            Object unityContext,
            string objectName,
            int? instanceId,
            bool isUnityObject,
            string logMessage,
            bool isLoggingEnabled,
            object[] args)
        {
            if (!isLoggingEnabled || !EnableWarnings)
                return;

            if (type == LogMessageType.WarningFormatted)
            {
                var (format, fmtArgs) = LogFormatter.BuildFormatted(
                    objectName, instanceId, isUnityObject, logMessage, args);

                _logger.LogFormat(LogType.Warning, unityContext, format, fmtArgs);
            }
            else
            {
                string final = LogFormatter.BuildUnformatted(
                    objectName, instanceId, isUnityObject, logMessage, args);

                _logger.Log(LogType.Warning, (object)final, unityContext);
            }
        }

        private static void LogError(
            LogMessageType type,
            Object unityContext,
            string objectName,
            int? instanceId,
            bool isUnityObject,
            string logMessage,
            object[] args)
        {
            // Errors are never locally suppressible

            bool isPause = type is LogMessageType.ErrorWithPause
                                or LogMessageType.ErrorFormattedWithPause;

            bool isFormatted = type is LogMessageType.ErrorFormatted
                                    or LogMessageType.ErrorFormattedWithPause;

            if (isFormatted)
            {
                var (format, fmtArgs) = LogFormatter.BuildFormatted(
                    objectName, instanceId, isUnityObject, logMessage, args);

                _logger.LogFormat(LogType.Error, unityContext, format, fmtArgs);
            }
            else
            {
                string final = LogFormatter.BuildUnformatted(
                    objectName, instanceId, isUnityObject, logMessage, args);

                _logger.Log(LogType.Error, (object)final, unityContext);
            }

            if (isPause)
            {
                UnityEngine.Debug.Break();
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  PRIVATE — caller auto-detection
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Inspects the caller object and extracts context information.
        /// The user never supplies name/ID directly — it's always derived.
        /// </summary>
        private static void ExtractCallerInfo(
            object caller,
            out string objectName,
            out int? entityId,
            out bool isUnityObject,
            out Object unityContext)
        {
            if (caller is Object uo)
            {
                // UnityEngine.Object — full context
                objectName = uo.name;
                entityId = uo.GetEntityId();
                isUnityObject = true;
                unityContext = uo;  // enables Console click-to-highlight
            }
            else if (caller != null)
            {
                // Plain C# object — type name only
                objectName = caller.GetType().Name;
                entityId = null;
                isUnityObject = false;
                unityContext = null;
            }
            else
            {
                // No caller context
                objectName = null;
                entityId = null;
                isUnityObject = false;
                unityContext = null;
            }
        }
    }
}