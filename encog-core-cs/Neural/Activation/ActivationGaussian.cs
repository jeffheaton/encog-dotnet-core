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
using Encog.MathUtil.RBF;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// An activation function based on the gaussian function.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationGaussian : BasicActivationFunction
    {
        /// <summary>
        /// The gaussian function to be used.
        /// </summary>
        private GaussianFunction gausian;

        /// <summary>
        /// Create a gaussian activation function.
        /// </summary>
        /// <param name="center">The center of the curve.</param>
        /// <param name="peak">The peak of the curve.</param>
        /// <param name="width">The width of the curve.</param>
        public ActivationGaussian(double center, double peak,
                 double width)
        {
            this.gausian = new GaussianFunction(center, peak, width);
        }

        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void ActivationFunction(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = this.gausian.Calculate(d[i]);
            }

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object cloned.</returns>
        public override Object Clone()
        {
            return new ActivationGaussian(this.gausian.Center, this.gausian
                    .Peak, this.gausian.Width);
        }

        /// <summary>
        /// Create a Persistor for this activation function.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new ActivationGaussianPersistor();
        }

        /// <summary>
        /// Implements the activation function derivative.  The array is modified 
        /// according derivative of the activation function being used.  See the 
        /// class description for more specific information on this type of 
        /// activation function. Propagation training requires the derivative. 
        /// Some activation functions do not support a derivative and will throw
        /// an error.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void DerivativeFunction(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = this.gausian.CalculateDerivative(d[i]);
            }

        }

        /// <summary>
        /// The gaussian funcion used.
        /// </summary>
        public GaussianFunction Gausian
        {
            get
            {
                return this.gausian;
            }
        }

        /// <summary>
        /// Return true, gaussian has a derivative.
        /// </summary>
        public override bool HasDerivative
        {
            get
            {
                return true;
            }
        }

    }

}
