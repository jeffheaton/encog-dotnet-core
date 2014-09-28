//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.MathUtil;
using Encog.ML.Factory;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// An activation function based on the Gaussian function. The output range is
    /// between 0 and 1. This activation function is used mainly for the HyperNeat
    /// implementation.
    /// 
    /// A derivative is provided, so this activation function can be used with
    /// propagation training.  However, its primary intended purpose is for
    /// HyperNeat.  The derivative was obtained with the R statistical package.
    /// 
    /// If you are looking to implement a RBF-based neural network, see the 
    /// RBFNetwork class.
    /// 
    /// The idea for this activation function was developed by  Ken Stanley, of  
    /// the University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class ActivationGaussian : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private double[] _params;

        /// <summary>
        /// Construct the activation function.
        /// </summary>
        public ActivationGaussian()
        {
            _params = new double[0];
        }

        /// <inheritdoc />
        public object Clone()
        {
            return new ActivationGaussian();
        }

        /**
         * @return Return true, gaussian has a derivative.
         */
        public bool HasDerivative
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start, int size)
        {

            for (int i = start; i < start + size; i++)
            {
                x[i] = BoundMath.Exp(-Math.Pow(2.5 * x[i], 2.0));
            }

        }

        /// <inheritdoc />
        public double DerivativeFunction(double b, double a)
        {
            return Math.Exp(Math.Pow(2.5 * b, 2.0) * 12.5 * b);
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
            get { return _params; }
        }

    }
}
