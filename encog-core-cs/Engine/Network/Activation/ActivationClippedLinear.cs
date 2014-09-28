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
    /// Linear activation function that bounds the output to [-1,+1].  This
    /// activation is typically part of a CPPN neural network, such as 
    /// HyperNEAT.
    /// 
    /// The idea for this activation function was developed by  Ken Stanley, of  
    /// the University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public class ActivationClippedLinear : IActivationFunction
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
                d[i] = (2.0 / (1.0 + Math.Exp(-4.9 * d[i]))) - 1.0;
            }
        }

        /// <inheritdoc/>
        public double DerivativeFunction(double b, double a)
        {
            return 1;
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

        public bool HasDerivative { get { return true; } }
    }
}
