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
    /// An activation function based on the gaussian function.
    /// </summary>
    [Serializable]
    public class ActivationGaussian : IActivationFunction
    {

        /// <summary>
        /// The offset to the parameter that holds the width.
        /// </summary>
        ///
        public const int PARAM_GAUSSIAN_CENTER = 0;

        /// <summary>
        /// The offset to the parameter that holds the peak.
        /// </summary>
        ///
        public const int PARAM_GAUSSIAN_PEAK = 1;

        /// <summary>
        /// The offset to the parameter that holds the width.
        /// </summary>
        ///
        public const int PARAM_GAUSSIAN_WIDTH = 2;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private double[] paras;

        /// <summary>
        /// Create a gaussian activation function.
        /// </summary>
        ///
        /// <param name="center"/>The center of the curve.</param>
        /// <param name="peak"/>The peak of the curve.</param>
        /// <param name="width"/>The width of the curve.</param>
        public ActivationGaussian(double center, double peak,
                double width)
        {
            this.paras = new double[3];
            this.paras[ActivationGaussian.PARAM_GAUSSIAN_CENTER] = center;
            this.paras[ActivationGaussian.PARAM_GAUSSIAN_PEAK] = peak;
            this.paras[ActivationGaussian.PARAM_GAUSSIAN_WIDTH] = width;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual object Clone()
        {
            return new ActivationGaussian(this.Center, this.Peak,
        this.Width);

        }



        /// <returns>The width of the function.</returns>
        private double Width
        {

            /// <returns>The width of the function.</returns>
            get
            {
                return this.Params[ActivationGaussian.PARAM_GAUSSIAN_WIDTH];
            }
        }



        /// <returns>The center of the function.</returns>
        private double Center
        {

            /// <returns>The center of the function.</returns>
            get
            {
                return this.Params[ActivationGaussian.PARAM_GAUSSIAN_CENTER];
            }
        }



        /// <returns>The peak of the function.</returns>
        private double Peak
        {

            /// <returns>The peak of the function.</returns>
            get
            {
                return this.Params[ActivationGaussian.PARAM_GAUSSIAN_PEAK];
            }
        }



        /// <returns>Return true, gaussian has a derivative.</returns>
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
                x[i] = paras[ActivationGaussian.PARAM_GAUSSIAN_PEAK]
                        * BoundMath
                                .Exp(-Math.Pow(x[i]
                                                                            - paras[ActivationGaussian.PARAM_GAUSSIAN_CENTER], 2)
                                        / (2.0d * paras[ActivationGaussian.PARAM_GAUSSIAN_WIDTH] * paras[ActivationGaussian.PARAM_GAUSSIAN_WIDTH]));
            }

        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            double width = paras[ActivationGaussian.PARAM_GAUSSIAN_WIDTH];
            double peak = paras[ActivationGaussian.PARAM_GAUSSIAN_PEAK];
            return Math.Exp(-0.5d * width * width * x * x) * peak * width * width
                    * (width * width * x * x - 1);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { "center", "peak", "width" };
                return result;
            }
        }


        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        ///
        public virtual double[] Params
        {
            get
            {
                return paras;
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
