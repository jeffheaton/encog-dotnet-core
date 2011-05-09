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

using Encog.MathUtil;

namespace Encog.Engine.Network.Activation
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;


    /// <summary>
    /// The sigmoid activation function takes on a sigmoidal shape. Only positive
    /// numbers are generated. Do not use this activation function if negative number
    /// output is desired.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct a basic sigmoid function, with a slope of 1.
        /// </summary>
        ///
        public ActivationSigmoid()
        {
            this.paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSigmoid();

        }

        /// <returns>True, sigmoid has a derivative.</returns>
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
                x[i] = 1.0d / (1.0d + BoundMath.Exp(-1 * x[i]));
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            return x * (1.0d - x);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] results = { };
                return results;
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
        public virtual String GetOpenCLExpression(bool derivative)
        {
            if (derivative)
            {
                return "(x * (1.0f - x))";
            }
            else
            {
                return "(1.0f / (1.0f + exp(-1 * x)))";
            }
        }
    }
}
