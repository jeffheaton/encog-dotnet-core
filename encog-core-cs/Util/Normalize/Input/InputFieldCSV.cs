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

namespace Encog.Util.Normalize.Input
{
    /// <summary>
    /// An input field based on a CSV file.
    /// </summary>
    [Serializable]
    public class InputFieldCSV : BasicInputField
    {
       
        /// <summary>
        /// The file to read.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The CSV column represented by this field.
        /// </summary>
        private readonly int _offset;


        private readonly string _columnName;
        /// <summary>
        /// Construct an InputFieldCSV with the default constructor.  This is mainly
        /// used for reflection.
        /// </summary>
        public InputFieldCSV()
        {
        }




        /// <summary>
        /// Construct a input field for a CSV file.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for actual 
        /// input to the neural network, as opposed to segregation only.</param>
        /// <param name="file">The tile to read.</param>
        /// <param name="offset">The CSV file column to read.</param>
        public InputFieldCSV(bool usedForNetworkInput, String file,
                             int offset)
        {
            _file = file;
            _offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// Construct a input field for a CSV file.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for actual
        /// input to the neural network, as opposed to segregation only.</param>
        /// <param name="file">The tile to read.</param>
        /// <param name="columnname">The columnname you wish to read.</param>
        public InputFieldCSV(bool usedForNetworkInput, String file,
                             string columnname)
        {
            _file = file;
            _columnName = columnname;

            UsedForNetworkInput = usedForNetworkInput;
        }
        /// <summary>
        /// The file being read.
        /// </summary>
        public String File
        {
            get { return _file; }
        }

        /// <summary>
        /// The column in this CSV file to read.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// Gets the name of the column we want to read.
        /// </summary>
        /// <value>
        /// The name of the column we want to read.
        /// </value>
        public string ColumnName
        {
            get { return _columnName; }
        }
    }
}
