using System;

namespace Debugging
{
    /// <summary>
    /// Handles message construction, placeholder replacement, and strict format rules.
    /// 
    /// Two output modes:
    ///   • Unformatted — returns a single pre-baked string (for Debug.Log / LogWarning / LogError).
    ///   • Formatted   — returns a composite format string + args array
    ///                    (for Debug.LogFormat / LogWarningFormat / LogErrorFormat).
    /// </summary>
    public static class LogFormatter
    {
        private const string UnityObjectHeader =
            "Message by Object: {0} | InstanceID: {1}\n";

        private const string PlainObjectHeader =
            "Message by Type: {0}\n";

        // ══════════════════════════════════════════════════════════════
        //  Unformatted path — returns a fully baked string
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Builds a complete log string with all placeholders already resolved.
        /// Used for Debug.Log / LogWarning / LogError.
        /// </summary>
        public static string BuildUnformatted(
            string objectName,
            int? instanceId,
            bool isUnityObject,
            string logMessage,
            object[] args)
        {
            string header = BuildHeader(objectName, instanceId, isUnityObject);
            string body = ReplacePlaceholdersAndFormat(logMessage, args);
            return header + body;
        }

        // ══════════════════════════════════════════════════════════════
        //  Formatted path — returns format string + args for Unity's
        //  LogFormat / LogWarningFormat / LogErrorFormat
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Builds a composite format string and an args array suitable for
        /// Debug.LogFormat(context, format, args).
        /// Strict formatting rules are applied here.
        /// </summary>
        public static (string format, object[] args) BuildFormatted(
            string objectName,
            int? instanceId,
            bool isUnityObject,
            string logMessage,
            object[] args)
        {
            // Step 1: Build the header (already resolved — no placeholders)
            string header = BuildHeader(objectName, instanceId, isUnityObject);

            // Step 2: Convert {data} → {0}, {1}, ... in the user message
            (string indexedMessage, int placeholderCount) = IndexPlaceholders(logMessage);

            // Step 3: Apply strict formatting to the body
            indexedMessage = ApplyStrictFormat(indexedMessage);

            // Step 4: Combine header + body into one format string
            //         Header is literal text, body contains {n} placeholders.
            string combinedFormat = header + indexedMessage;

            // Step 5: Trim args to match placeholder count
            object[] trimmedArgs = args;
            if (args != null && args.Length > placeholderCount)
            {
                trimmedArgs = new object[placeholderCount];
                Array.Copy(args, trimmedArgs, placeholderCount);
            }

            return (combinedFormat, trimmedArgs ?? Array.Empty<object>());
        }

        // ══════════════════════════════════════════════════════════════
        //  Internals
        // ══════════════════════════════════════════════════════════════

        private static string BuildHeader(
            string objectName, int? instanceId, bool isUnityObject)
        {
            if (string.IsNullOrEmpty(objectName))
                return string.Empty;

            return isUnityObject
                ? string.Format(UnityObjectHeader, objectName, instanceId ?? 0)
                : string.Format(PlainObjectHeader, objectName);
        }

        /// <summary>
        /// Replaces {data} tokens with indexed {n} placeholders and then
        /// immediately formats the string (for the unformatted path).
        /// </summary>
        private static string ReplacePlaceholdersAndFormat(
            string message, object[] args)
        {
            if (args == null || args.Length == 0)
                return message;

            (string indexed, _) = IndexPlaceholders(message);

            try
            {
                return string.Format(indexed, args);
            }
            catch (FormatException)
            {
                return indexed + "  [FORMAT ERROR — argument count mismatch]";
            }
        }

        /// <summary>
        /// Converts each sequential {data} in the message to {0}, {1}, {2}, etc.
        /// Returns the indexed string and the number of placeholders found.
        /// </summary>
        private static (string indexed, int count) IndexPlaceholders(string message)
        {
            int count = 0;
            string result = message;

            while (true)
            {
                int pos = result.IndexOf("{data}", StringComparison.Ordinal);
                if (pos < 0)
                    break;

                result = result.Remove(pos, "{data}".Length)
                               .Insert(pos, $"{{{count}}}");
                count++;
            }

            return (result, count);
        }

        /// <summary>
        /// Applies strict formatting rules to the message body.
        /// Extend this method with project-specific conventions.
        /// </summary>
        private static string ApplyStrictFormat(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"[{timestamp}] ── {message}";
        }
    }
}