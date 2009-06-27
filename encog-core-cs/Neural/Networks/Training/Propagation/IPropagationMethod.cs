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

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Defines the specifics to one of the propagation methods. The individual ways
    /// that each of the propagation methods uses to modify the weight and] threshold
    /// matrix are defined here.
    /// </summary>
    public interface IPropagationMethod
    {
        /// <summary>
        /// Calculate the error between these two levels.
        /// </summary>
        /// <param name="output">The output to the "to level".</param>
        /// <param name="fromLevel">The from level.</param>
        /// <param name="toLevel">The target level.</param>
        void CalculateError(NeuralOutputHolder output,
                 PropagationLevel fromLevel,
                 PropagationLevel toLevel);

        /// <summary>
        /// Init with the specified propagation object.
        /// </summary>
        /// <param name="propagation">The propagation object that this method will
        /// be used with.</param>
        void Init(Propagation propagation);

        /// <summary>
        /// Apply the accumulated deltas and learn.
        /// </summary>
        void Learn();

    }

}
