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
using System.Text;

namespace Encog.App.Analyst.Script.Task
{
    /// <summary>
    ///     Holds a task in the script. A task is a named set of commands.
    /// </summary>
    public class AnalystTask
    {
        /// <summary>
        ///     The "source code" for this task.
        /// </summary>
        private readonly IList<String> _lines;

        /// <summary>
        ///     The name of the task.
        /// </summary>
        private String _name;

        /// <summary>
        ///     Construct an analyst task.
        /// </summary>
        /// <param name="theName">The name of this task.</param>
        public AnalystTask(String theName)
        {
            _lines = new List<String>();
            _name = theName;
        }


        /// <value>the lines</value>
        public IList<String> Lines
        {
            get { return _lines; }
        }


        /// <value>the name to set</value>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// </summary>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append("]");
            return result.ToString();
        }
    }
}
