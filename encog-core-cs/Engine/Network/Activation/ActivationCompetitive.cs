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
using Encog.Neural;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// An activation function that only allows a specified number, usually one, of
    /// the out-bound connection to win. These connections will share in the sum of
    /// the output, whereas the other neurons will receive zero.
    /// This activation function can be useful for "winner take all" layers.
    /// </summary>
    [Serializable]
    public class ActivationCompetitive : IActivationFunction
    {
        /// <summary>
        /// The offset to the parameter that holds the max winners.
        /// </summary>
        ///
        public const int ParamCompetitiveMaxWinners = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Create a competitive activation function with one winner allowed.
        /// </summary>
        ///
        public ActivationCompetitive()
            : this(1)
        {
        }

        /// <summary>
        /// Create a competitive activation function with the specified maximum
        /// number of winners.
        /// </summary>
        ///
        /// <param name="winners">The maximum number of winners that this function supports.</param>
        public ActivationCompetitive(int winners)
        {
            _paras = new double[1];
            _paras[ParamCompetitiveMaxWinners] = winners;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            var winners = new bool[x.Length];
            double sumWinners = 0;

            // find the desired number of winners
            for (int i = 0; i < _paras[0]; i++)
            {
                double maxFound = Double.NegativeInfinity;
                int winner = -1;

                // find one winner
                for (int j = start; j < start + size; j++)
                {
                    if (!winners[j] && (x[j] > maxFound))
                    {
                        winner = j;
                        maxFound = x[j];
                    }
                }
                sumWinners += maxFound;
                winners[winner] = true;
            }

            // adjust weights for winners and non-winners
            for (int i = start; i < start + size; i++)
            {
                if (winners[i])
                {
                    x[i] = x[i]/sumWinners;
                }
                else
                {
                    x[i] = 0.0d;
                }
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationCompetitive(
                (int) _paras[ParamCompetitiveMaxWinners]);
        }

        /// <inheritdoc/>
        public virtual double DerivativeFunction(double b, double a)
        {
            throw new NeuralNetworkError(
                "Can't use the competitive activation function "
                + "where a derivative is required.");
        }


        /// <summary>
        /// The maximum number of winners this function supports.
        /// </summary>
        public int MaxWinners
        {
            get { return (int) _paras[ParamCompetitiveMaxWinners]; }
        }


        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {"maxWinners"};
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }


        /// <returns>False, indication that no derivative is available for thisfunction.</returns>
        public virtual bool HasDerivative
        {
            get
            {
                return false;
            }
        }
    }
}
