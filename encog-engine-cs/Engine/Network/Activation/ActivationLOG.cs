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
    /// An activation function based on the logarithm function.
    /// This type of activation function can be useful to prevent saturation. A
    /// hidden node of a neural network is said to be saturated on a given set of
    /// inputs when its output is approximately 1 or -1 "most of the time". If this
    /// phenomena occurs during training then the learning of the network can be
    /// slowed significantly since the error surface is very at in this instance.
    /// </summary>
    [Serializable]
    public class ActivationLOG : IActivationFunction
    {

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct the activation function.
        /// </summary>
        ///
        public ActivationLOG()
        {
            this.paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public IActivationFunction Clone()
        {
            return new ActivationLOG();

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationLOG();
        }


        /// <returns>Return true, log has a derivative.</returns>
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
                if (x[i] >= 0)
                {
                    x[i] = BoundMath.Log(1 + x[i]);
                }
                else
                {
                    x[i] = -BoundMath.Log(1 - x[i]);
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            if (x >= 0)
            {
                return 1 / (1 + x);
            }
            else
            {
                return 1 / (1 - x);
            }
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
