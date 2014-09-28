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
using Encog.ML.Prg.Ext;

namespace Encog.App.Analyst.Script.ML
{
    /// <summary>
    ///     An opcode, stored in the script.
    /// </summary>
    public class ScriptOpcode
    {
        /// <summary>
        ///     The argument count of the opcode.
        /// </summary>
        private readonly int argCount;

        /// <summary>
        ///     The name of the opcode.
        /// </summary>
        private readonly String name;

        /// <summary>
        ///     Construct the opcode.
        /// </summary>
        /// <param name="name">The name of the opcode.</param>
        /// <param name="argCount">The argument count.</param>
        public ScriptOpcode(String name, int argCount)
        {
            this.name = name;
            this.argCount = argCount;
        }

        /// <summary>
        ///     Construct using an extension template.
        /// </summary>
        /// <param name="temp">The template.</param>
        public ScriptOpcode(IProgramExtensionTemplate temp)
            : this(temp.Name, temp.ChildNodeCount)
        {
        }

        /// <summary>
        ///     The name.
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        ///     The argument count.
        /// </summary>
        public int ArgCount
        {
            get { return argCount; }
        }
    }
}
