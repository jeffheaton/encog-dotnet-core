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
using Encog.Util.Randomize;
using Encog.Neural.Data;

namespace Encog.Util.Banchmark
{
    /// <summary>
    /// Class used to generate random training sets.
    /// </summary>
    public class RandomTrainingFactory
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private RandomTrainingFactory()
        {

        }

        /// <summary>
        /// Generate a random training set.
        /// </summary>
        /// <param name="count">How many training items to generate.</param>
        /// <param name="inputCount">How many input numbers.</param>
        /// <param name="idealCount">How many ideal numbers.</param>
        /// <param name="min">The minimum random number.</param>
        /// <param name="max">The maximum random number.</param>
        /// <returns>The random training set.</returns>
        public static INeuralDataSet Generate(int count,
                 int inputCount,
                 int idealCount, double min, double max)
        {
            INeuralDataSet result = new BasicNeuralDataSet();
            for (int i = 0; i < count; i++)
            {
                INeuralData inputData = new BasicNeuralData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData.Data[j] = RangeRandomizer.Randomize(min, max);
                }

                INeuralData idealData = new BasicNeuralData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData.Data[j] = RangeRandomizer.Randomize(min, max);
                }

                BasicNeuralDataPair pair = new BasicNeuralDataPair(inputData,
                       idealData);
                result.Add(pair);

            }
            return result;
        }
    }

}
