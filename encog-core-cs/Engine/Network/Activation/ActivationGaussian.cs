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
using Encog.MathUtil;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// An activation function based on the gaussian function.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
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
        private readonly double[] paras;

        /// <summary>
        /// Create a gaussian activation function.
        /// </summary>
        ///
        /// <param name="center">The center of the curve.</param>
        /// <param name="peak">The peak of the curve.</param>
        /// <param name="width">The width of the curve.</param>
        public ActivationGaussian(double center, double peak,
                                  double width)
        {
            paras = new double[3];
            paras[PARAM_GAUSSIAN_CENTER] = center;
            paras[PARAM_GAUSSIAN_PEAK] = peak;
            paras[PARAM_GAUSSIAN_WIDTH] = width;
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationGaussian(Center, Peak,
                                          Width);
        }


        /// <summary>
        /// The width of the function.
        /// </summary>
        private double Width
        {
            get { return Params[PARAM_GAUSSIAN_WIDTH]; }
        }


        /// <summary>
        /// The center of the function.
        /// </summary>
        private double Center
        {
            get { return Params[PARAM_GAUSSIAN_CENTER]; }
        }


        /// <summary>
        /// The peak of the function.
        /// </summary>
        private double Peak
        {
            get { return Params[PARAM_GAUSSIAN_PEAK]; }
        }


        /// <summary>
        /// Return true, gaussian has a derivative.
        /// </summary>
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
                x[i] = paras[PARAM_GAUSSIAN_PEAK]
                       *BoundMath
                            .Exp(-Math.Pow(x[i]
                                           - paras[PARAM_GAUSSIAN_CENTER], 2)
                                 /(2.0d*paras[PARAM_GAUSSIAN_WIDTH]*paras[PARAM_GAUSSIAN_WIDTH]));
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            double width = paras[PARAM_GAUSSIAN_WIDTH];
            double peak = paras[PARAM_GAUSSIAN_PEAK];
            return Math.Exp(-0.5d*width*width*x*x)*peak*width*width
                   *(width*width*x*x - 1);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {"center", "peak", "width"};
                return result;
            }
        }


        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        ///
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
            return null;
        }
    }
}