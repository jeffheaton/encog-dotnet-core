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

using System;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The Linear layer is really not an activation function at all. The input is
    /// simply passed on, unmodified, to the output. This activation function is
    /// primarily theoretical and of little actual use. Usually an activation
    /// function that scales between 0 and 1 or -1 and 1 should be used.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationLinear : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] paras;

        /// <summary>
        /// Construct a linear activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationLinear()
        {
            paras = new double[0];
        }


        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationLinear();
        }


        /// <returns>Return true, linear has a 1 derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = x[i];
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
                String[] result = {};
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return paras; }
        }


        /// <inheritdoc />
        public virtual void SetParam(int index, double value_ren)
        {
            paras[index] = value_ren;
        }

        /// <inheritdoc />
        public virtual String GetOpenCLExpression(bool derivative)
        {
            if (derivative)
            {
                return "(1.0)";
            }
            else
            {
                return "(x)";
            }
        }
    }
}