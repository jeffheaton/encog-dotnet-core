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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Encog.ML.Data.Basic;

namespace Encog.ML.Data.Buffer
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
    public class BufferedMLDataSet : IMLDataSet
    {
        /// <summary>
        /// Error message for ADD.
        /// </summary>
        public const String ERROR_ADD = "Add can only be used after calling beginLoad.";

        /// <summary>
        /// True, if we are in the process of loading.
        /// </summary>
#if !SILVERLIGHT
        [NonSerialized]
#endif
            private bool loading;

        /// <summary>
        /// The file being used.
        /// </summary>
        private readonly String file;

        /// <summary>
        /// The EGB file we are working wtih.
        /// </summary>
#if !SILVERLIGHT
        [NonSerialized]
#endif
            private EncogEGBFile egb;

        /// <summary>
        /// Additional sets that were opened.
        /// </summary>
#if !SILVERLIGHT
        [NonSerialized]
#endif
            private readonly IList<BufferedMLDataSet> additional = new List<BufferedMLDataSet>();

        /// <summary>
        /// The owner.
        /// </summary>
#if !SILVERLIGHT
        [NonSerialized]
#endif
            private BufferedMLDataSet _owner;


        /// <summary>
        /// Construct a buffered dataset using the specified file. 
        /// </summary>
        /// <param name="binaryFile">The file to read/write binary data to/from.</param>
        public BufferedMLDataSet(String binaryFile)
        {
            file = binaryFile;
            egb = new EncogEGBFile(binaryFile);
            if (File.Exists(file))
            {
                egb.Open();
            }
        }


        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            if (loading)
            {
                throw new IMLDataError(
                    "Can't create enumerator while loading, call EndLoad first.");
            }
            var result = new BufferedNeuralDataSetEnumerator(this);
            return result;
        }


        /// <summary>
        /// Open the binary file for reading.
        /// </summary>
        public void Open()
        {
            egb.Open();
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public long Count
        {
            get
            {
                return egb == null ? 0 : egb.NumberOfRecords;
            }
        }

        /// <summary>
        /// Read an individual record. 
        /// </summary>
        /// <param name="index">The zero-based index. Specify 0 for the first record, 1 for
        /// the second, and so on.</param>
        /// <param name="pair">The data to read.</param>
        public void GetRecord(long index, IMLDataPair pair)
        {
            double[] inputTarget = pair.InputArray;
            double[] idealTarget = pair.IdealArray;

            egb.SetLocation((int) index);
            egb.Read(inputTarget);
            egb.Read(idealTarget);
        }

        /// <summary>
        /// Open an additional training set.
        /// </summary>
        /// <returns>An additional training set.</returns>
        public IMLDataSet OpenAdditional()
        {
            var result = new BufferedMLDataSet(file) {_owner = this};
            additional.Add(result);
            return result;
        }

        /// <summary>
        /// Add only input data, for an unsupervised dataset. 
        /// </summary>
        /// <param name="data1">The data to be added.</param>
        public void Add(IMLData data1)
        {
            if (!loading)
            {
                throw new IMLDataError(ERROR_ADD);
            }

            egb.Write(data1.Data);
        }


        /// <summary>
        /// Add both the input and ideal data. 
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public void Add(IMLData inputData, IMLData idealData)
        {
            if (!loading)
            {
                throw new IMLDataError(ERROR_ADD);
            }

            egb.Write(inputData.Data);
            egb.Write(idealData.Data);
        }

        /// <summary>
        /// Add a data pair of both input and ideal data. 
        /// </summary>
        /// <param name="pair">The pair to add.</param>
        public void Add(IMLDataPair pair)
        {
            if (!loading)
            {
                throw new IMLDataError(ERROR_ADD);
            }

            egb.Write(pair.Input.Data);
            egb.Write(pair.Ideal.Data);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            Object[] obj = additional.ToArray();

            foreach (var set in obj.Cast<BufferedMLDataSet>())
            {
                set.Close();
            }

            additional.Clear();

            if (_owner != null)
            {
                _owner.RemoveAdditional(this);
            }

            egb.Close();
            egb = null;
        }

        /// <summary>
        /// The ideal data size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return egb == null ? 0 : egb.IdealCount;
            }
        }

        /// <summary>
        /// The input data size.
        /// </summary>
        public int InputSize
        {
            get
            {
                return egb == null ? 0 : egb.InputCount;
            }
        }

        /// <summary>
        /// True if this dataset is supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                if (egb == null)
                {
                    return false;
                }
                return egb.IdealCount > 0;
            }
        }


        /// <summary>
        /// Remove an additional dataset that was created. 
        /// </summary>
        /// <param name="child">The additional dataset to remove.</param>
        public void RemoveAdditional(BufferedMLDataSet child)
        {
            lock (this)
            {
                additional.Remove(child);
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
            egb.Create(inputSize, idealSize);
            loading = true;
        }

        /// <summary>
        /// This method should be called once all the data has been loaded. The
        /// underlying file will be closed. The binary fill will then be opened for
        /// reading.
        /// </summary>
        public void EndLoad()
        {
            if (!loading)
            {
                throw new BufferedDataError("Must call beginLoad, before endLoad.");
            }

            egb.Close();
            loading = false;

            Open();
        }

        /// <summary>
        /// The binary file used.
        /// </summary>
        public String BinaryFile
        {
            get { return file; }
        }

        /// <summary>
        /// The EGB file to use.
        /// </summary>
        public EncogEGBFile EGB
        {
            get { return egb; }
        }

        /// <summary>
        /// Load the binary dataset to memory.  Memory access is faster. 
        /// </summary>
        /// <returns>A memory dataset.</returns>
        public IMLDataSet LoadToMemory()
        {
            var result = new BasicMLDataSet();

            foreach (IMLDataPair pair in this)
            {
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Load the specified training set. 
        /// </summary>
        /// <param name="training">The training set to load.</param>
        public void Load(IMLDataSet training)
        {
            BeginLoad(training.InputSize, training.IdealSize);
            foreach (IMLDataPair pair in training)
            {
                Add(pair);
            }
            EndLoad();
        }

        /// <summary>
        /// The owner.  Set when create additional is used.
        /// </summary>
        public BufferedMLDataSet Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
    }
}
