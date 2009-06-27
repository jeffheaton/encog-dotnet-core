// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;
using Encog.Neural.Data.Basic;
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData.CSV
{
    /// <summary>
    /// An implementation of the NeuralDataSet interface designed to provide a CSV
    /// file to the neural network. This implementation uses the BasicNeuralData to
    /// hold the data being read. This class has no ability to write CSV files.
    /// The columns of the CSV file will specify both the input and ideal 
    /// columns.  
    /// 
    /// This class is not memory based, so very long files can be used, 
    /// without running out of memory.
    /// </summary>
    public class CSVNeuralDataSet : INeuralDataSet, IEnumerable<INeuralDataPair>
    {
        /// <summary>
        /// The enumerator for the CSVNeuralDataSet.
        /// </summary>
        public class CSVNeuralEnumerator : IEnumerator<INeuralDataPair>
        {
            private CSVNeuralDataSet owner;

            /// <summary>
            /// A ReadCSV object used to parse the CSV file.
            /// </summary>
            private ReadCSV reader;

            /// <summary>
            /// The enumerator for the CSV set.
            /// </summary>
            /// <param name="owner">The owner of this enumerator.</param>
            public CSVNeuralEnumerator(CSVNeuralDataSet owner)
            {
                this.owner = owner;
                this.reader = null;
                this.reader = new ReadCSV(this.owner.Filename,
                        this.owner.Headers,
                        this.owner.Delimiter);
            }

            /// <summary>
            /// The current item.
            /// </summary>
            public INeuralDataPair Current
            {
                get
                {
                    INeuralData input = new BasicNeuralData(this.owner.InputSize);
                    INeuralData ideal = null;

                    for (int i = 0; i < this.owner.InputSize; i++)
                    {
                        input[i] = double.Parse(this.reader.Get(i));
                    }

                    if (this.owner.IdealSize > 0)
                    {
                        ideal = new BasicNeuralData(this.owner.IdealSize);
                        for (int i = 0; i < this.owner.IdealSize; i++)
                        {
                            ideal[i] = double.Parse(this.reader.Get(i
                                    + this.owner.InputSize));
                        }
                    }

                    return new BasicNeuralDataPair(input, ideal);
                }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                this.reader.Close();
            }

            /// <summary>
            /// Get the current item.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                return this.reader.Next();
            }

            /// <summary>
            /// Not supported.
            /// </summary>
            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

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
        /// The delimiter that separates the columns, defaults to a comma.
        /// </summary>
        private char delimiter;

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
        public char Delimiter
        {
            get
            {
                return this.delimiter;
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
        public CSVNeuralDataSet(String filename, int inputSize,
                 int idealSize, bool headers)
            : this(filename, inputSize, idealSize, headers, ',')
        {
        }

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        /// <param name="delimiter">The delimiter to use.</param>
        public CSVNeuralDataSet(String filename, int inputSize,
                 int idealSize, bool headers, char delimiter)
        {
            this.filename = filename;
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            this.delimiter = delimiter;
            this.headers = headers;
        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(INeuralData data1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(INeuralDataPair inputData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public void Close()
        {
            // not needed
        }

        /// <summary>
        /// Get an enumerator.
        /// </summary>
        /// <returns>The enumerator to use.</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new CSVNeuralEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator.
        /// </summary>
        /// <returns>The enumerator to use.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
