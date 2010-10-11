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
    /// The step activation function is a very simple activation function. It is the
    /// activation function that was used by the original perceptron. Using the
    /// default parameters it will return 1 if the input is 0 or greater. Otherwise
    /// it will return 1.
    /// The center, low and high properties allow you to define how this activation
    /// function works. If the input is equal to center or higher the high property
    /// value will be returned, otherwise the low property will be returned. This
    /// activation function does not have a derivative, and can not be used with
    /// propagation training, or any other training that requires a derivative.
    /// </summary>
    ///
    public class ActivationStep : IActivationFunction
    {

        /// <summary>
        /// The step center parameter.
        /// </summary>
        ///
        public const int PARAM_STEP_CENTER = 0;

        /// <summary>
        /// The step low parameter.
        /// </summary>
        ///
        public const int PARAM_STEP_LOW = 1;

        /// <summary>
        /// The step high parameter.
        /// </summary>
        ///
        public const int PARAM_STEP_HIGH = 2;

        /// <summary>
        /// The serial ID.
        /// </summary>
        ///
        private const long serialVersionUID = 3416782010146745754L;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct a step activation function.
        /// </summary>
        ///
        /// <param name="low"/>The low of the function.</param>
        /// <param name="center"/>The center of the function.</param>
        /// <param name="high"/>The high of the function.</param>
        public ActivationStep(double low, double center,
                double high)
        {
            this.paras = new double[3];
            this.paras[ActivationStep.PARAM_STEP_CENTER] = center;
            this.paras[ActivationStep.PARAM_STEP_LOW] = low;
            this.paras[ActivationStep.PARAM_STEP_HIGH] = high;
        }

        /// <summary>
        /// Create a basic step activation with low=0, center=0, high=1.
        /// </summary>
        ///
        public ActivationStep()
            : this(0.0d, 0.0d, 1.0d)
        {
        }

        /// <summary>
        /// Set the center of this function.
        /// </summary>
        ///
        /// <param name="d"/>The center of this function.</param>
        public double Center
        {

            /// <returns>The center.</returns>
            get
            {
                return this.paras[ActivationStep.PARAM_STEP_CENTER];
            }
            /// <summary>
            /// Set the center of this function.
            /// </summary>
            ///
            /// <param name="d"/>The center of this function.</param>
            set
            {
                this.SetParam(ActivationStep.PARAM_STEP_CENTER, value);
            }
        }


        /// <summary>
        /// Set the low of this function.
        /// </summary>
        ///
        /// <param name="d"/>The low of this function.</param>
        public double Low
        {

            /// <returns>The low value.</returns>
            get
            {
                return this.paras[ActivationStep.PARAM_STEP_LOW];
            }
            /// <summary>
            /// Set the low of this function.
            /// </summary>
            ///
            /// <param name="d"/>The low of this function.</param>
            set
            {
                this.SetParam(ActivationStep.PARAM_STEP_LOW, value);
            }
        }


        /// <summary>
        /// Set the high of this function.
        /// </summary>
        ///
        /// <param name="d"/>The high of this function.</param>
        public double High
        {

            /// <returns>The high value.</returns>
            get
            {
                return this.paras[ActivationStep.PARAM_STEP_HIGH];
            }
            /// <summary>
            /// Set the high of this function.
            /// </summary>
            ///
            /// <param name="d"/>The high of this function.</param>
            set
            {
                this.SetParam(ActivationStep.PARAM_STEP_HIGH, value);
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public IActivationFunction Clone()
        {
            ActivationStep result = new ActivationStep(Low, Center,
                    High);
            return result;

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            ActivationStep result = new ActivationStep(Low, Center,
                    High);
            return result;
        }


        /// <returns>Returns true, this activation function has a derivative.</returns>
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
                if (x[i] >= paras[ActivationStep.PARAM_STEP_CENTER])
                {
                    x[i] = paras[ActivationStep.PARAM_STEP_HIGH];
                }
                else
                {
                    x[i] = paras[ActivationStep.PARAM_STEP_LOW];
                }
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
                String[] result = { "center", "low", "high" };
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
