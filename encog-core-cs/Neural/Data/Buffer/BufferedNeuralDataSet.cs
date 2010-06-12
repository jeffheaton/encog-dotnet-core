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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using System.IO;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Data.Buffer
{
    /// <summary>
    /// This class is not memory based, so very long files can be used, without
    /// running out of memory. This dataset uses a binary file as a buffer. When used
    /// with a slower access dataset, such as CSV, XML or SQL, where parsing must
    /// occur, this dataset can be used to load from the slower dataset and train at
    /// much higher speeds.
    /// 
    /// If you are going to create a binary file, by using the add methods, you must
    /// call beginLoad to cause Encog to open an output file. Once the data has been
    /// loaded, call endLoad.
    /// 
    /// The floating point numbers stored to the binary file may not be cross
    /// platform.
    /// </summary>
    public class BufferedNeuralDataSet : INeuralDataSet, IIndexable
    {
        /// <summary>
        /// An enumerator to move through the buffered data set.
        /// </summary>
        public class BufferedNeuralDataSetEnumerator : IEnumerator<INeuralDataPair>
        {

            private BufferedNeuralDataSet owner;

            /// <summary>
            /// The file to read from.
            /// </summary>
            private BinaryReader input;

            /// <summary>
            /// The next data pair to read.
            /// </summary>
            private INeuralDataPair next;

            /// <summary>
            /// The data pair that was just read.
            /// </summary>
            private INeuralDataPair current;

            /// <summary>
            /// Construct the buffered enumerator. This is where the file is actually
            /// opened.
            /// </summary>
            /// <param name="owner">The object that created this enumeration.</param>
            public BufferedNeuralDataSetEnumerator(BufferedNeuralDataSet owner)
            {

                this.owner = owner;
                this.input = new BinaryReader(
                        new FileStream(owner.BufferFile, FileMode.Open, FileAccess.Read,FileShare.Read));
                this.input.ReadInt32();
                this.input.ReadInt32();

                this.next = CreatePair();
                this.current = CreatePair();
            }

            /// <summary>
            /// Close the enumerator, and the underlying file.
            /// </summary>
            public void Close()
            {

                this.input.Close();

            }

            /// <summary>
            /// Create a neural data pair of the correct size.
            /// </summary>
            /// <returns>The pair created.</returns>
            private INeuralDataPair CreatePair()
            {
                INeuralDataPair result;

                if (this.owner.IdealSize > 0)
                {
                    result = new BasicNeuralDataPair(new BasicNeuralData(
                            (int)this.owner.InputSize),
                            new BasicNeuralData(
                                    (int)this.owner.IdealSize));
                }
                else
                {
                    result = new BasicNeuralDataPair(new BasicNeuralData(
                            (int)this.owner.InputSize));
                }

                return result;
            }


            /// <summary>
            /// Get the current record
            /// </summary>
            public INeuralDataPair Current
            {
                get
                {
                    return this.next;
                }
            }

            /// <summary>
            /// Dispose of the enumerator.
            /// </summary>
            public void Dispose()
            {
            }


            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (this.next == null)
                    {
                        throw new NeuralDataError("Can't read current record until MoveNext is called once.");
                    }
                    return this.next;
                }
            }

            /// <summary>
            /// Move to the next element.
            /// </summary>
            /// <returns>True if there are more elements to read.</returns>
            public bool MoveNext()
            {
                try
                {
                    if (this.owner.IdealSize > 0)
                    {
                        this.owner.ReadDoubleArray(this.input, this.next.Input);
                        this.owner.ReadDoubleArray(this.input, this.next.Ideal);
                    }
                    else
                    {
                        this.owner.ReadDoubleArray(this.input, this.next.Input);
                    }
                    return true;
                }
                catch (EndOfStreamException)
                {
                    this.input.Close();
                    return false;
                }
            }

            /// <summary>
            /// Not implemented.
            /// </summary>
            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Error message for ADD.
        /// </summary>
        public const String ERROR_ADD = "Add can only be used after calling beginLoad.";


        /// <summary>
        /// The buffer file to use.
        /// </summary>
        private String bufferFile;

        /// <summary>
        /// The size of the input data.
        /// </summary>
        private long inputSize;

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        private long idealSize;

        /// <summary>
        /// The size(in bytes) of a record.
        /// </summary>
        private int recordSize;

        /// <summary>
        /// The enumerators.
        /// </summary>
        private ICollection<BufferedNeuralDataSetEnumerator> enumerators = new List<BufferedNeuralDataSetEnumerator>();

        /// <summary>
        /// A random access file to use for output.
        /// </summary>
        private BinaryWriter output;

        /// <summary>
        /// The current input file.
        /// </summary>
        private BinaryReader input;

        /// <summary>
        /// Construct a buffered dataset using the specified file. 
        /// </summary>
        /// <param name="bufferFile">The file to read/write binary data to/from.</param>
        public BufferedNeuralDataSet(String bufferFile)
        {
            this.bufferFile = bufferFile;

            if (File.Exists(bufferFile))
            {
                FileStream f =
                       new FileStream(this.bufferFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader o = new BinaryReader(f);
                this.inputSize = o.ReadInt64();
                this.idealSize = o.ReadInt64();
                this.recordSize = (this.InputSize * 8) + (this.IdealSize * 8);
                o.Close();
                f.Close();
            }

        }

        /// <summary>
        /// Add only input data, for an unsupervised dataset. 
        /// </summary>
        /// <param name="data1">The data to be added.</param>
        public void Add(INeuralData data1)
        {
            if (this.output == null)
            {
                throw new NeuralDataError(BufferedNeuralDataSet.ERROR_ADD);
            }
            WriteDoubleArray(data1);

        }

        /// <summary>
        /// Add both the input and ideal data. 
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public void Add(INeuralData inputData, INeuralData idealData)
        {
            if (this.output == null)
            {
                throw new NeuralDataError(BufferedNeuralDataSet.ERROR_ADD);
            }
            WriteDoubleArray(inputData);
            WriteDoubleArray(idealData);
        }

        /// <summary>
        /// Add a data pair of both input and ideal data. 
        /// </summary>
        /// <param name="inputData">The pair to add.</param>
        public void Add(INeuralDataPair inputData)
        {
            if (this.output == null)
            {
                throw new NeuralDataError(BufferedNeuralDataSet.ERROR_ADD);
            }
            WriteDoubleArray(inputData.Input);
            if (inputData.Ideal != null)
            {
                WriteDoubleArray(inputData.Ideal);
            }
        }

        /// <summary>
        /// Begin loading to the binary file. After calling this method the add
        /// methods may be called.
        /// </summary>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void BeginLoad(int inputSize, int idealSize)
        {

            this.inputSize = inputSize;
            this.idealSize = idealSize;
            this.recordSize = (this.InputSize * 8) + (this.IdealSize * 8);
            File.Delete(this.bufferFile);
            this.output = new BinaryWriter(new FileStream(this.bufferFile, FileMode.CreateNew));
            // write the header
            this.output.Write(this.inputSize);
            this.output.Write(this.idealSize);


        }

        /// <summary>
        /// Close all enumerators.
        /// </summary>
        public void Close()
        {
            foreach (BufferedNeuralDataSetEnumerator enumerator in this.enumerators)
            {
                enumerator.Close();
            }

        }

        /// <summary>
        /// This method should be called once all the data has been loaded. The
        /// underlying file will be closed.
        /// </summary>
        public void EndLoad()
        {
            this.output.Close();
            this.output = null;

        }

        /// <summary>
        /// Load from the specified data source into the binary file. Do not call
        /// beginLoad before calling this method, as this is handled internally.
        /// </summary>
        /// <param name="source">The source.</param>
        public void Load(INeuralDataSet source)
        {
            BeginLoad(source.InputSize, source.IdealSize);

            // write the data
            foreach (INeuralDataPair pair in source)
            {
                if (pair.Input != null)
                {
                    WriteDoubleArray(pair.Input);
                }
                if (pair.Ideal != null)
                {
                    WriteDoubleArray(pair.Ideal);
                }
            }

            EndLoad();
        }

        /// <summary>
        /// Open a second buffered data set, useful for multithreading. 
        /// </summary>
        /// <returns>The additional buffered data set.</returns>
        public IIndexable OpenAdditional()
        {
            return new BufferedNeuralDataSet(this.bufferFile);
        }

        /// <summary>
        /// Open an input file to allow records to be read randomly.
        /// </summary>
        private void OpenInputFile()
        {

            if (this.input == null)
            {
                this.input = new BinaryReader(new FileStream(this.bufferFile, FileMode.Open, FileAccess.Read,FileShare.Read));
            }

        }

        /// <summary>
        /// Read an array of doubles from the file. 
        /// </summary>
        /// <param name="raf">The random access file to read from.</param>
        /// <param name="data">The neural data to read this array into.</param>
        private void ReadDoubleArray(BinaryReader raf,
                 INeuralData data)
        {
            double[] d = data.Data;
            for (int i = 0; i < data.Count; i++)
            {
                d[i] = raf.ReadDouble();
            }
        }

        /// <summary>
        /// Write a double array from the specified data to the file. 
        /// </summary>
        /// <param name="data">The data that holds the array.</param>
        private void WriteDoubleArray(INeuralData data)
        {

            for (int i = 0; i < data.Count; i++)
            {
                this.output.Write(data[i]);
            }

        }

        /// <summary>
        /// The record count.
        /// </summary>
        public long Count
        {
            get
            {
                return (new FileInfo(this.bufferFile)).Length / this.recordSize;
            }
        }

        /// <summary>
        /// Get a record by index and copy it into the specified pair.
        /// </summary>
        /// <param name="index">The index to load.</param>
        /// <param name="pair">The pair to copy into.</param>
        public void GetRecord(long index, INeuralDataPair pair)
        {
            OpenInputFile();
            this.input.BaseStream.Position = index * this.recordSize;
            if (idealSize > 0)
            {
                ReadDoubleArray(this.input, pair.Input);
                ReadDoubleArray(this.input, pair.Ideal);
            }
            else
            {
                ReadDoubleArray(this.input, pair.Input);
            }

        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return (int)this.idealSize;
            }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get
            {
                return (int)this.inputSize;
            }
        }

        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            if (this.output != null)
            {
                throw new NeuralDataError(
                        "Can't create enumerator while loading, call endLoad first.");
            }
            BufferedNeuralDataSetEnumerator result = new BufferedNeuralDataSetEnumerator(this);
            this.enumerators.Add(result);
            return result;

        }

        /// <summary>
        /// The name of the buffer file.
        /// </summary>
        public String BufferFile
        {
            get
            {
                return this.bufferFile;
            }
        }

    }
}
