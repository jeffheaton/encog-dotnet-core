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
    /// Train a flat network, using backpropagation.
    /// </summary>
    ///
    public class TrainFlatNetworkBackPropagation : TrainFlatNetworkProp
    {

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// The momentum.
        /// </summary>
        ///
        private double momentum;

        /// <summary>
        /// The last delta values.
        /// </summary>
        ///
        private double[] lastDelta;

        /// <summary>
        /// Construct a backprop trainer for flat networks.
        /// </summary>
        ///
        /// <param name="network"/>The network to train.</param>
        /// <param name="training"/>The training data.</param>
        /// <param name="learningRate_0"/>The learning rate.</param>
        /// <param name="momentum_1"/>The momentum.</param>
        public TrainFlatNetworkBackPropagation(FlatNetwork network,
                IEngineDataSet training, double learningRate_0,
                double momentum_1)
            : base(network, training)
        {
            this.momentum = momentum_1;
            this.learningRate = learningRate_0;
            this.lastDelta = new double[network.Weights.Length];
        }

        /// <summary>
        /// the learningRate
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


        /// <summary>
        /// The momentum.
        /// </summary>
        public double Momentum
        {
            get
            {
                return this.momentum;
            }
            set
            {
                this.learningRate = value;
            }
        }


        /// <summary>
        /// Update a weight.
        /// </summary>
        ///
        /// <param name="gradients"/>The gradients.</param>
        /// <param name="lastGradient"/>The last gradients.</param>
        /// <param name="index"/>The index.</param>
        /// <returns>The weight delta.</returns>
        public override double UpdateWeight(double[] gradients,
                double[] lastGradient, int index)
        {
            double delta = (gradients[index] * this.learningRate)
                    + (this.lastDelta[index] * this.momentum);
            this.lastDelta[index] = delta;
            return delta;
        }

        /// <summary>
        /// The last deltas.
        /// </summary>
        public double[] LastDelta
        {

            /// <returns>The last deltas.</returns>
            get
            {
                return lastDelta;
            }
            set
            {
                this.lastDelta = value;
            }
        }
    }
}
