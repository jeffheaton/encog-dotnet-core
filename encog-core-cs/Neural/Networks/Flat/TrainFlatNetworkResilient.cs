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
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Train a flat network using RPROP.
    /// </summary>
    public class TrainFlatNetworkResilient : TrainFlatNetworkMulti
    {
        /// <summary>
        /// The update values, for the weights and thresholds.
        /// </summary>
        private double[] updateValues;

        /// <summary>
        /// The zero tolerance.
        /// </summary>
        private double zeroTolerance;

        /// <summary>
        /// The maximum step value for rprop.
        /// </summary>
        private double maxStep;

        /// <summary>
        /// Construct a resilient trainer for flat networks.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="zeroTolerance">How close a number should 
        /// be to zero to be counted as zero.</param>
        /// <param name="initialUpdate">The initial update value.</param>
        /// <param name="maxStep">The maximum step value.</param>
        /// <summary>
        /// The maximum delta amount.
        /// </summary>
        public TrainFlatNetworkResilient(FlatNetwork network,
            INeuralDataSet training, double zeroTolerance,
                 double initialUpdate, double maxStep) :
            base(network,training)
        {
            updateValues = new double[network.Weights.Length];
            this.zeroTolerance = zeroTolerance;
            this.maxStep = maxStep;

            for (int i = 0; i < updateValues.Length; i++)
            {
                updateValues[i] = initialUpdate;
            }
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
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index] * lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = updateValues[index]
                        * ResilientPropagation.POSITIVE_ETA;
                delta = Math.Min(delta, this.maxStep);
                weightChange = Sign(gradients[index]) * delta;
                updateValues[index] = delta;
                lastGradient[index] = gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = updateValues[index]
                        * ResilientPropagation.NEGATIVE_ETA;
                delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                updateValues[index] = delta;
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = lastGradient[index];
                weightChange = Sign(gradients[index]) * delta;
                lastGradient[index] = gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }

        /// <summary>
        /// Determine the sign of the value. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double value)
        {
            if (Math.Abs(value) < this.zeroTolerance)
            {
                return 0;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
