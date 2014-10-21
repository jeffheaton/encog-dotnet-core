//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
        public const int LevelDebug = 0;

        /// <summary>
        /// Info logging tells you when major processes start and stop.
        /// </summary>
        ///
        public const int LevelInfo = 1;

        /// <summary>
        /// Error level tells you about errors, less important to critical.
        /// </summary>
        ///
        public const int LevelError = 2;

        /// <summary>
        /// Critical logging logs errors that cannot be recovered from.
        /// </summary>
        ///
        public const int LevelCritical = 3;

        /// <summary>
        /// Logging is disabled at this level.
        /// </summary>
        ///
        public const int LevelDisable = 4;

        /// <value>The current logging level.</value>
        public int CurrentLevel
        {
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
            Log(LevelError, t);
        }
    }
}
