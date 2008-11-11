using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Encog.Neural.NeuralData
{
    public interface INeuralDataSet
    {
        int IdealSize
        {
            get;
        }

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

        IEnumerator<INeuralDataPair> GetEnumerator();
        
    }
}
