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

    /// <summary>
    /// The Linear layer is really not an activation function at all. The input is
    /// simply passed on, unmodified, to the output. This activation function is
    /// primarily theoretical and of little actual use. Usually an activation
    /// function that scales between 0 and 1 or -1 and 1 should be used.
    /// </summary>
    [Serializable]
    public class ActivationLinear : IActivationFunction
    {

        /// <summary>
        /// The offset to the parameter that holds the linear slope.
        /// </summary>
        ///
        public const int PARAM_LINEAR_SLOPE = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct a linear activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationLinear()
        {
            this.paras = new double[1];
            this.paras[ActivationLinear.PARAM_LINEAR_SLOPE] = 1;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual object Clone()
        {
            return new ActivationLinear();
        }


        /// <returns>Return true, linear has a 1 derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }


        /// <returns>The slope of the activation function.</returns>
        public double Slope
        {

            /// <returns>The slope of the activation function.</returns>
            get
            {
                return this.paras[ActivationLinear.PARAM_LINEAR_SLOPE];
            }
        }


        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = x[i] * paras[0];
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double d)
        {
            return 1;
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
                return "(1.0)";
            }
            else
            {
                return "(slope * x)";
            }
        }
    }
}
