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
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Anneal
{
    /// <summary>
    /// A simulated annealing implementation that trains from a training set.
    /// </summary>
    public class NeuralTrainingSetSimulatedAnnealing : NeuralSimulatedAnnealing
    {
        /// <summary>
        /// Construct a simulated annealing object that works with a training set.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles per iteration.</param>
        public NeuralTrainingSetSimulatedAnnealing(BasicNetwork network,
        INeuralDataSet training, double startTemp, double stopTemp,
        int cycles)
            : base(network, startTemp, stopTemp, cycles)
        {
            this.Training = training;
        }

        /// <summary>
        /// Determine the error of the current weights and thresholds.
        /// </summary>
        /// <returns>The error.</returns>
        public override double DetermineError()
        {
            return this.Network.CalculateError(this.Training);
        }

    }
}
