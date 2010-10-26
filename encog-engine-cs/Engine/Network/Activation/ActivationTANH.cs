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

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine.Util;


    /// <summary>
    /// The hyperbolic tangent activation function takes the curved shape of the
    /// hyperbolic tangent. This activation function produces both positive and
    /// negative output. Use this activation function if both negative and positive
    /// output is desired.
    /// </summary>
    [Serializable]
    public class ActivationTANH : IActivationFunction
    {

        /// <summary>
        /// The offset to the parameter that holds the tanh slope.
        /// </summary>
        ///
        public const int PARAM_TANH_SLOPE = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct a basic HTAN activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationTANH()
        {
            this.paras = new double[1];
            this.paras[ActivationTANH.PARAM_TANH_SLOPE] = 1;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual object Clone()
        {
            return new ActivationTANH();

        }


        /// <returns>Return true, TANH has a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }


        /// <returns>Get the slope of the activation function.</returns>
        public double Slope
        {

            /// <returns>Get the slope of the activation function.</returns>
            get
            {
                return this.paras[ActivationTANH.PARAM_TANH_SLOPE];
            }
        }


        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                int size)
        {
            for (int i = start; i < start + size; i++)
            {
                double z = BoundMath.Exp(-paras[0] * x[i]);
                x[i] = (1.0d - z) / (1.0d + z);
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            return (paras[0] * (1.0d - x * x));
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "slope" };
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


        /// <inheritdoc />
        public virtual void SetParam(int index, double value_ren)
        {
            this.paras[index] = value_ren;
        }

        /// <inheritdoc />
        public virtual String GetOpenCLExpression(bool derivative,
                bool allSlopeOne)
        {

            if (derivative)
            {
                return "(slope * (1.0f - x * x))";
            }
            else
            {
                if (allSlopeOne)
                {
                    return "tanh(x)";
                }
                else
                {
                    return "tanh(x)";
                }
            }
        }
    }
}
