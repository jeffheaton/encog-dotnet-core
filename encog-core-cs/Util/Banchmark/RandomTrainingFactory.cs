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
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

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
            var rand =
                new LinearCongruentialGenerator(seed);

            var result = new BasicMLDataSet();
            for (int i = 0; i < count; i++)
            {
                var inputData = new BasicMLData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData[j] = rand.Range(min, max);
                }

                var idealData = new BasicMLData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData[j] = rand.Range(min, max);
                }

                var pair = new BasicMLDataPair(inputData,
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
        public static void Generate(IMLDataSetAddable training,
                                    long seed,
                                    int count,
                                    double min, double max)
        {
            var rand
                = new LinearCongruentialGenerator(seed);

            int inputCount = training.InputSize;
            int idealCount = training.IdealSize;

            for (int i = 0; i < count; i++)
            {
                var inputData = new BasicMLData(inputCount);

                for (int j = 0; j < inputCount; j++)
                {
                    inputData[j] = rand.Range(min, max);
                }

                var idealData = new BasicMLData(idealCount);

                for (int j = 0; j < idealCount; j++)
                {
                    idealData[j] = rand.Range(min, max);
                }

                var pair = new BasicMLDataPair(inputData,
                                               idealData);
                training.Add(pair);
            }
        }
    }
}
