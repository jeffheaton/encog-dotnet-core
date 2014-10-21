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

namespace Encog.App.Analyst.Script.Segregate
{
    /// <summary>
    ///     This class specifies a target for the segregation process.
    /// </summary>
    public class AnalystSegregateTarget
    {
        /// <summary>
        ///     The file target.
        /// </summary>
        private String _file;

        /// <summary>
        ///     Construct the segregation target.
        /// </summary>
        /// <param name="theFile">The file.</param>
        /// <param name="thePercent">The percent.</param>
        public AnalystSegregateTarget(String theFile, int thePercent)
        {
            _file = theFile;
            Percent = thePercent;
        }


        /// <value>the file to set</value>
        public String File
        {
            get { return _file; }
            set { _file = value; }
        }


        /// <value>the percent to set</value>
        public int Percent { get; set; }


        /// <imheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" file=");
            result.Append(_file);
            result.Append(", percent=");
            result.Append(_file);
            result.Append("]");
            return result.ToString();
        }
    }
}
