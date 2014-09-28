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

namespace Encog.App.Analyst.Script
{
    /// <summary>
    ///     Holds a class item for the script. Some columns in a CSV are classes. This
    ///     object holds the possible class types.
    /// </summary>
    public class AnalystClassItem : IComparable<AnalystClassItem>
    {
        /// <summary>
        ///     THe code for the class item.
        /// </summary>
        private String _code;

        /// <summary>
        ///     THe count.
        /// </summary>
        private int _count;

        /// <summary>
        ///     The name for the class item.
        /// </summary>
        private String _name;

        /// <summary>
        ///     Construct a class item.
        /// </summary>
        /// <param name="theCode">The code, this is how it is in the CSV.</param>
        /// <param name="theName"></param>
        /// <param name="theCount">The count.</param>
        public AnalystClassItem(String theCode, String theName,
                                int theCount)
        {
            _code = theCode;
            _name = theName;
            _count = theCount;
        }


        /// <value>the code to set</value>
        public String Code
        {
            get { return _code; }
            set { _code = value; }
        }


        /// <value>The count.</value>
        public int Count
        {
            get { return _count; }
        }


        /// <value>the name to set</value>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #region IComparable<AnalystClassItem> Members

        /// <summary>
        /// </summary>
        public int CompareTo(AnalystClassItem o)
        {
            return String.CompareOrdinal(_code, o.Code);
        }

        #endregion

        /// <summary>
        ///     Increase the count.
        /// </summary>
        public void IncreaseCount()
        {
            _count++;
        }

        /// <inheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(_name);
            result.Append(", code=");
            result.Append(_code);
            result.Append("]");
            return result.ToString();
        }
    }
}
