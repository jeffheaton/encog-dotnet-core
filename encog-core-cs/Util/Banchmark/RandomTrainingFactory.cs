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
using Encog.Neural.Data.Basic;
using Encog.MathUtil.Randomize;
using Encog.Neural.Data;
using Encog.MathUtil;

namespace Encog.Util.Banchmark
{
    /// <summary>
    /// Class used to generate random training sets.
    /// </summary>
    public class RandomTrainingFactory
    {      
        /// <summary>
        /// Generate a random training set. 
        /// </summary>
        /// <param name="seed">The seed value to use, the same seed value will always produce
        /// the same results.</param>
        /// <param name="count">How many training items to generate.</param>
        /// <param name="inputCount">How many input numbers.</param>
        /// <param name="idealCount">How many ideal numbers.</param>
        /// <param name="min">The minimum random number.</param>
        /// <param name="max">The maximum random number.</param>
        /// <returns>The random training set.</returns>
        public static BasicMLDataSet Generate(long seed,
                int count, int inputCount,
                int idealCount, double min, double max)
        {

            LinearCongruentialGenerator rand =
                new LinearCongruentialGenerator(seed);

            BasicMLDataSet result = new BasicMLDataSet();
            for (int i = 0; i < count; i++)
            {
                MLData inputData = new BasicMLData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData.Data[j] = rand.Range(min, max);
                }

                MLData idealData = new BasicMLData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData[j] = rand.Range(min, max);
                }

                BasicMLDataPair pair = new BasicMLDataPair(inputData,
                        idealData);
                result.Add(pair);

            }
            return result;
        }

        /// <summary>
        /// Generate random training into a training set.
        /// </summary>
        /// <param name="training">The training set to generate into.</param>
        /// <param name="seed">The seed to use.</param>
        /// <param name="count">How much data to generate.</param>
        /// <param name="min">The low random value.</param>
        /// <param name="max">The high random value.</param>
        public static void Generate(MLDataSet training,
                long seed,
                int count,
                double min, double max)
        {

            LinearCongruentialGenerator rand
                = new LinearCongruentialGenerator(seed);

            int inputCount = training.InputSize;
            int idealCount = training.IdealSize;

            for (int i = 0; i < count; i++)
            {
                MLData inputData = new BasicMLData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData[j] = rand.Range(min, max);
                }

                MLData idealData = new BasicMLData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData[j] = rand.Range(min, max);
                }

                BasicMLDataPair pair = new BasicMLDataPair(inputData,
                        idealData);
                training.Add(pair);

            }
        }


        /// <summary>
        /// Private constructor.
        /// </summary>
        private RandomTrainingFactory()
        {

        }

    }

}
