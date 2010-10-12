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
using Encog.Persist;

namespace Encog.Neural.Data.Buffer
{
    /// <summary>
    /// This class is not memory based, so very long files can be used, without
    /// running out of memory. This dataset uses a Encog binary training file as a
    /// buffer.
    /// 
    /// When used with a slower access dataset, such as CSV, XML or SQL, where
    /// parsing must occur, this dataset can be used to load from the slower dataset
    /// and train at much higher speeds.
    /// 
    /// If you are going to create a binary file, by using the add methods, you must
    /// call beginLoad to cause Encog to open an output file. Once the data has been
    /// loaded, call endLoad. You can also use the BinaryDataLoader class, with a
    /// CODEC, to load many other popular external formats.
    /// 
    /// The binary files produced by this class are in the Encog binary training
    /// format, and can be used with any Encog platform. Encog binary files are
    /// stored using "little endian" numbers.
    /// </summary>
    public class BufferedNeuralDataSet : INeuralDataSet, IIndexable
    {
        /// <summary>
        /// Error message for ADD.
        /// </summary>
        public const String ERROR_ADD = "Add can only be used after calling beginLoad.";

        /// <summary>
        /// True, if we are in the process of loading.
        /// </summary>
        [NonSerialized]
        private bool loading;

        /// <summary>
        /// The file being used.
        /// </summary>
        private String file;

        /// <summary>
        /// The EGB file we are working wtih.
        /// </summary>
        [NonSerialized]
        private EncogEGBFile egb;

        /// <summary>
        /// Additional sets that were opened.
        /// </summary>
        [NonSerialized]
        private IList<BufferedNeuralDataSet> additional = new List<BufferedNeuralDataSet>();

        /// <summary>
        /// The owner.
        /// </summary>
        [NonSerialized]
        private BufferedNeuralDataSet owner;



        /// <summary>
        /// Construct a buffered dataset using the specified file. 
        /// </summary>
        /// <param name="binaryFile">The file to read/write binary data to/from.</param>
        public BufferedNeuralDataSet(String binaryFile)
        {
            this.file = binaryFile;
            this.egb = new EncogEGBFile(binaryFile);
            if (File.Exists(this.file))
            {
                this.egb.Open();
            }

        }


        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            if (this.loading)
            {
                throw new NeuralDataError(
                        "Can't create enumerator while loading, call endLoad first.");
            }
            BufferedNeuralDataSetEnumerator result = new BufferedNeuralDataSetEnumerator(this);
            return result;

        }


        /// <summary>
        /// Open the binary file for reading.
        /// </summary>
        public void Open()
        {
            this.egb.Open();
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public long Count
        {
            get
            {
                if (this.egb == null)
                {
                    return 0;
                }
                else
                {
                    return this.egb.NumberOfRecords;
                }
            }
        }

        /// <summary>
        /// Read an individual record. 
        /// </summary>
        /// <param name="index">The zero-based index. Specify 0 for the first record, 1 for
        /// the second, and so on.</param>
        /// <param name="pair">The data to read.</param>
        public void GetRecord(long index, INeuralDataPair pair)
        {
            double[] inputTarget = pair.Input.Data;
            double[] idealTarget = pair.Ideal.Data;

            this.egb.SetLocation((int)index);
            this.egb.Read(inputTarget);
            this.egb.Read(idealTarget);
        }

        /// <summary>
        /// Open an additional training set.
        /// </summary>
        /// <returns>An additional training set.</returns>
        public IIndexable OpenAdditional()
        {

            BufferedNeuralDataSet result = new BufferedNeuralDataSet(this.file);
            result.owner = this;
            this.additional.Add(result);
            return result;
        }

        /// <summary>
        /// Add only input data, for an unsupervised dataset. 
        /// </summary>
        /// <param name="data1">The data to be added.</param>
        public void Add(INeuralData data1)
        {
            if (!this.loading)
            {
                throw new NeuralDataError(BufferedNeuralDataSet.ERROR_ADD);
            }

            egb.Write(data1.Data);
        }


        /// <summary>
        /// Add both the input and ideal data. 
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public void Add(INeuralData inputData, INeuralData idealData)
        {

            if (!this.loading)
            {
                throw new NeuralDataError(BufferedNeuralDataSet.ERROR_ADD);
            }

            this.egb.Write(inputData.Data);
            this.egb.Write(idealData.Data);
        }

        /// <summary>
        /// Add a data pair of both input and ideal data. 
        /// </summary>
        /// <param name="pair">The pair to add.</param>
        public void Add(INeuralDataPair pair)
        {
            if (!this.loading)
            {
                throw new NeuralDataError(BufferedNeuralDataSet.ERROR_ADD);
            }

            this.egb.Write(pair.Input.Data);
            this.egb.Write(pair.Ideal.Data);

        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {

            Object[] obj = this.additional.ToArray();

            for (int i = 0; i < obj.Length; i++)
            {
                BufferedNeuralDataSet set = (BufferedNeuralDataSet)obj[i];
                set.Close();
            }

            this.additional.Clear();

            if (this.owner != null)
            {
                this.owner.RemoveAdditional(this);
            }

            this.egb.Close();
            this.egb = null;
        }

        /// <summary>
        /// The ideal data size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                if (this.egb == null)
                {
                    return 0;
                }
                else
                {
                    return this.egb.IdealCount;
                }
            }
        }

        /// <summary>
        /// The input data size.
        /// </summary>
        public int InputSize
        {
            get
            {
                if (this.egb == null)
                {
                    return 0;
                }
                else
                {
                    return this.egb.InputCount;
                }
            }
        }

        /// <summary>
        /// True if this dataset is supervised.
        /// </summary>
        public bool IsSupervised
        {
            get
            {
                if (this.egb == null)
                {
                    return false;
                }
                else
                {
                    return this.egb.IdealCount > 0;
                }
            }
        }


        /// <summary>
        /// Remove an additional dataset that was created. 
        /// </summary>
        /// <param name="child">The additional dataset to remove.</param>
        public void RemoveAdditional(BufferedNeuralDataSet child)
        {
            lock (this)
            {
                this.additional.Remove(child);
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
            this.egb.Create(inputSize, idealSize);
            this.loading = true;
        }

        /// <summary>
        /// This method should be called once all the data has been loaded. The
        /// underlying file will be closed. The binary fill will then be opened for
        /// reading.
        /// </summary>
        public void EndLoad()
        {
            if (!this.loading)
            {
                throw new BufferedDataError("Must call beginLoad, before endLoad.");
            }

            this.egb.Close();

            Open();

        }

        /// <summary>
        /// Create an Encog persistor for this object.
        /// </summary>
        /// <returns>An Encog persistor for this object.</returns>
        public IPersistor CreatePersistor()
        {
            //return new BufferedNeuralDataSetPersistor();
            return null;
        }

        /// <summary>
        /// The binary file used.
        /// </summary>
        public String BinaryFile
        {
            get
            {
                return this.file;
            }
        }

        /// <summary>
        /// The EGB file to use.
        /// </summary>
        public EncogEGBFile EGB
        {
            get
            {
                return this.egb;
            }
        }

        /// <summary>
        /// Load the binary dataset to memory.  Memory access is faster. 
        /// </summary>
        /// <returns>A memory dataset.</returns>
        public INeuralDataSet LoadToMemory()
        {
            BasicNeuralDataSet result = new BasicNeuralDataSet();

            foreach (INeuralDataPair pair in this)
            {
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Load the specified training set. 
        /// </summary>
        /// <param name="training">The training set to load.</param>
        public void Load(INeuralDataSet training)
        {
            BeginLoad(training.InputSize, training.IdealSize);
            foreach (INeuralDataPair pair in training)
            {
                Add(pair);
            }
            EndLoad();

        }

        /// <summary>
        /// The owner.  Set when create additional is used.
        /// </summary>
        public BufferedNeuralDataSet Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

    }
}
