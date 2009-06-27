// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using Encog.Persist;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// Holds basic functionality that all activation functions will likely have use
    /// of. Specifically it implements a name and description for the
    /// EncogPersistedObject class.
    /// </summary>
    [Serializable]
    public abstract class BasicActivationFunction : IActivationFunction
    {
        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual Object Clone()
        {
            return null;
        }

        /// <summary>
        /// Create a persistor.  Not implemented at this level.
        /// </summary>
        /// <returns>The persistor.</returns>
        public virtual IPersistor CreatePersistor()
        {
            return null;
        }

        /// <summary>
        /// Always returns null, descriptions and names are not used
        /// for activation functions.
        /// </summary>
        public String Description
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Always returns null, descriptions and names are not used
        /// for activation functions.
        /// </summary>
        public String Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Return true if this function has a derivative.
        /// </summary>
        public abstract bool HasDerivative
        {
            get;
        }

        /// <summary>
        /// Implements the activation function derivative.  The array is modified 
        /// according derivative of the activation function being used.  See the 
        /// class description for more specific information on this type of 
        /// activation function. Propagation training requires the derivative. 
        /// Some activation functions do not support a derivative and will throw
        /// an error.
        /// </summary>
        /// <param name="d"></param>
        public abstract void DerivativeFunction(double[] d);

        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public abstract void ActivationFunction(double[] d);
    }
}
