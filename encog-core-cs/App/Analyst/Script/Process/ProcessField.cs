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

namespace Encog.App.Analyst.Script.Process
{
    /// <summary>
    ///     Holds one field for Encog analyst preprocess.
    /// </summary>
    public class ProcessField
    {
        /// <summary>
        ///     The command for this field.
        /// </summary>
        private readonly String command;

        /// <summary>
        ///     The name of this field.
        /// </summary>
        private readonly String name;

        /// <summary>
        ///     Construct this field.
        /// </summary>
        /// <param name="name">The name of this field.</param>
        /// <param name="command">The command for this field.</param>
        public ProcessField(String name, String command)
        {
            this.name = name;
            this.command = command;
        }

        /// <summary>
        ///     The name of this field.
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        ///     The command for this field.
        /// </summary>
        public String Command
        {
            get { return command; }
        }
    }
}
