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
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Propagation.Manhattan;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Train a flat network using Manhattan prop.
    /// </summary>
    public class TrainFlatNetworkManhattan:TrainFlatNetworkMulti
    {
        /// <summary>
        /// The zero tolearnce to use.
        /// </summary>
        public double ZeroTolerance { get; set; }

        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Construct a trainer for flat networks to use the Manhattan update rule.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learningRate">The learning rate to use.</param>
        public TrainFlatNetworkManhattan(
            FlatNetwork network,
            INeuralDataSet training,
            double learningRate):
            base(network,training)
        {
            LearningRate = learningRate;
            ZeroTolerance = Encog.DEFAULT_DOUBLE_EQUAL;
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override double UpdateWeight(double[] gradients, double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < this.ZeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return this.LearningRate;
            }
            else
            {
                return -this.LearningRate;
            }       
        }
    }
}
