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
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Freeform.Basic
{
    /// <summary>
    /// Provides a basic implementation of an input summation. The inputs are summed
    /// and applied to the activation function.
    /// </summary>
    [Serializable]
    public class BasicActivationSummation : IInputSummation
    {
        /// <summary>
        /// The inputs.
        /// </summary>
        private readonly IList<IFreeformConnection> _inputs = new List<IFreeformConnection>();

        /// <summary>
        /// The pre-activation summation.
        /// </summary>
        private double _sum;

        /// <summary>
        /// Construct the activation summation. 
        /// </summary>
        /// <param name="theActivationFunction">The activation function.</param>
        public BasicActivationSummation(
            IActivationFunction theActivationFunction)
        {
            ActivationFunction = theActivationFunction;
        }

        /// <inheritdoc/>
        public void Add(IFreeformConnection connection)
        {
            _inputs.Add(connection);
        }

        /// <inheritdoc/>
        public double Calculate()
        {
            var sumArray = new double[1];
            _sum = 0;

            // sum the input connections
            foreach (IFreeformConnection connection in _inputs)
            {
                connection.Source.PerformCalculation();
                _sum += connection.Weight
                        * connection.Source.Activation;
            }

            // perform the activation function
            sumArray[0] = _sum;
            ActivationFunction
                    .ActivationFunction(sumArray, 0, sumArray.Count());

            return sumArray[0];
        }

        /// <inheritdoc/>
        public IActivationFunction ActivationFunction { get; set; }

        /// <inheritdoc/>
        public double Sum
        {
            get
            {
                return _sum;
            }
        }

        /// <inheritdoc/>
        public IList<IFreeformConnection> List
        {
            get
            {
                return _inputs;
            }
        }

    }
}
