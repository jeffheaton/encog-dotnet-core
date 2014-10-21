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

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    ///     A basic cached column. Used internally by some of the Encog CSV quant
    ///     classes. All of the file contents for this column are loaded into memory.
    /// </summary>
    public class BaseCachedColumn
    {
        /// <summary>
        ///     The data for this column.
        /// </summary>
        private double[] _data;

        /// <summary>
        ///     Construct the cached column.
        /// </summary>
        /// <param name="theName">The name of the column.</param>
        /// <param name="theInput">Is this column used for input?</param>
        /// <param name="theOutput">Is this column used for output?</param>
        public BaseCachedColumn(String theName, bool theInput,
                                bool theOutput)
        {
            Name = theName;
            Input = theInput;
            Output = theOutput;
            Ignore = false;
        }


        /// <value>The data for this column.</value>
        public double[] Data
        {
            get { return _data; }
        }


        /// <summary>
        ///     Set the name of this column.
        /// </summary>
        public String Name { get; set; }


        /// <summary>
        ///     Set if this column is to be ignored?
        /// </summary>
        public bool Ignore { get; set; }


        /// <summary>
        ///     Set if this column is used for input.
        /// </summary>
        public bool Input { get; set; }


        /// <summary>
        ///     Set if this column is used for output.
        /// </summary>
        public bool Output { get; set; }

        /// <summary>
        ///     Allocate enough space for this column.
        /// </summary>
        public void Allocate(int length)
        {
            _data = new double[length];
        }
    }
}
