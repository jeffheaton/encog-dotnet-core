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
using System.IO;

namespace Encog.Neural.Data.Buffer
{
    /// <summary>
    /// Reads in little endian form.
    /// </summary>
    public class EncogEGBFile
    {
        /// <summary>
        /// The size of a double.
        /// </summary>
        public const int DOUBLE_SIZE = sizeof(double);

        /// <summary>
        /// The size of the file header.
        /// </summary>
        public const int HEADER_SIZE = DOUBLE_SIZE * 3;

        /// <summary>
        /// The file that we are working with.
        /// </summary>
        private String file;

        /// <summary>
        /// The number of input values per record.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The number of ideal values per record.
        /// </summary>
        private int idealCount;

        /// <summary>
        /// The underlying file.
        /// </summary>
        private FileStream stream;

        /// <summary>
        /// The binary reader.
        /// </summary>
        private BinaryReader binaryReader;

        /// <summary>
        /// The binary writer.
        /// </summary>
        private BinaryWriter binaryWriter;

        /// <summary>
        /// The number of values in a record, this is the input and ideal combined.
        /// </summary>
        private int recordCount;

        /// <summary>
        /// The size of a record.
        /// </summary>
        private int recordSize;

        /// <summary>
        /// The number of records int he file.
        /// </summary>
        private int numberOfRecords;

        /// <summary>
        /// Construct an EGB file. 
        /// </summary>
        /// <param name="file">The file.</param>
        public EncogEGBFile(String file)
        {
            this.file = file;
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

                double[] input = new double[inputCount];
                double[] ideal = new double[idealCount];

                this.stream = new FileStream(this.file, FileMode.Create, FileAccess.ReadWrite);
                this.binaryWriter = new BinaryWriter(this.stream);
                this.binaryReader = null;

                this.binaryWriter.Write((byte)'E');
                this.binaryWriter.Write((byte)'N');
                this.binaryWriter.Write((byte)'C');
                this.binaryWriter.Write((byte)'O');
                this.binaryWriter.Write((byte)'G');
                this.binaryWriter.Write((byte)'-');
                this.binaryWriter.Write((byte)'0');
                this.binaryWriter.Write((byte)'0');

                this.binaryWriter.Write((double)input.Length);
                this.binaryWriter.Write((double)ideal.Length);

                this.numberOfRecords = 0;
                this.recordCount = this.inputCount + this.idealCount;
                this.recordSize = this.recordCount * EncogEGBFile.DOUBLE_SIZE;
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
                this.stream = new FileStream(this.file, FileMode.Open, FileAccess.Read);
                this.binaryReader = new BinaryReader(this.stream);
                this.binaryWriter = null;

                bool isEncogFile = true;

                isEncogFile = isEncogFile ? this.binaryReader.ReadByte() == 'E' : false;
                isEncogFile = isEncogFile ? this.binaryReader.ReadByte() == 'N' : false;
                isEncogFile = isEncogFile ? this.binaryReader.ReadByte() == 'C' : false;
                isEncogFile = isEncogFile ? this.binaryReader.ReadByte() == 'O' : false;
                isEncogFile = isEncogFile ? this.binaryReader.ReadByte() == 'G' : false;
                isEncogFile = isEncogFile ? this.binaryReader.ReadByte() == '-' : false;

                if (!isEncogFile)
                {
                    throw new BufferedDataError(
                            "File is not a valid Encog binary file:"
                                    + this.file);
                }

                char v1 = (char)this.binaryReader.ReadByte();
                char v2 = (char)this.binaryReader.ReadByte();
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
                catch (Exception ex)
                {
                    throw new BufferedDataError("File has invalid version number.");
                }

                this.inputCount = (int)this.binaryReader.ReadDouble();
                this.idealCount = (int)this.binaryReader.ReadDouble();

                this.recordCount = this.inputCount + this.idealCount;
                this.recordSize = this.recordCount * EncogEGBFile.DOUBLE_SIZE;
                this.numberOfRecords = (int)((this.stream.Length - EncogEGBFile.HEADER_SIZE) / this.recordSize);

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
                if (this.binaryWriter != null)
                {
                    this.binaryWriter.Close();
                    this.binaryWriter = null;
                }

                if (this.binaryReader != null)
                {
                    this.binaryReader.Close();
                    this.binaryReader = null;
                }

                if (this.stream != null)
                {
                    this.stream.Close();
                    this.stream = null;
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
            return EncogEGBFile.HEADER_SIZE + (row * this.recordSize);
        }
        
        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row, or record, to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>THe value read.</returns>
        private int CalculateIndex(int row, int col)
        {
            return EncogEGBFile.HEADER_SIZE + (row * this.recordSize)
                    + (col * EncogEGBFile.DOUBLE_SIZE);
        }

        /// <summary>
        /// Set the current location to the specified row. 
        /// </summary>
        /// <param name="row">The row.</param>
        public void SetLocation(int row)
        {
            try
            {
                this.stream.Position = CalculateIndex(row);
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
                this.stream.Position = CalculateIndex(row, col);
                this.binaryWriter.Write(v);
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
                this.stream.Position = CalculateIndex(row, 0);
                for (int i = 0; i < v.Length; i++)
                {
                    this.binaryWriter.Write(v[i]);
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
                    this.binaryWriter.Write(v[i]);
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
                this.binaryWriter.Write(b);
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
                this.stream.Position = CalculateIndex(row, col);
                return this.binaryReader.ReadDouble();
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
                this.stream.Position = CalculateIndex(row, 0);

                for (int i = 0; i < recordCount; i++)
                {
                    d[i] = this.binaryReader.ReadDouble();
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
                    d[i] = this.binaryReader.ReadDouble();
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
                return this.binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get
            {
                return inputCount;
            }
        }

        /// <summary>
        /// The ideal count.
        /// </summary>
        public int IdealCount
        {
            get
            {
                return idealCount;
            }
        }

        /// <summary>
        /// The stream.
        /// </summary>
        public FileStream Stream
        {
            get
            {
                return this.stream;
            }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get
            {
                return recordCount;
            }
        }

        /// <summary>
        /// The record size.
        /// </summary>
        public int RecordSize
        {
            get
            {
                return recordSize;
            }
        }

        /// <summary>
        /// The number of records.
        /// </summary>
        public int NumberOfRecords
        {
            get
            {
                return numberOfRecords;
            }
        }

    }
}
