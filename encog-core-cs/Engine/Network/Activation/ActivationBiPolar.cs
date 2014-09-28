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

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// BiPolar activation function. This will scale the neural data into the bipolar
    /// range. Greater than zero becomes 1, less than or equal to zero becomes -1.
    /// </summary>
    [Serializable]
    public class ActivationBiPolar : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the bipolar activation function.
        /// </summary>
        ///
        public ActivationBiPolar()
        {
            _paras = new double[0];
        }

        /// <inheritdoc/>
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1;
        }


        /// <returns>Return true, bipolar has a 1 for derivative.</returns>
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
                if (x[i] > 0)
                {
                    x[i] = 1;
                }
                else
                {
                    x[i] = -1;
                }
            }
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {"slope"};
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }        

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationBiPolar();
        }
    }
}
