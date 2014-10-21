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
using Encog.ML.Data;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    ///     A row of a CSV file loaded to memory. This class is used internally by many
    ///     of the Encog quant classes.
    /// </summary>
    public class LoadedRow
    {
        /// <summary>
        ///     The row data.
        /// </summary>
        private readonly String[] _data;

        /// <summary>
        ///     Load a row from the specified CSV file.
        /// </summary>
        /// <param name="csv">The CSV file to use.</param>
        public LoadedRow(ReadCSV csv) : this(csv, 0)
        {
        }

        /// <summary>
        ///     Construct a loaded row.
        /// </summary>
        /// <param name="csv">The CSV file to use.</param>
        /// <param name="extra">The number of extra columns to add.</param>
        public LoadedRow(ReadCSV csv, int extra)
        {
            int count = csv.GetCount();
            _data = new String[count + extra];
            for (int i = 0; i < count; i++)
            {
                _data[i] = csv.Get(i);
            }
        }

        /// <summary>
        ///     Construct a loaded row from an array.
        /// </summary>
        /// <param name="format">The format to store the numbers in.</param>
        /// <param name="data">The data to use.</param>
        /// <param name="extra">The extra positions to allocate.</param>
        public LoadedRow(CSVFormat format, double[] data, int extra)
        {
            int count = data.Length;
            _data = new String[count + extra];
            for (int i = 0; i < count; i++)
            {
                _data[i] = format.Format(data[i], 5);
            }
        }

        /// <summary>
        ///     Construct a loaded row from an IMLData.
        /// </summary>
        /// <param name="format">The format to store the numbers in.</param>
        /// <param name="data">The data to use.</param>
        /// <param name="extra">The extra positions to allocate.</param>
        public LoadedRow(CSVFormat format, IMLData data, int extra)
        {
            int count = data.Count;
            _data = new String[count + extra];
            for (int i = 0; i < count; i++)
            {
                _data[i] = format.Format(data[i], 5);
            }
        }


        /// <value>The row data.</value>
        public String[] Data
        {
            get { return _data; }
        }
    }
}
