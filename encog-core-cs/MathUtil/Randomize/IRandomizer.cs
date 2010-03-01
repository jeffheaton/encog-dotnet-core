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
using Encog.Neural.Networks;
using Encog.MathUtil.Matrices;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Defines the interface for a class that is capable of randomizing the weights
    /// and thresholds of a neural network.
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        double Randomize(double d);

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        void Randomize(double[] d);

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous
        /// values may be used, or they may be discarded, depending on the
        /// randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        void Randomize(double[][] d);


        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="m">A matrix to randomize.</param>
        void Randomize(Matrix m);

        /// <summary>
        /// Randomize the synapses and thresholds in the basic network based on an
        /// array, modify the array. Previous values may be used, or they may be
        /// discarded, depending on the randomizer.
        /// </summary>
        /// <param name="network">A network to randomize.</param>
        void Randomize(BasicNetwork network);

    }

}
