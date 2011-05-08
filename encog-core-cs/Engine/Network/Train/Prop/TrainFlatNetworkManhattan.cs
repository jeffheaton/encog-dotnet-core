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
    using Encog.Engine.Network.Flat;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Neural.NeuralData;

    /// <summary>
    /// Train the flat network using Manhattan update rule.
    /// </summary>
    ///
    public class TrainFlatNetworkManhattan : TrainFlatNetworkProp
    {

        /// <summary>
        /// The zero tolerance to use.
        /// </summary>
        ///
        private readonly double zeroTolerance;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// Construct a trainer for flat networks to use the Manhattan update rule.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learningRate">The learning rate to use.</param>
        public TrainFlatNetworkManhattan(FlatNetwork network,
                MLDataSet training, double learningRate)
            : base(network, training)
        {
            this.learningRate = learningRate;
            this.zeroTolerance = RPROPConst.DEFAULT_ZERO_TOLERANCE;
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override double UpdateWeight(double[] gradients,
                double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < this.zeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return this.learningRate;
            }
            else
            {
                return -this.learningRate;
            }
        }

        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return this.learningRate;
            }
            set
            {
                this.learningRate = value;
            }
        }

    }
}
