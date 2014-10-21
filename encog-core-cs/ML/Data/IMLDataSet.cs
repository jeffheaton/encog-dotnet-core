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
using System.Collections.Generic;

namespace Encog.ML.Data
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
    public interface IMLDataSet
    {
        /// <summary>
        /// The size of the ideal data, 0 if no ideal data.
        /// </summary>
        int IdealSize { get; }

        /// <summary>
        /// The size of the input data.
        /// </summary>
        int InputSize { get; }

        /// <summary>
        /// The number of records in the data set.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Return true if supervised.
        /// </summary>
        bool Supervised { get; }

        /// <summary>
        /// Close this datasource and release any resources obtained by it, including
        /// any iterators created. 
        /// </summary>
        void Close();

        /// <summary>
        /// Get an enumerator to access the data.
        /// </summary>
        /// <returns></returns>
        IEnumerator<IMLDataPair> GetEnumerator();

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        IMLDataSet OpenAdditional();

        /// <summary>
        /// Get the specified record.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        IMLDataPair this[int x] { get; }
    }

	public interface IMLDataSetAddable: IMLDataSet
	{
		/// <summary>
		/// Add a NeuralData object to the dataset. This is used with unsupervised
		/// training, as no ideal output is provided. Note: not all implemenations
		/// support the add methods. 
		/// </summary>
		/// <param name="data1">The data to add.</param>
		void Add(IMLData data1);

		/// <summary>
		/// Add a set of input and ideal data to the dataset. This is used with
		/// supervised training, as ideal output is provided. Note: not all
		/// implementations support the add methods.
		/// </summary>
		/// <param name="inputData">Input data.</param>
		/// <param name="idealData">Ideal data.</param>
		void Add(IMLData inputData, IMLData idealData);

		/// <summary>
		/// Add a NeuralData object to the dataset. This is used with unsupervised
		/// training, as no ideal output is provided. Note: not all implementations
		/// support the add methods. 
		/// </summary>
		/// <param name="inputData">A NeuralDataPair object that contains both input and ideal
		/// data.</param>
		void Add(IMLDataPair inputData);

	}
}
