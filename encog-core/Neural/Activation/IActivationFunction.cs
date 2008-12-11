// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// An activation function filters the input or output from a neuron.
    /// </summary>
    public interface IActivationFunction : IEncogPersistedObject
    {
        /// <summary>
        /// A activation function for a neural network. 
        /// </summary>
        /// <param name="d">The input to the function.</param>
        /// <returns>The output from the function.</returns>
        double ActivationFunction(double d);

        /// <summary>
        /// Performs the derivative of the activation function function on the input.
        /// </summary>
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        double DerivativeFunction(double d);
    }
}
