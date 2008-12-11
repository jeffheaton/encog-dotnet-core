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

namespace Encog.Neural.Activation
{
    /// <summary>
    /// Activation function for a TANH function.
    /// </summary>
    [Serializable]
    public class ActivationTANH : IActivationFunction
    {
        /// <summary>
        /// The description for this object.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private String description;
        private String name;

        /// <summary>
        /// A threshold function for a neural network.
        /// </summary>
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        public double ActivationFunction(double d)
        {
            double result = (Math.Exp(d * 2.0) - 1.0)
                    / (Math.Exp(d * 2.0) + 1.0);
            return result;
        }

        /// <summary>
        /// The derivative of the TANH.
        /// </summary>
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        public double DerivativeFunction(double d)
        {
            return 1.0 - Math.Pow(ActivationFunction(d), 2.0);
        }


    }
}
