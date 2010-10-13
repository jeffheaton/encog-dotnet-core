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
    /// A ramp activation function. This function has a high and low threshold. If
    /// the high threshold is exceeded a fixed value is returned. Likewise, if the
    /// low value is exceeded another fixed value is returned.
    /// </summary>
    ///
    public class ActivationRamp : IActivationFunction
    {

        /// <summary>
        /// The ramp high threshold parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_HIGH_THRESHOLD = 0;

        /// <summary>
        /// The ramp low threshold parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_LOW_THRESHOLD = 1;

        /// <summary>
        /// The ramp high parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_HIGH = 2;

        /// <summary>
        /// The ramp low parameter.
        /// </summary>
        ///
        public const int PARAM_RAMP_LOW = 3;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Construct a ramp activation function.
        /// </summary>
        ///
        /// <param name="thresholdHigh"/>The high threshold value.</param>
        /// <param name="thresholdLow"/>The low threshold value.</param>
        /// <param name="high"/>The high value, replaced if the high threshold is exceeded.</param>
        /// <param name="low"/>The low value, replaced if the low threshold is exceeded.</param>
        public ActivationRamp(double thresholdHigh,
                double thresholdLow, double high, double low)
        {

            this.paras = new double[4];
            this.paras[ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD] = thresholdHigh;
            this.paras[ActivationRamp.PARAM_RAMP_LOW_THRESHOLD] = thresholdLow;
            this.paras[ActivationRamp.PARAM_RAMP_HIGH] = high;
            this.paras[ActivationRamp.PARAM_RAMP_LOW] = low;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public ActivationRamp()
            : this(1, 0, 1, 0)
        {
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public IActivationFunction Clone()
        {
            return new ActivationRamp(
                    this.paras[ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD],
                    this.paras[ActivationRamp.PARAM_RAMP_LOW_THRESHOLD],
                    this.paras[ActivationRamp.PARAM_RAMP_HIGH],
                    this.paras[ActivationRamp.PARAM_RAMP_LOW]);

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        object ICloneable.Clone()
        {
            return new ActivationRamp(
                    this.paras[ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD],
                    this.paras[ActivationRamp.PARAM_RAMP_LOW_THRESHOLD],
                    this.paras[ActivationRamp.PARAM_RAMP_HIGH],
                    this.paras[ActivationRamp.PARAM_RAMP_LOW]);
        }


        /// <summary>
        /// Set the high value.
        /// </summary>
        ///
        /// <param name="d"/>The high value.</param>
        public double High
        {

            /// <returns>the high</returns>
            get
            {
                return this.paras[ActivationRamp.PARAM_RAMP_HIGH];
            }
            /// <summary>
            /// Set the high value.
            /// </summary>
            ///
            /// <param name="d"/>The high value.</param>
            set
            {
                this.SetParam(ActivationRamp.PARAM_RAMP_HIGH, value);

            }
        }


        /// <summary>
        /// Set the low value.
        /// </summary>
        ///
        /// <param name="d"/>The low value.</param>
        public double Low
        {

            /// <returns>the low</returns>
            get
            {
                return this.paras[ActivationRamp.PARAM_RAMP_LOW];
            }
            /// <summary>
            /// Set the low value.
            /// </summary>
            ///
            /// <param name="d"/>The low value.</param>
            set
            {
                this.SetParam(ActivationRamp.PARAM_RAMP_LOW, value);
            }
        }


        /// <summary>
        /// Set the threshold high.
        /// </summary>
        ///
        /// <param name="d"/>The threshold high.</param>
        public double ThresholdHigh
        {

            /// <returns>the thresholdHigh</returns>
            get
            {
                return this.paras[ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD];
            }
            /// <summary>
            /// Set the threshold high.
            /// </summary>
            ///
            /// <param name="d"/>The threshold high.</param>
            set
            {
                this.SetParam(ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD, value);
            }
        }


        /// <summary>
        /// Set the threshold low.
        /// </summary>
        ///
        /// <param name="d"/>The threshold low.</param>
        public double ThresholdLow
        {

            /// <returns>the thresholdLow</returns>
            get
            {
                return this.paras[ActivationRamp.PARAM_RAMP_LOW_THRESHOLD];
            }
            /// <summary>
            /// Set the threshold low.
            /// </summary>
            ///
            /// <param name="d"/>The threshold low.</param>
            set
            {
                this.SetParam(ActivationRamp.PARAM_RAMP_LOW_THRESHOLD, value);
            }
        }



        /// <returns>True, as this function does have a derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                int size)
        {
            double slope = (paras[ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD] - paras[ActivationRamp.PARAM_RAMP_LOW_THRESHOLD])
                    / (paras[ActivationRamp.PARAM_RAMP_HIGH] - paras[ActivationRamp.PARAM_RAMP_LOW]);

            for (int i = start; i < start + size; i++)
            {
                if (x[i] < paras[ActivationRamp.PARAM_RAMP_LOW_THRESHOLD])
                {
                    x[i] = paras[ActivationRamp.PARAM_RAMP_LOW];
                }
                else if (x[i] > paras[ActivationRamp.PARAM_RAMP_HIGH_THRESHOLD])
                {
                    x[i] = paras[ActivationRamp.PARAM_RAMP_HIGH];
                }
                else
                {
                    x[i] = (slope * x[i]);
                }
            }

        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double d)
        {
            // TODO Auto-generated method stub
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            /// <inheritdoc />
            get
            {
                String[] result = { "thresholdHigh", "thresholdLow", "high",
						"low" };
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
