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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Generate random choices unevenly.  This class is used to select random 
    /// choices from a list, with a probability weight places on each item 
    /// in the list.  
    /// 
    /// This is often called a Roulette Wheel in Machine Learning texts.  How it differs from
    /// a Roulette Wheel that you might find in Las Vegas or Monte Carlo is that the
    /// areas that can be selected are not of uniform size.  However, you can be sure
    /// that one will be picked. 
    /// 
    /// http://en.wikipedia.org/wiki/Fitness_proportionate_selection
    /// </summary>
    [Serializable]
    public class RandomChoice
    {
        /// <summary>
        /// The probabilities of each item in the list.
        /// </summary>
        private double[] probabilities;

        /// <summary>
        /// Construct a list of probabilities.
        /// </summary>
        /// <param name="theProbabilities">The probability of each item in the list.</param>
        public RandomChoice(double[] theProbabilities)
        {
            this.probabilities = EngineArray.ArrayCopy(theProbabilities);

            double total = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                total += probabilities[i];
            }

            if (total == 0.0)
            {
                double prob = 1.0 / probabilities.Length;
                for (int i = 0; i < probabilities.Length; i++)
                {
                    probabilities[i] = prob;
                }
            }
            else
            {
                double total2 = 0;
                double factor = 1.0 / total;
                for (int i = 0; i < probabilities.Length; i++)
                {
                    probabilities[i] = probabilities[i] * factor;
                    total2 += probabilities[i];
                }

                if (Math.Abs(1.0 - total2) > 0.02)
                {
                    double prob = 1.0 / probabilities.Length;
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        probabilities[i] = prob;
                    }
                }
            }
        }

        /// <summary>
        /// Generate a random choice, based on the probabilities provided to the constructor.
        /// </summary>
        /// <param name="theGenerator"></param>
        /// <returns>The random choice.</returns>
        public int Generate(Random theGenerator)
        {
            double r = theGenerator.NextDouble();
            double sum = 0.0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                sum += probabilities[i];
                if (r < sum)
                {
                    return i;
                }
            }

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] != 0.0)
                {
                    return i;
                }
            }

            throw new EncogError("Invalid probabilities.");
        }

        /// <summary>
        /// Generate a random choice, but skip one of the choices.
        /// </summary>
        /// <param name="theGenerator">The choice to skip.</param>
        /// <param name="skip">The random choice.</param>
        /// <returns></returns>
        public int Generate(Random theGenerator, int skip)
        {
            double totalProb = 1.0 - probabilities[skip];

            double throwValue = theGenerator.NextDouble() * totalProb;
            double accumulator = 0.0;

            for (int i = 0; i < skip; i++)
            {
                accumulator += probabilities[i];
                if (accumulator > throwValue)
                {
                    return i;
                }
            }

            for (int i = skip + 1; i < probabilities.Length; i++)
            {
                accumulator += probabilities[i];
                if (accumulator > throwValue)
                {
                    return i;
                }
            }

            for (int i = 0; i < skip; i++)
            {
                if (probabilities[i] != 0.0)
                {
                    return i;
                }
            }
            for (int i = skip + 1; i < probabilities.Length; i++)
            {
                if (probabilities[i] != 0.0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
