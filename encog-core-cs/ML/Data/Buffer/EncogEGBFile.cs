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
        public const int DoubleSize = sizeof (double);

        /// <summary>
        /// The size of the file header.
        /// </summary>
        public const int HeaderSize = DoubleSize*3;

        /// <summary>
        /// The file that we are working with.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The binary reader.
        /// </summary>
        private BinaryReader _binaryReader;

        /// <summary>
        /// The binary writer.
        /// </summary>
        private BinaryWriter _binaryWriter;

        /// <summary>
        /// The number of ideal values per record.
        /// </summary>
        private int _idealCount;

        /// <summary>
        /// The number of input values per record.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// The number of records int he file.
        /// </summary>
        private int _numberOfRecords;

        /// <summary>
        /// The number of values in a record, this is the input and ideal combined.
        /// </summary>
        private int _recordCount;

        /// <summary>
        /// The size of a record.
        /// </summary>
        private int _recordSize;

        /// <summary>
        /// The underlying file.
        /// </summary>
        private FileStream _stream;

        /// <summary>
        /// Construct an EGB file. 
        /// </summary>
        /// <param name="file">The file.</param>
        public EncogEGBFile(String file)
        {
            _file = file;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return _inputCount; }
        }

        /// <summary>
        /// The ideal count.
        /// </summary>
        public int IdealCount
        {
            get { return _idealCount; }
        }

        /// <summary>
        /// The stream.
        /// </summary>
        public FileStream Stream
        {
            get { return _stream; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get { return _recordCount; }
        }

        /// <summary>
        /// The record size.
        /// </summary>
        public int RecordSize
        {
            get { return _recordSize; }
        }

        /// <summary>
        /// The number of records.
        /// </summary>
        public int NumberOfRecords
        {
            get { return _numberOfRecords; }
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
                _inputCount = inputCount;
                _idealCount = idealCount;

                var input = new double[inputCount];
                var ideal = new double[idealCount];

                if( _stream!=null )
                {
                    _stream.Close();
                    _stream = null;
                }

                _stream = new FileStream(_file, FileMode.Create, FileAccess.ReadWrite);
                _binaryWriter = new BinaryWriter(_stream);
                _binaryReader = null;

                _binaryWriter.Write((byte) 'E');
                _binaryWriter.Write((byte) 'N');
                _binaryWriter.Write((byte) 'C');
                _binaryWriter.Write((byte) 'O');
                _binaryWriter.Write((byte) 'G');
                _binaryWriter.Write((byte) '-');
                _binaryWriter.Write((byte) '0');
                _binaryWriter.Write((byte) '0');

                _binaryWriter.Write((double) input.Length);
                _binaryWriter.Write((double) ideal.Length);

                _numberOfRecords = 0;
                _recordCount = _inputCount + _idealCount + 1;
                _recordSize = _recordCount*DoubleSize;
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
                _stream = new FileStream(_file, FileMode.Open, FileAccess.Read);
                _binaryReader = new BinaryReader(_stream);
                _binaryWriter = null;

                bool isEncogFile = true;

                isEncogFile = isEncogFile ? _binaryReader.ReadByte() == 'E' : false;
                isEncogFile = isEncogFile ? _binaryReader.ReadByte() == 'N' : false;
                isEncogFile = isEncogFile ? _binaryReader.ReadByte() == 'C' : false;
                isEncogFile = isEncogFile ? _binaryReader.ReadByte() == 'O' : false;
                isEncogFile = isEncogFile ? _binaryReader.ReadByte() == 'G' : false;
                isEncogFile = isEncogFile ? _binaryReader.ReadByte() == '-' : false;

                if (!isEncogFile)
                {
                    throw new BufferedDataError(
                        "File is not a valid Encog binary file:"
                        + _file);
                }

                var v1 = (char) _binaryReader.ReadByte();
                var v2 = (char) _binaryReader.ReadByte();
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

                _inputCount = (int) _binaryReader.ReadDouble();
                _idealCount = (int) _binaryReader.ReadDouble();

                _recordCount = _inputCount + _idealCount + 1;
                _recordSize = _recordCount*DoubleSize;
                _numberOfRecords = (int) ((_stream.Length - HeaderSize)/_recordSize);
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
                if (_binaryWriter != null)
                {
                    _binaryWriter.Close();
                    _binaryWriter = null;
                }

                if (_binaryReader != null)
                {
                    _binaryReader.Close();
                    _binaryReader = null;
                }

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
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
        private long CalculateIndex(long row)
        {
            return (long)HeaderSize + (row*(long)_recordSize);
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row, or record, to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>THe value read.</returns>
        private int CalculateIndex(int row, int col)
        {
            return HeaderSize + (row*_recordSize)
                   + (col*DoubleSize);
        }

        /// <summary>
        /// Set the current location to the specified row. 
        /// </summary>
        /// <param name="row">The row.</param>
        public void SetLocation(int row)
        {
            try
            {
                _stream.Position = CalculateIndex(row);
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
                _stream.Position = CalculateIndex(row, col);
                _binaryWriter.Write(v);
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
                _stream.Position = CalculateIndex(row, 0);
                foreach (double t in v)
                {
                    _binaryWriter.Write(t);
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
                foreach (double t in v)
                {
                    _binaryWriter.Write(t);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

		/// <summary>
		/// Write the data from an IMLData
		/// </summary>
		/// <param name="v">The array to write.</param>
		public void Write(IMLData v)
		{
			try
			{
				for(int i = 0; i < v.Count; i++)
				{
					_binaryWriter.Write(v[i]);
				}
			}
			catch(IOException ex)
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
                _binaryWriter.Write(b);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write a double. 
        /// </summary>
        /// <param name="d">The double to write.</param>
        public void Write(double d)
        {
            try
            {
                _binaryWriter.Write(d);
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
                _stream.Position = CalculateIndex(row, col);
                return _binaryReader.ReadDouble();
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
                _stream.Position = CalculateIndex(row, 0);

                for (int i = 0; i < _recordCount; i++)
                {
                    d[i] = _binaryReader.ReadDouble();
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
                    d[i] = _binaryReader.ReadDouble();
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
                return _binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }
    }
}
