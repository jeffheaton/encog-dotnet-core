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
using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Buffer.CODEC;
using Encog.Util.CSV;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// An implementation of the MLDataSet interface designed to provide a CSV
    /// file to the neural network. This implementation uses the BasicMLData to
    /// hold the data being read. This class has no ability to write CSV files.
    /// The columns of the CSV file will specify both the input and ideal 
    /// columns.  
    /// 
    /// This class is not memory based, so very long files can be used, 
    /// without running out of memory.
    /// </summary>
    public class CSVMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// The CSV filename to read from.
        /// </summary>
        private readonly String _filename;

        /// <summary>
        /// The format that separates the columns, defaults to a comma.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// Specifies if headers are present on the first row.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The number of columns of ideal data.
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// The number of columns of input data.
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers)
            : this(filename, inputSize, idealSize, headers, CSVFormat.English, false)
        {
        }

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        /// <param name="format">The format to use.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers, CSVFormat format, bool expectSignificance)
        {
            _filename = filename;
            _inputSize = inputSize;
            _idealSize = idealSize;
            _format = format;
            _headers = headers;

            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, inputSize, idealSize, expectSignificance);
            var load = new MemoryDataLoader(codec) {Result = this};
            load.External2Memory();
        }

        /// <summary>
        /// Get the filename for the CSV file.
        /// </summary>
        public String Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// The delimiter.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
        }

        /// <summary>
        /// True if the first row specifies field names.
        /// </summary>
        public bool Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// The amount of ideal data.
        /// </summary>
        public override int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// The amount of input data.
        /// </summary>
        public override int InputSize
        {
            get { return _inputSize; }
        }
    }
}
