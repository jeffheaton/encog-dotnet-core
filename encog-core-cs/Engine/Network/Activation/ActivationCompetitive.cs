/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

namespace Encog.Engine.Network.Activation
{

    using Encog.Engine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

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
        public const int PARAM_COMPETITIVE_MAX_WINNERS = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] paras;

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
        /// <param name="winners"/>The maximum number of winners that this function supports.</param>
        public ActivationCompetitive(int winners)
        {
            this.paras = new double[1];
            this.paras[ActivationCompetitive.PARAM_COMPETITIVE_MAX_WINNERS] = winners;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                int size)
        {
            bool[] winners = new bool[x.Length];
            double sumWinners = 0;

            // find the desired number of winners
            for (int i = 0; i < paras[0]; i++)
            {
                double maxFound = System.Double.NegativeInfinity;
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
            for (int i_0 = start; i_0 < start + size; i_0++)
            {
                if (winners[i_0])
                {
                    x[i_0] = x[i_0] / sumWinners;
                }
                else
                {
                    x[i_0] = 0.0d;
                }
            }

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public IActivationFunction Clone()
        {
            return new ActivationCompetitive(
                    (int)this.paras[ActivationCompetitive.PARAM_COMPETITIVE_MAX_WINNERS]);
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationCompetitive(
                    (int)this.paras[ActivationCompetitive.PARAM_COMPETITIVE_MAX_WINNERS]);
        }

        /// <summary>
        /// Implements the activation function. The array is modified according to
        /// the activation function being used. See the class description for more
        /// specific information on this type of activation function.
        /// </summary>
        ///
        /// <param name="d"/>The input array to the activation function.</param>
        /// <returns>The derivative.</returns>
        public virtual double DerivativeFunction(double d)
        {
            throw new EncogEngineError(
                    "Can't use the competitive activation function "
                            + "where a derivative is required.");

        }


        /// <returns>The maximum number of winners this function supports.</returns>
        public int MaxWinners
        {

            /// <returns>The maximum number of winners this function supports.</returns>
            get
            {
                return (int)this.paras[ActivationCompetitive.PARAM_COMPETITIVE_MAX_WINNERS];
            }
        }


        /// <inheritdoc />
        public virtual String[] ParamNames
        {          
            get
            {
                String[] result = { "maxWinners" };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get
            {
                return this.paras;
            }
        }



        /// <returns>False, indication that no derivative is available for thisfunction.</returns>
        public virtual bool HasDerivative()
        {
            return false;
        }

        /// <inheritdoc />
        public virtual void SetParam(int index, double value_ren)
        {
            this.paras[index] = value_ren;
        }

        /// <inheritdoc />
        public virtual String GetOpenCLExpression(bool derivative)
        {
            return null;
        }
    }
}
