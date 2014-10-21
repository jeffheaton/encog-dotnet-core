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

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// An activation function based on the sin function, with a double period.
    /// 
    /// This activation is typically part of a CPPN neural network, such as 
    /// HyperNEAT.
    /// 
    /// It was developed by  Ken Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class ActivationSIN : IActivationFunction
    {
        /// <summary>
        /// Construct the sin activation function.
        /// </summary>
        ///
        public ActivationSIN()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSIN();
        }


        /// <returns>Return true, sin has a derivative.</returns>
        public virtual bool HasDerivative
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = BoundMath.Sin(2.0 * x[i]);
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return BoundMath.Cos(2.0 * b);
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
            get { return _paras; }
        }
    }
}
