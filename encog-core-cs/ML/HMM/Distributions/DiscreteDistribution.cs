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
using Encog.Util;
using System;

namespace Encog.ML.HMM.Distributions
{
    /// <summary>
    /// A discrete distribution is a distribution with a finite set of states that it
    /// can be in.
    /// </summary>
    [Serializable]
    public class DiscreteDistribution : IStateDistribution
    {
        /// <summary>
        /// The probabilities of moving between states.
        /// </summary>
        private readonly double[][] _probabilities;


        /// <summary>
        /// Construct a discrete distribution with the specified probabilities.
        /// </summary>
        /// <param name="theProbabilities">The probabilities.</param>
        public DiscreteDistribution(double[][] theProbabilities)
        {
            if (theProbabilities.Length == 0)
            {
                throw new EncogError("Invalid empty array");
            }

            _probabilities = new double[theProbabilities.Length][];

            for (int i = 0; i < theProbabilities.Length; i++)
            {
                if (theProbabilities[i].Length == 0)
                {
                    throw new EncogError("Invalid empty array");
                }

                _probabilities[i] = new double[theProbabilities[i].Length];

                for (int j = 0; j < _probabilities[i].Length; j++)
                {
                    if ((_probabilities[i][j] = theProbabilities[i][j]) < 0.0)
                    {
                        throw new EncogError("Invalid probability.");
                    }
                }
            }
        }

        /// <summary>
        /// Construct a discrete distribution.
        /// </summary>
        /// <param name="cx">The count of each.</param>
        public DiscreteDistribution(int[] cx)
        {
            _probabilities = new double[cx.Length][];
            for (int i = 0; i < cx.Length; i++)
            {
                int c = cx[i];
                _probabilities[i] = new double[c];

                for (int j = 0; j < c; j++)
                {
                    _probabilities[i][j] = 1.0/c;
                }
            }
        }

        /// <summary>
        /// The state probabilities.
        /// </summary>
        public double[][] Probabilities
        {
            get { return _probabilities; }
        }

        #region IStateDistribution Members

        /// <summary>
        /// Fit this distribution to the specified data.
        /// </summary>
        /// <param name="co">THe data to fit to.</param>
        public void Fit(IMLDataSet co)
        {
            if (co.Count < 1)
            {
                throw new EncogError("Empty observation set");
            }

            for (int i = 0; i < _probabilities.Length; i++)
            {
                for (int j = 0; j < _probabilities[i].Length; j++)
                {
                    _probabilities[i][j] = 0.0;
                }

                foreach (IMLDataPair o in co)
                {
                    _probabilities[i][(int) o.Input[i]]++;
                }

                for (int j = 0; j < _probabilities[i].Length; j++)
                {
                    _probabilities[i][j] /= co.Count;
                }
            }
        }

        /// <summary>
        /// Fit this distribution to the specified data, with weights. 
        /// </summary>
        /// <param name="co">The data to fit to.</param>
        /// <param name="weights">The weights.</param>
        public void Fit(IMLDataSet co, double[] weights)
        {
            if ((co.Count < 1) || (co.Count != weights.Length))
            {
                throw new EncogError("Invalid weight size.");
            }

            for (int i = 0; i < _probabilities.Length; i++)
            {
                EngineArray.Fill(_probabilities[i], 0.0);

                int j = 0;
                foreach (IMLDataPair o in co)
                {
                    _probabilities[i][(int) o.Input[i]] += weights[j++];
                }
            }
        }


        /// <summary>
        /// Generate a random sequence.
        /// </summary>
        /// <returns>The random element.</returns>
        public IMLDataPair Generate()
        {
            var result = new BasicMLData(_probabilities.Length);

            for (int i = 0; i < _probabilities.Length; i++)
            {
                double rand = ThreadSafeRandom.NextDouble();

                result[i] = _probabilities[i].Length - 1;
                for (int j = 0; j < (_probabilities[i].Length - 1); j++)
                {
                    if ((rand -= _probabilities[i][j]) < 0.0)
                    {
                        result[i] = j;
                        break;
                    }
                }
            }

            return new BasicMLDataPair(result);
        }

        /// <summary>
        /// Determine the probability of the specified data pair. 
        /// </summary>
        /// <param name="o">THe data pair.</param>
        /// <returns>The probability.</returns>
        public double Probability(IMLDataPair o)
        {
            double result = 1;

            for (int i = 0; i < _probabilities.Length; i++)
            {
                if (o.Input[i] > (_probabilities[i].Length - 1))
                {
                    throw new EncogError("Wrong observation value");
                }
                result *= _probabilities[i][(int) o.Input[i]];
            }

            return result;
        }

        /// <summary>
        /// Clone.
        /// </summary>
        /// <returns>A clone of the distribution.</returns>
        IStateDistribution IStateDistribution.Clone()
        {
            return new DiscreteDistribution((double[][])_probabilities.Clone());
        }

        #endregion
    }
}
