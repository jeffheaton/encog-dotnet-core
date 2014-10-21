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
using System.Collections.Generic;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Freeform
{
    /// <summary>
    /// Specifies how the inputs to a neuron are to be summed.
    /// </summary>
    public interface IInputSummation
    {
        /// <summary>
        /// Add an input connection.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        void Add(IFreeformConnection connection);

        /// <summary>
        /// Perform the summation, and apply the activation function.
        /// </summary>
        /// <returns>The sum.</returns>
        double Calculate();

        /// <summary>
        /// The activation function
        /// </summary>
        IActivationFunction ActivationFunction { get; }

        /// <summary>
        /// The preactivation sum.
        /// </summary>
        double Sum { get; }

        /// <summary>
        /// The input connections.
        /// </summary>
        IList<IFreeformConnection> List { get; }
    }
}
