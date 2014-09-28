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

namespace Encog.Cloud.Indicator.Basic
{
    /// <summary>
    /// Used to hold instruments, i.e. ticker symbols of securities.
    /// Also holds financial data downloaded by ticker symbol.
    /// </summary>
    public class InstrumentHolder
    {
        /// <summary>
        /// The downloaded financial data.
        /// </summary>
        private readonly IDictionary<long, string> _data = new Dictionary<long, string>();

        /// <summary>
        /// The sorted data.
        /// </summary>
        private readonly ICollection<long> _sorted = new SortedSet<long>();

        /// <summary>
        /// The data.
        /// </summary>
        public IDictionary<long, string> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Sorted keys.
        /// </summary>
        public ICollection<long> Sorted
        {
            get { return _sorted; }
        }

        /// <summary>
        /// Record one piece of data. Data with the same time stamp.
        /// </summary>
        /// <param name="when">The time the data occurred.</param>
        /// <param name="starting">Where should we start from when storing, index into data.
        /// Allows unimportant "leading data" to be discarded without creating a new
        /// array.</param>
        /// <param name="data">The financial data.</param>
        /// <returns>True, if the data did not exist already.</returns>
        public bool Record(long when, int starting, String[] data)
        {
            var str = new StringBuilder();

            for (int i = starting; i < data.Length; i++)
            {
                if (i > starting)
                {
                    str.Append(',');
                }
                str.Append(data[i]);
            }

            bool result = !_data.ContainsKey(when);
            _sorted.Add(when);
            this._data[when] = str.ToString();
            return result;
        }
    }
}
