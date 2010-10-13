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
    /// The softmax activation function.
    /// </summary>
    ///
    public class ActivationSoftMax : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct the soft-max activation function.
        /// </summary>
        ///
        public ActivationSoftMax()
        {
            this.paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public IActivationFunction Clone()
        {
            return new ActivationSoftMax();

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationSoftMax();
        }

        /// <returns>Return false, softmax has no derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                int size)
        {
            double sum = 0;
            for (int i = start; i < start + size; i++)
            {
                x[i] = BoundMath.Exp(x[i]);
                sum += x[i];
            }
            for (int i_0 = start; i_0 < start + size; i_0++)
            {
                x[i_0] = x[i_0] / sum;
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double d)
        {
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
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
            return null;
        }

    }
}
