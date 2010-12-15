/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */
namespace Encog.Engine.Network.Train.Prop
{

    using Encog.Engine.Data;
    using Encog.Engine.Network.Flat;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Train a flat network using RPROP.
    /// </summary>
    ///
    public class TrainFlatNetworkResilient : TrainFlatNetworkProp
    {
        /// <summary>
        /// The update values, for the weights and thresholds.
        /// </summary>
        ///
        private readonly double[] updateValues;

        /// <summary>
        /// The zero tolerance.
        /// </summary>
        ///
        private readonly double zeroTolerance;

        /// <summary>
        /// The maximum step value for rprop.
        /// </summary>
        ///
        private readonly double maxStep;

        /// <summary>
        /// Construct a resilient trainer for flat networks.
        /// </summary>
        ///
        /// <param name="network"/>The network to train.</param>
        /// <param name="training"/>The training data to use.</param>
        /// <param name="zeroTolerance_0"/>How close a number should be to zero to be counted as zero.</param>
        /// <param name="initialUpdate"/>The initial update value.</param>
        /// <param name="maxStep_1"/>The maximum step value.</param>
        public TrainFlatNetworkResilient(FlatNetwork network,
                IEngineDataSet training, double zeroTolerance_0,
                double initialUpdate, double maxStep_1)
            : base(network, training)
        {
            this.updateValues = new double[network.Weights.Length];
            this.zeroTolerance = zeroTolerance_0;
            this.maxStep = maxStep_1;

            for (int i = 0; i < this.updateValues.Length; i++)
            {
                this.updateValues[i] = initialUpdate;
            }
        }

        /// <summary>
        /// Tran a network using RPROP.
        /// </summary>
        ///
        /// <param name="flat"/>The network to train.</param>
        /// <param name="trainingSet"/>The training data to use.</param>
        public TrainFlatNetworkResilient(FlatNetwork flat,
                IEngineDataSet trainingSet)
            : this(flat, trainingSet, RPROPConst.DEFAULT_ZERO_TOLERANCE, RPROPConst.DEFAULT_INITIAL_UPDATE, RPROPConst.DEFAULT_MAX_STEP)
        {
        }

        /// <summary>
        /// Determine the sign of the value.
        /// </summary>
        ///
        /// <param name="value"/>The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double value_ren)
        {
            if (Math.Abs(value_ren) < this.zeroTolerance)
            {
                return 0;
            }
            else if (value_ren > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients"/>The gradients.</param>
        /// <param name="lastGradient"/>The last gradients.</param>
        /// <param name="index"/>The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override double UpdateWeight(double[] gradients,
                double[] lastGradient, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index] * lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = this.updateValues[index] * RPROPConst.POSITIVE_ETA;
                delta = Math.Min(delta, this.maxStep);
                weightChange = Sign(gradients[index]) * delta;
                this.updateValues[index] = delta;
                lastGradient[index] = gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta_0 = this.updateValues[index] * RPROPConst.NEGATIVE_ETA;
                delta_0 = Math.Max(delta_0, RPROPConst.DELTA_MIN);
                this.updateValues[index] = delta_0;
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = this.updateValues[index];

                weightChange = Sign(gradients[index]) * delta;
                lastGradient[index] = gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }


        /// <returns>The RPROP update values.</returns>
        public double[] UpdateValues
        {

            /// <returns>The RPROP update values.</returns>
            get
            {
                return updateValues;
            }
        }


    }
}
