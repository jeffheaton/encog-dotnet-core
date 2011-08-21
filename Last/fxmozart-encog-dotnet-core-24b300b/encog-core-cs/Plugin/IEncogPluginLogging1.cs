using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Plugin
{
    /// <summary>
    /// A plugin that supports logging.  This is a version 1 plugin.
    /// </summary>
    public interface IEncogPluginLogging1: EncogPluginBase
    {
        /// <summary>
        /// The current log level.
        /// </summary>
        int LogLevel { get; }

        /// <summary>
        /// Log a message at the specified level. 
        /// </summary>
        /// <param name="level">The level to log at.</param>
        /// <param name="message">The message to log.</param>
        void Log(int level, String message);
        
        /// <summary>
        /// Log a throwable at the specified level.
        /// </summary>
        /// <param name="level">The level to log at.</param>
        /// <param name="t">The error to log.</param>
        void Log(int level, Exception t);
    }
}
