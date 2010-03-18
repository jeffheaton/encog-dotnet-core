// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Persist.Persistors.Generic;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// Holds basic functionality that all activation functions will likely have use
    /// of. Specifically it implements a name and description for the
    /// EncogPersistedObject class.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
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
        /// Create a persistor.  At this level we will use a generic persistor.
        /// </summary>
        /// <returns>The persistor.</returns>
        public virtual IPersistor CreatePersistor()
        {
            return new GenericPersistor(this.GetType());
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
