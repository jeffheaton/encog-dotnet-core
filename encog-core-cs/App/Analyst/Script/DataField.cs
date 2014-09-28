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

namespace Encog.App.Analyst.Script
{
    /// <summary>
    ///     Holds stats on a data field for the Encog Analyst. This data is used to
    ///     normalize the field.
    /// </summary>
    public class DataField
    {
        /// <summary>
        ///     The class members.
        /// </summary>
        private readonly IList<AnalystClassItem> _classMembers;

        /// <summary>
        ///     Construct the data field.
        /// </summary>
        /// <param name="theName">The name of this field.</param>
        public DataField(String theName)
        {
            _classMembers = new List<AnalystClassItem>();
            Name = theName;
            Min = Double.MaxValue;
            Max = Double.MinValue;
            Mean = Double.NaN;
            StandardDeviation = Double.NaN;
            Integer = true;
            Real = true;
            Class = true;
            Complete = true;
        }


        /// <value>the classMembers</value>
        public IList<AnalystClassItem> ClassMembers
        {
            get { return _classMembers; }
        }


        /// <value>the max to set</value>
        public double Max { get; set; }


        /// <value>the mean to set</value>
        public double Mean { get; set; }


        /// <value>the theMin to set</value>
        public double Min { get; set; }

        public string Source { get; set; }


        /// <summary>
        ///     Determine the minimum class count. This is the count of the
        ///     classification field that is the smallest.
        /// </summary>
        /// <value>The minimum class count.</value>
        public int MinClassCount
        {
            get { return _classMembers.Aggregate(Int32.MaxValue, (current, cls) => Math.Min(current, cls.Count)); }
        }


        /// <value>the name to set</value>
        public String Name { get; set; }


        /// <value>the standardDeviation to set</value>
        public double StandardDeviation { get; set; }


        /// <value>the isClass to set</value>
        public bool Class { get; set; }


        /// <value>the isComplete to set</value>
        public bool Complete { get; set; }


        /// <value>the isInteger to set</value>
        public bool Integer { get; set; }


        /// <value>the isReal to set</value>
        public bool Real { get; set; }
    }
}
