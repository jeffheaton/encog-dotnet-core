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
