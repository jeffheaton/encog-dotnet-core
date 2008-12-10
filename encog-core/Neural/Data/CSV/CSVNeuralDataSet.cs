// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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

namespace Encog.Neural.NeuralData.CSV
{
    public class CSVNeuralDataSet : INeuralDataSet, IEnumerable<INeuralDataPair>
    {
        public class CSVNeuralEnumerator : IEnumerator<INeuralDataPair>
        {
            private CSVNeuralDataSet owner;

            /// <summary>
            /// A ReadCSV object used to parse the CSV file.
            /// </summary>
            private ReadCSV reader;

            public CSVNeuralEnumerator(CSVNeuralDataSet owner)
            {
                this.owner = owner;
                this.reader = null;
                this.reader = new ReadCSV(this.owner.Filename,
                        this.owner.Headers,
                        this.owner.Delimiter);
            }

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

            public void Dispose()
            {
                this.reader.Close();
            }

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                return this.reader.Next();
            }

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

        public String Filename
        {
            get
            {
                return this.filename;
            }
        }

        public char Delimiter
        {
            get
            {
                return this.delimiter;
            }
        }

        public bool Headers
        {
            get
            {
                return this.headers;
            }
        }

        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

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

        public void Add(INeuralData data1)
        {
            throw new NotImplementedException();
        }

        public void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new NotImplementedException();
        }

        public void Add(INeuralDataPair inputData)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            // not needed
        }

        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new CSVNeuralEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
