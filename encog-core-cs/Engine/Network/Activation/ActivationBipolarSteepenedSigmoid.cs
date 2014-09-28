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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The bipolar sigmoid activation function is like the regular sigmoid activation function,
    /// except Bipolar sigmoid activation function. TheOutput range is -1 to 1 instead of the more normal 0 to 1.
    /// 
    /// This activation is typically part of a CPPN neural network, such as 
    /// HyperNEAT.
    /// 
    /// It was developed by  Ken Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class ActivationBipolarSteepenedSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private double[] _params;

        /// <inheritdoc/>
        public void ActivationFunction(double[] d, int start, int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (d[i] < -1.0)
                {
                    d[i] = -1.0;
                }
                if (d[i] > 1.0)
                {
                    d[i] = 1.0;
                }
            }
        }

        /// <inheritdoc/>
        public double DerivativeFunction(double b, double a)
        {
            return 1;
        }

        /// <inheritdoc/>
        public bool HasDerivative
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public Object Clone()
        {
            return new ActivationBipolarSteepenedSigmoid();
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

        /// <inheritdoc/>
        public string FactoryCode
        {
            get
            {
                return null;
            }
        }
    }
}
