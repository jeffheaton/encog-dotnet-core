
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData
{
    /// <summary>
    /// An interface designed to abstract classes that store neural data. This
    /// interface is designed to provide NeuralDataPair objects. This can be used to
    /// train neural networks using both supervised and unsupervised training.
    /// 
    /// Some implementations of this interface are memory based. That is they store
    /// the entire contents of the dataset in memory.
    /// 
    /// Other implementations of this interface are not memory based. These
    /// implementations read in data as it is needed. This allows very large datasets
    /// to be used. Typically the add methods are not supported on non-memory based
    /// datasets.
    /// </summary>
    public interface INeuralDataSet
    {
        /// <summary>
        /// The size of the ideal data.  Zero if unsupervised.
        /// </summary>
        int IdealSize
        {
            get;
        }

        /// <summary>
        /// The size of the input data.
        /// </summary>
        int InputSize
        {
            get;
        }


        /// <summary>
        /// Add a NeuralData object to the dataset. This is used with unsupervised
        /// training, as no ideal output is provided. Note: not all implemenations
        /// support the add methods. 
        /// </summary>
        /// <param name="data1">The data to add.</param>
        void Add(INeuralData data1);

        /// <summary>
        /// Add a set of input and ideal data to the dataset. This is used with
        /// supervised training, as ideal output is provided. Note: not all
        /// implementations support the add methods.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idealData">Ideal data.</param>
        void Add(INeuralData inputData, INeuralData idealData);

        /// <summary>
        /// Add a NeuralData object to the dataset. This is used with unsupervised
        /// training, as no ideal output is provided. Note: not all implementations
        /// support the add methods. 
        /// </summary>
        /// <param name="inputData">A NeuralDataPair object that contains both input and ideal
        /// data.</param>
        void Add(INeuralDataPair inputData);


        /// <summary>
        /// Close this datasource and release any resources obtained by it, including
        /// any iterators created. 
        /// </summary>
        void Close();

        /// <summary>
        /// Get an enumerator to access the data.
        /// </summary>
        /// <returns></returns>
        IEnumerator<INeuralDataPair> GetEnumerator();

    }
}
