// Encog(tm) Artificial Intelligence Framework v2.5
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

namespace Encog.Neural.Networks.Prune
{
    /// <summary>
    /// Specifies the minimum and maximum neuron counts for a layer.
    /// </summary>
    public class HiddenLayerParams
    {
        /// <summary>
        /// The minimum number of neurons on this layer.
        /// </summary>
        private int min;

        /// <summary>
        /// The maximum number of neurons on this layer.
        /// </summary>
        private int max;


        /// <summary>
        /// Construct a hidden layer param object with the specified min and max
        /// values.
        /// </summary>
        /// <param name="min">The minimum number of neurons.</param>
        /// <param name="max">The maximum number of neurons.</param>
        public HiddenLayerParams(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        public int Max
        {
            get
            {
                return this.max;
            }
        }

        /// <summary>
        /// The minimum number of neurons.
        /// </summary>
        public int Min
        {
            get
            {
                return this.min;
            }
        }

    }

}
