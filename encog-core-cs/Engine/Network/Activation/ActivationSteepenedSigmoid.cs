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
    /// The Steepened Sigmoid is an activation function typically used with NEAT.
    /// 
    /// Valid derivative calculated with the R package, so this does work with 
    /// non-NEAT networks too.
    /// 
    /// It was developed by  Ken Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class ActivationSteepenedSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private double[] _params;

        /// <summary>
        /// Construct a steepend sigmoid activation function.
        /// </summary>
        public ActivationSteepenedSigmoid()
        {
            _params = new double[0];
        }

        /// <inheritdoc/>
        public void ActivationFunction(double[] x, int start,
                int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = 1.0 / (1.0 + Math.Exp(-4.9 * x[i]));
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object cloned;</returns>
        public Object Clone()
        {
            return new ActivationSteepenedSigmoid();
        }

        /// <inheritdoc/>
        public double DerivativeFunction(double b, double a)
        {
            double s = Math.Exp(-4.9 * a);
            return Math.Pow(s * 4.9 / (1 + s), 2);
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

        /// <summary>
        /// Return true, steepend sigmoid has a derivative.
        /// </summary>
        public virtual bool HasDerivative
        {
            get
            {
                return true;
            }
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
