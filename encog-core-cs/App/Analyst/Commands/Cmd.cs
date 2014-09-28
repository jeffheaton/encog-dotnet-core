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
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     Base class for Encog Analyst commands. This class defines the properties sent
    ///     to a command.
    /// </summary>
    public abstract class Cmd
    {
        /// <summary>
        ///     The analyst object that this command belongs to.
        /// </summary>
        private readonly EncogAnalyst _analyst;

        /// <summary>
        ///     The properties to use with this command.
        /// </summary>
        private readonly ScriptProperties _properties;

        /// <summary>
        ///     The script object that this command belongs to.
        /// </summary>
        private readonly AnalystScript _script;

        /// <summary>
        ///     Construct this command.
        /// </summary>
        /// <param name="theAnalyst">The analyst that this command belongs to.</param>
        protected Cmd(EncogAnalyst theAnalyst)
        {
            _analyst = theAnalyst;
            _script = _analyst.Script;
            _properties = _script.Properties;
        }


        /// <value>The analyst used with this command.</value>
        public EncogAnalyst Analyst
        {
            get { return _analyst; }
        }


        /// <value>The name of this command.</value>
        public abstract String Name { get; }


        /// <value>The properties used with this command.</value>
        public ScriptProperties Prop
        {
            get { return _properties; }
        }


        /// <value>The script used with this command.</value>
        public AnalystScript Script
        {
            get { return _script; }
        }

        /// <summary>
        ///     Execute this command.
        /// </summary>
        /// <param name="args">The arguments for this command.</param>
        /// <returns>True if processing should stop after this command.</returns>
        public abstract bool ExecuteCommand(String args);


        /// <inheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(Name);
            result.Append("]");
            return result.ToString();
        }
    }
}
