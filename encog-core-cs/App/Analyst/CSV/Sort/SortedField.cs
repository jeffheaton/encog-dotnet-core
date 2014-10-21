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

namespace Encog.App.Analyst.CSV.Sort
{
    /// <summary>
    ///     Specifies how a field is to be sorted by SortCSV.
    /// </summary>
    public class SortedField
    {
        /// <summary>
        ///     The index of the field.
        /// </summary>
        private int _index;

        /// <summary>
        ///     The type of sort.
        /// </summary>
        private SortType _sortType;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="theIndexindex">The index of the sorted field.</param>
        /// <param name="t">The type of sort, the type of object.</param>
        /// <param name="theAscending">True, if this is an ascending sort.</param>
        public SortedField(int theIndexindex, SortType t,
                           bool theAscending)
        {
            _index = theIndexindex;
            Ascending = theAscending;
            _sortType = t;
        }


        /// <value>the index to set</value>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        /// <value>the sortType to set</value>
        public SortType SortType
        {
            get { return _sortType; }
            set { _sortType = value; }
        }


        /// <value>the ascending to set</value>
        public bool Ascending { get; set; }


        /// <inheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" index=");
            result.Append(_index);
            result.Append(", type=");
            result.Append(_sortType);

            result.Append("]");
            return result.ToString();
        }
    }
}
