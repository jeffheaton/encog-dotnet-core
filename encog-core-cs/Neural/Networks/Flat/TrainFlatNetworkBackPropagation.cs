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

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Train a flat network using backpropagation.
    /// </summary>
    public class TrainFlatNetworkBackPropagation:
        TrainFlatNetworkMulti
    {
        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// The momentum.
        /// </summary>
        public double Momentum { get; set; }

        /// <summary>
        /// The last delta values.
        /// </summary>
        private double[] lastDelta;

        /// <summary>
        /// Construct a backprop trainer for flat networks.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="learningRate">The learning rate.</param>
        /// <param name="momentum">The momentum.</param>
        public TrainFlatNetworkBackPropagation(
            FlatNetwork network,
            INeuralDataSet training,
            double learningRate,
            double momentum):
            base(network,training)
        {
            this.Momentum = momentum;
            this.LearningRate = learningRate;
            this.lastDelta = new double[network.Weights.Length];
        }

        /// <summary>
        /// Update a weight.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override double UpdateWeight(double[] gradients, double[] lastGradient, int index)
        {
            double delta = (gradients[index] * this.LearningRate) +
                (lastDelta[index] * this.Momentum);
            lastDelta[index] = delta;
            return delta;
        }
    }
}
