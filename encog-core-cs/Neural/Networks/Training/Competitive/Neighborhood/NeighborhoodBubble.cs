// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    /// <summary>
    /// A neighborhood function that uses a simple bubble. A width is defined, and
 /// any neuron that is plus or minus that width from the winning neuron will be
 /// updated as a result of training.
    /// </summary>
    public class NeighborhoodBubble : INeighborhoodFunction
    {

        /// <summary>
        /// The width of the bubble.
        /// </summary>
        private int width;

        /// <summary>
        /// Create a bubble neighborhood function that will return 1.0 (full update)
         /// for any neuron that is plus or minus the width distance from the winning
         /// neuron.
        /// </summary>
        /// <param name="width">The width of the bubble, this is the distance that the neuron
        /// can be from the winning neuron. The true width, across the
        /// bubble, is actually two times this parameter.</param>
        public NeighborhoodBubble(int width)
        {
            this.width = width;
        }

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        /// <param name="currentNeuron">THe current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public double Function(int currentNeuron, int bestNeuron)
        {
            int distance = Math.Abs(bestNeuron - currentNeuron);
            if (distance <= this.width)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }


        /// <summary>
        /// The radius of the bubble.
        /// </summary>
        public double Radius
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = (int)value;
            }
        }
    }
}
