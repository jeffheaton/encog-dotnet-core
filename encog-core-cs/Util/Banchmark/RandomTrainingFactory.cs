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

                INeuralData idealData = new BasicNeuralData(inputCount);

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
