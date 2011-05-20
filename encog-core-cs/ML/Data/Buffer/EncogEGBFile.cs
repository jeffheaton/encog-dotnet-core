//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using System.IO;

namespace Encog.ML.Data.Buffer
{
    /// <summary>
    /// Reads in little endian form.
    /// </summary>
    public class EncogEGBFile
    {
        /// <summary>
        /// The size of a double.
        /// </summary>
        public const int DOUBLE_SIZE = sizeof (double);

        /// <summary>
        /// The size of the file header.
        /// </summary>
        public const int HEADER_SIZE = DOUBLE_SIZE*3;

        /// <summary>
        /// The file that we are working with.
        /// </summary>
        private readonly String file;

        /// <summary>
        /// The binary reader.
        /// </summary>
        private BinaryReader binaryReader;

        /// <summary>
        /// The binary writer.
        /// </summary>
        private BinaryWriter binaryWriter;

        /// <summary>
        /// The number of ideal values per record.
        /// </summary>
        private int idealCount;

        /// <summary>
        /// The number of input values per record.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The number of records int he file.
        /// </summary>
        private int numberOfRecords;

        /// <summary>
        /// The number of values in a record, this is the input and ideal combined.
        /// </summary>
        private int recordCount;

        /// <summary>
        /// The size of a record.
        /// </summary>
        private int recordSize;

        /// <summary>
        /// The underlying file.
        /// </summary>
        private FileStream stream;

        /// <summary>
        /// Construct an EGB file. 
        /// </summary>
        /// <param name="file">The file.</param>
        public EncogEGBFile(String file)
        {
            this.file = file;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
        }

        /// <summary>
        /// The ideal count.
        /// </summary>
        public int IdealCount
        {
            get { return idealCount; }
        }

        /// <summary>
        /// The stream.
        /// </summary>
        public FileStream Stream
        {
            get { return stream; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get { return recordCount; }
        }

        /// <summary>
        /// The record size.
        /// </summary>
        public int RecordSize
        {
            get { return recordSize; }
        }

        /// <summary>
        /// The number of records.
        /// </summary>
        public int NumberOfRecords
        {
            get { return numberOfRecords; }
        }

        /// <summary>
        /// Create a new RGB file. 
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The ideal count.</param>
        public void Create(int inputCount, int idealCount)
        {
            try
            {
                this.inputCount = inputCount;
                this.idealCount = idealCount;

                var input = new double[inputCount];
                var ideal = new double[idealCount];

                stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite);
                binaryWriter = new BinaryWriter(stream);
                binaryReader = null;

                binaryWriter.Write((byte) 'E');
                binaryWriter.Write((byte) 'N');
                binaryWriter.Write((byte) 'C');
                binaryWriter.Write((byte) 'O');
                binaryWriter.Write((byte) 'G');
                binaryWriter.Write((byte) '-');
                binaryWriter.Write((byte) '0');
                binaryWriter.Write((byte) '0');

                binaryWriter.Write((double) input.Length);
                binaryWriter.Write((double) ideal.Length);

                numberOfRecords = 0;
                recordCount = this.inputCount + this.idealCount;
                recordSize = recordCount*DOUBLE_SIZE;
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Open an existing EGB file.
        /// </summary>
        public void Open()
        {
            try
            {
                stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                binaryReader = new BinaryReader(stream);
                binaryWriter = null;

                bool isEncogFile = true;

                isEncogFile = isEncogFile ? binaryReader.ReadByte() == 'E' : false;
                isEncogFile = isEncogFile ? binaryReader.ReadByte() == 'N' : false;
                isEncogFile = isEncogFile ? binaryReader.ReadByte() == 'C' : false;
                isEncogFile = isEncogFile ? binaryReader.ReadByte() == 'O' : false;
                isEncogFile = isEncogFile ? binaryReader.ReadByte() == 'G' : false;
                isEncogFile = isEncogFile ? binaryReader.ReadByte() == '-' : false;

                if (!isEncogFile)
                {
                    throw new BufferedDataError(
                        "File is not a valid Encog binary file:"
                        + file);
                }

                var v1 = (char) binaryReader.ReadByte();
                var v2 = (char) binaryReader.ReadByte();
                String versionStr = "" + v1 + v2;

                try
                {
                    int version = int.Parse(versionStr);
                    if (version > 0)
                    {
                        throw new BufferedDataError(
                            "File is from a newer version of Encog than is currently in use.");
                    }
                }
                catch (Exception)
                {
                    throw new BufferedDataError("File has invalid version number.");
                }

                inputCount = (int) binaryReader.ReadDouble();
                idealCount = (int) binaryReader.ReadDouble();

                recordCount = inputCount + idealCount;
                recordSize = recordCount*DOUBLE_SIZE;
                numberOfRecords = (int) ((stream.Length - HEADER_SIZE)/recordSize);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            try
            {
                if (binaryWriter != null)
                {
                    binaryWriter.Close();
                    binaryWriter = null;
                }

                if (binaryReader != null)
                {
                    binaryReader.Close();
                    binaryReader = null;
                }

                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Calculate the index for the specified row. 
        /// </summary>
        /// <param name="row">The row to calculate for.</param>
        /// <returns>The index.</returns>
        private int CalculateIndex(int row)
        {
            return HEADER_SIZE + (row*recordSize);
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row, or record, to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>THe value read.</returns>
        private int CalculateIndex(int row, int col)
        {
            return HEADER_SIZE + (row*recordSize)
                   + (col*DOUBLE_SIZE);
        }

        /// <summary>
        /// Set the current location to the specified row. 
        /// </summary>
        /// <param name="row">The row.</param>
        public void SetLocation(int row)
        {
            try
            {
                stream.Position = CalculateIndex(row);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write the specified row and column. 
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <param name="v">The value.</param>
        public void Write(int row, int col, double v)
        {
            try
            {
                stream.Position = CalculateIndex(row, col);
                binaryWriter.Write(v);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write an array at the specified record.
        /// </summary>
        /// <param name="row">The record to write.</param>
        /// <param name="v">The array to write.</param>
        public void Write(int row, double[] v)
        {
            try
            {
                stream.Position = CalculateIndex(row, 0);
                for (int i = 0; i < v.Length; i++)
                {
                    binaryWriter.Write(v[i]);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write an array. 
        /// </summary>
        /// <param name="v">The array to write.</param>
        public void Write(double[] v)
        {
            try
            {
                for (int i = 0; i < v.Length; i++)
                {
                    binaryWriter.Write(v[i]);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write a byte. 
        /// </summary>
        /// <param name="b">The byte to write.</param>
        public void Write(byte b)
        {
            try
            {
                binaryWriter.Write(b);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>The value read.</returns>
        public double Read(int row, int col)
        {
            try
            {
                stream.Position = CalculateIndex(row, col);
                return binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a double array at the specified record. 
        /// </summary>
        /// <param name="row">The record to read.</param>
        /// <param name="d">The array to read into.</param>
        public void Read(int row, double[] d)
        {
            try
            {
                stream.Position = CalculateIndex(row, 0);

                for (int i = 0; i < recordCount; i++)
                {
                    d[i] = binaryReader.ReadDouble();
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read an array of doubles. 
        /// </summary>
        /// <param name="d">The array to read into.</param>
        public void Read(double[] d)
        {
            try
            {
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = binaryReader.ReadDouble();
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a single double. 
        /// </summary>
        /// <returns>The double read.</returns>
        public double Read()
        {
            try
            {
                return binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }
    }
}
