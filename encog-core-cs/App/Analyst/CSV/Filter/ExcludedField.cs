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

namespace Encog.App.Analyst.CSV.Filter
{
    /// <summary>
    ///     Used internally to track excluded fields from the FilterCSV.
    /// </summary>
    public class ExcludedField
    {
        /// <summary>
        ///     The field number.
        /// </summary>
        private int _fieldNumber;

        /// <summary>
        ///     The field value to filter on.
        /// </summary>
        private String _fieldValue;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="theFieldNumber">The field number.</param>
        /// <param name="theFieldValue">The field value to filter on.</param>
        public ExcludedField(int theFieldNumber, String theFieldValue)
        {
            _fieldNumber = theFieldNumber;
            _fieldValue = theFieldValue;
        }


        /// <value>the fieldNumber to set</value>
        public int FieldNumber
        {
            get { return _fieldNumber; }
            set { _fieldNumber = value; }
        }


        /// <value>the fieldValue to set</value>
        public String FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }


        /// <summary>
        /// </summary>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" fieldNumber=");
            result.Append(_fieldNumber);
            result.Append(", value=");
            result.Append(_fieldValue);

            result.Append("]");
            return result.ToString();
        }
    }
}
