// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Buffer.CODEC;
using Encog.Util.CSV;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// An implementation of the NeuralDataSet interface designed to provide a CSV
    /// file to the neural network. This implementation uses the BasicMLData to
    /// hold the data being read. This class has no ability to write CSV files.
    /// The columns of the CSV file will specify both the input and ideal 
    /// columns.  
    /// 
    /// This class is not memory based, so very long files can be used, 
    /// without running out of memory.
    /// </summary>
    public class CsvMlDataSet : BasicMLDataSet
    {


        /// <summary>
        /// The CSV filename to read from.
        /// </summary>
        private String filename;

        /// <summary>
        /// The number of columns of input data.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// The number of columns of ideal data.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The format that separates the columns, defaults to a comma.
        /// </summary>
        private CSVFormat format;

        /// <summary>
        /// Specifies if headers are present on the first row.
        /// </summary>
        private bool headers;

        /// <summary>
        /// Get the filename for the CSV file.
        /// </summary>
        public String Filename
        {
            get
            {
                return this.filename;
            }
        }

        /// <summary>
        /// The delimiter.
        /// </summary>
        public CSVFormat Format
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// True if the first row specifies field names.
        /// </summary>
        public bool Headers
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>
        /// The amount of ideal data.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        /// <summary>
        /// The amount of input data.
        /// </summary>
        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        public CsvMlDataSet(String filename, int inputSize,
                 int idealSize, bool headers)
            : this(filename, inputSize, idealSize, headers, CSVFormat.ENGLISH)
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
        public CsvMlDataSet(String filename, int inputSize,
                 int idealSize, bool headers, CSVFormat format)
        {
            this.filename = filename;
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            this.format = format;
            this.headers = headers;

            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, inputSize, idealSize);
            MemoryDataLoader load = new MemoryDataLoader(codec);
            load.Result = this;
            load.External2Memory();
        }
    }
}
