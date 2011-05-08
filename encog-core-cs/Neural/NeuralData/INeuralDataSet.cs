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
        long Count { get; }

        /// <summary>
        /// Get one record from the data set.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to read into.</param>
        void GetRecord(long index, INeuralDataPair pair);

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        INeuralDataSet OpenAdditional();       

        /// <summary>
        /// Return true if supervised.
        /// </summary>
        bool Supervised { get; }

    }
}
