namespace Encog.Engine.Data
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Data is stored in an ArrayList. This class is memory based, so large enough
    /// datasets could cause memory issues. Many other dataset types extend this
    /// class.
    /// </summary>
    ///
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicEngineDataSet : IEngineIndexableSet
    {


        /// <summary>
        /// The data held by this object.
        /// </summary>
        ///
        private IList<IEngineData> data;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public BasicEngineDataSet()
        {
            this.data = new List<IEngineData>();
        }

        /// <summary>
        /// Construct a data set from an input and idea array.
        /// </summary>
        ///
        /// <param name="input">The input into the neural network for training.</param>
        /// <param name="ideal">The ideal output for training.</param>
        public BasicEngineDataSet(double[][] input, double[][] ideal)
        {
            this.data = new List<IEngineData>();
            if (ideal != null)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    double[] inputData = input[i];
                    double[] idealData = ideal[i];
                    this.Add(inputData, idealData);
                }
            }
            else
            {
                /* foreach */
                foreach (double[] element in input)
                {
                    double[] inputData_0 = element;
                    this.Add(inputData_0);
                }
            }
        }

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        ///
        /// <param name="data">The data to use.</param>
        public BasicEngineDataSet(IList<IEngineData> data)
        {
            this.data = data;
        }

        /// <summary>
        /// Add input to the training set with no expected output. This is used for
        /// unsupervised training.
        /// </summary>
        ///
        /// <param name="d">The input to be added to the training set.</param>
        public void Add(double[] d)
        {
            this.data.Add(new BasicEngineData(d));
        }

        /// <summary>
        /// Add input and expected output. This is used for supervised training.
        /// </summary>
        ///
        /// <param name="inputData">The input data to train on.</param>
        /// <param name="idealData">The ideal data to use for training.</param>
        public void Add(double[] inputData, double[] idealData)
        {
            IEngineData pair = new BasicEngineData(inputData, idealData);
            this.data.Add(pair);
        }

        /// <summary>
        /// Add a neural data pair to the list.
        /// </summary>
        ///
        /// <param name="inputData">A NeuralDataPair object that contains both input and idealdata.</param>
        public void Add(IEngineData inputData)
        {
            this.data.Add(inputData);
        }


        /// <summary>
        /// The data to set.
        /// </summary>
        public IList<IEngineData> Data
        {
            get
            {
                return this.data;
            }

            set
            {
                this.data = value;
            }
        }


        /// <summary>
        /// Get the size of the ideal dataset. This is obtained from the first item
        /// in the list.
        /// </summary>
        public virtual int IdealSize
        {
            get
            {
                if ((this.data.Count == 0))
                {
                    return 0;
                }
                IEngineData first = this.data[0];
                if (first.IdealArray == null)
                {
                    return 0;
                }

                return first.IdealArray.Length;
            }
        }


        /// <summary>
        /// Get the size of the input dataset. This is obtained from the first item
        /// in the list.
        /// </summary>
        public virtual int InputSize
        {
            get
            {
                if ((this.data.Count == 0))
                {
                    return 0;
                }
                IEngineData first = this.data[0];
                return first.InputArray.Length;
            }
        }


        /// <summary>
        /// Get a record by index into the specified pair.
        /// </summary>
        ///
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to hold the data.</param>
        public virtual void GetRecord(long index, IEngineData pair)
        {

            IEngineData source = this.data[(int)index];
            pair.InputArray = source.InputArray;
            if (pair.IdealArray != null)
            {
                pair.IdealArray = source.IdealArray;
            }

        }


        /// <summary>
        /// The total number of records in the file.
        /// </summary>
        public virtual long Count
        {         
            get
            {
                return this.data.Count;
            }
        }


        /// <summary>
        /// Determine if this neural data set is supervied. All of the pairs should
        /// be either supervised or not, so simply check the first pair. If the list
        /// is empty then assume unsupervised.
        /// </summary>
        ///
        /// <returns>True if supervised.</returns>
        public virtual bool Supervised
        {
            get
            {
                if (this.data.Count == 0)
                {
                    return false;
                }
                return this.data[0].Supervised;
            }
        }


        /// <summary>
        /// Create an additional data set. It will use the same list.
        /// </summary>
        ///
        /// <returns>The additional data set.</returns>
        public virtual IEngineIndexableSet OpenAdditional()
        {
            return new BasicEngineDataSet(this.data);
        }
    }
}
