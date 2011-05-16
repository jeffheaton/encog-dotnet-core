using System;

namespace Encog.Util.Logging
{
    /// <summary>
    /// This class provides logging for Encog. Programs using Encog can make use of
    /// it as well. All logging is passed on to the current logging plugin. By
    /// default the SystemLoggingPlugin is used.
    /// </summary>
    ///
    public class EncogLogging
    {
        /// <summary>
        /// The lowest level log type. Debug logging provides low-level Encog
        /// diagnostics that may slow performance, but allow you to peer into the
        /// inner workings.
        /// </summary>
        ///
        public const int LEVEL_DEBUG = 0;

        /// <summary>
        /// Info logging tells you when major processes start and stop.
        /// </summary>
        ///
        public const int LEVEL_INFO = 1;

        /// <summary>
        /// Error level tells you about errors, less important to critical.
        /// </summary>
        ///
        public const int LEVEL_ERROR = 2;

        /// <summary>
        /// Critical logging logs errors that cannot be recovered from.
        /// </summary>
        ///
        public const int LEVEL_CRITICAL = 3;

        /// <summary>
        /// Logging is disabled at this level.
        /// </summary>
        ///
        public const int LEVEL_DISABLE = 4;

        /// <value>The current logging level.</value>
        public int CurrentLevel
        {
            /// <returns>The current logging level.</returns>
            get { return EncogFramework.Instance.LoggingPlugin.LogLevel; }
        }

        /// <summary>
        /// Log the message.
        /// </summary>
        ///
        /// <param name="level">The level to log at.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(int level, String message)
        {
            EncogFramework.Instance.LoggingPlugin.Log(level, message);
        }

        /// <summary>
        /// Log the error.
        /// </summary>
        ///
        /// <param name="level">The level to log at.</param>
        /// <param name="t">The exception to log.</param>
        public static void Log(int level, Exception t)
        {
            EncogFramework.Instance.LoggingPlugin.Log(level, t);
        }

        /// <summary>
        /// Log the error at ERROR level.
        /// </summary>
        ///
        /// <param name="t">The exception to log.</param>
        public static void Log(Exception t)
        {
            Log(LEVEL_ERROR, t);
        }
    }
}