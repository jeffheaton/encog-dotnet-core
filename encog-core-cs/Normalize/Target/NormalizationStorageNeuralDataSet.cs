// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;

namespace Encog.Normalize.Target
{
    /// <summary>
    /// Store the normalized data to a neural data set.
    /// </summary>
    public class NormalizationStorageNeuralDataSet : INormalizationStorage
    {
        /// <summary>
        /// The input count.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The ideal count.
        /// </summary>
        private int idealCount;

        /// <summary>
        /// The data set to add to.
        /// </summary>
        private INeuralDataSet dataset;

        /// <summary>
        /// Construct a new NeuralDataSet based on the parameters specified.
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The output count.</param>
        public NormalizationStorageNeuralDataSet(int inputCount,
                 int idealCount)
        {
            this.inputCount = inputCount;
            this.idealCount = idealCount;
            this.dataset = new BasicNeuralDataSet();
        }

        /// <summary>
        /// Construct a normalized neural storage class to hold data.
        /// </summary>
        /// <param name="dataset">The data set to store to. This uses an existing data set.</param>
        public NormalizationStorageNeuralDataSet(INeuralDataSet dataset)
        {
            this.dataset = dataset;
            this.inputCount = this.dataset.InputSize;
            this.idealCount = this.dataset.IdealSize;
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {

            if (this.idealCount == 0)
            {
                BasicNeuralData inputData = new BasicNeuralData(data);
                this.dataset.Add(inputData);
            }
            else
            {
                BasicNeuralData inputData = new BasicNeuralData(
                       this.inputCount);
                BasicNeuralData idealData = new BasicNeuralData(
                       this.idealCount);

                int index = 0;
                for (int i = 0; i < this.inputCount; i++)
                {
                    inputData[i] = data[index++];
                }

                for (int i = 0; i < this.idealCount; i++)
                {
                    idealData[i] = data[index++];
                }

                this.dataset.Add(inputData, idealData);
            }
        }
    }
}
