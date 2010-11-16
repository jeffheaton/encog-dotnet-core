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

namespace Encog.Engine.Network.Train.Gradient
{

    using Encog.Engine.Data;
    using Encog.Engine.Network.Activation;
    using Encog.Engine.Network.Flat;
    using Encog.Engine.Network.Train.Prop;
    using Encog.Engine.Util;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Diagnostics;
    using Encog.Util.Time;

    /// <summary>
    /// Worker class for the mulithreaded training of flat networks.
    /// </summary>
    ///
    public class GradientWorkerCPU : IFlatGradientWorker
    {

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly FlatNetwork network;

        /// <summary>
        /// The error calculation method.
        /// </summary>
        ///
        private readonly ErrorCalculation errorCalculation;

        /// <summary>
        /// The actual values from the neural network.
        /// </summary>
        ///
        private readonly double[] actual;

        /// <summary>
        /// The deltas for each layer.
        /// </summary>
        ///
        private readonly double[] layerDelta;

        /// <summary>
        /// The neuron counts, per layer.
        /// </summary>
        ///
        private readonly int[] layerCounts;

        /// <summary>
        /// The feed counts, per layer.
        /// </summary>
        ///
        private int[] layerFeedCounts;

        /// <summary>
        /// The layer indexes.
        /// </summary>
        ///
        private readonly int[] layerIndex;

        /// <summary>
        /// The index to each layer's weights and thresholds.
        /// </summary>
        ///
        private readonly int[] weightIndex;

        /// <summary>
        /// The output from each layer.
        /// </summary>
        ///
        private readonly double[] layerOutput;

        /// <summary>
        /// The gradients.
        /// </summary>
        ///
        private readonly double[] gradients;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        ///
        private readonly double[] weights;

        /// <summary>
        /// The pair to use for training.
        /// </summary>
        ///
        private readonly IEngineData pair;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly IEngineIndexableSet training;

        /// <summary>
        /// The high end of the training data.
        /// </summary>
        ///
        private readonly int low;

        /// <summary>
        /// The low end of the training.
        /// </summary>
        ///
        private readonly int high;

        /// <summary>
        /// The owner.
        /// </summary>
        ///
        private readonly TrainFlatNetworkProp owner;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        ///
        private long elapsedTime;

        /// <summary>
        /// The stopwatch, to evaluate performance.
        /// </summary>
        ///
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Construct a gradient worker.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="owner">The owner that is doing the training.</param>
        /// <param name="training">The training data.</param>
        /// <param name="low">The low index to use in the training data.</param>
        /// <param name="high">The high index to use in the training data.</param>
        public GradientWorkerCPU(FlatNetwork network,
                TrainFlatNetworkProp owner,
                IEngineIndexableSet training, int low, int high)
        {
            this.errorCalculation = new ErrorCalculation();
            this.network = network;
            this.training = training;
            this.low = low;
            this.high = high;
            this.owner = owner;

            this.stopwatch = new Stopwatch();

            this.layerDelta = new double[network.LayerOutput.Length];
            this.gradients = new double[network.Weights.Length];
            this.actual = new double[network.OutputCount];

            this.weights = network.Weights;
            this.layerIndex = network.LayerIndex;
            this.layerCounts = network.LayerCounts;
            this.weightIndex = network.WeightIndex;
            this.layerOutput = network.LayerOutput;
            this.layerFeedCounts = network.LayerFeedCounts;

            this.pair = BasicEngineData.CreatePair(network.InputCount,
                    network.OutputCount);
        }


        /// <summary>
        /// Elapsed time for the last iteration.
        /// </summary>
        public virtual long ElapsedTime
        {
            get
            {
                return this.elapsedTime;
            }
        }



        /// <summary>
        /// The weights for this network.
        /// </summary>
        public virtual double[] Weights
        {
            get
            {
                return this.weights;
            }
        }


        /// <summary>
        /// Process one training set element.
        /// </summary>
        ///
        /// <param name="input">The network input.</param>
        /// <param name="ideal">The ideal values.</param>
        private void Process(double[] input, double[] ideal)
        {
            this.network.Compute(input, this.actual);

            this.errorCalculation.UpdateError(this.actual, ideal);

            for (int i = 0; i < this.actual.Length; i++)
            {

                this.layerDelta[i] = this.network.ActivationFunctions[0]
                        .DerivativeFunction(this.actual[i])
                        * (ideal[i] - this.actual[i]);
            }

            for (int i_0 = this.network.BeginTraining; i_0 < this.network.EndTraining; i_0++)
            {
                ProcessLevel(i_0);
            }
        }

        /// <summary>
        /// Process one level.
        /// </summary>
        ///
        /// <param name="currentLevel">The level.</param>
        private void ProcessLevel(int currentLevel)
        {
            int fromLayerIndex = this.layerIndex[currentLevel + 1];
            int toLayerIndex = this.layerIndex[currentLevel];
            int fromLayerSize = this.layerCounts[currentLevel + 1];
            int toLayerSize = this.layerFeedCounts[currentLevel];

            int index = this.weightIndex[currentLevel];
            IActivationFunction activation = this.network.ActivationFunctions[currentLevel + 1];

            // handle weights
            int yi = fromLayerIndex;
            for (int y = 0; y < fromLayerSize; y++)
            {
                double output = this.layerOutput[yi];
                double sum = 0;
                int xi = toLayerIndex;
                int wi = index + y;
                for (int x = 0; x < toLayerSize; x++)
                {
                    this.gradients[wi] += output * this.layerDelta[xi];
                    sum += this.weights[wi] * this.layerDelta[xi];
                    wi += fromLayerSize;
                    xi++;
                }

                this.layerDelta[yi] = sum
                        * activation.DerivativeFunction(this.layerOutput[yi]);
                yi++;
            }
        }

        /// <summary>
        /// Perform the gradient calculation for the specified index range.
        /// </summary>
        ///
        public virtual void Run()
        {
            try
            {
                this.stopwatch.Reset();
                this.stopwatch.Start();
                this.errorCalculation.Reset();
                for (int i = this.low; i <= this.high; i++)
                {
                    this.training.GetRecord(i, this.pair);
                    Process(this.pair.InputArray, this.pair.IdealArray);
                }
                double error = this.errorCalculation.Calculate();
                this.owner.Report(this.gradients, error, null);
                EngineArray.Fill(this.gradients, 0);
                this.stopwatch.Stop();
                this.elapsedTime = this.stopwatch.ElapsedTicks;
            }
            catch (Exception ex)
            {
                this.owner.Report(null, 0, ex);
            }
        }


        /// <summary>
        /// The network training.
        /// </summary>
        public virtual FlatNetwork Network
        {
            get
            {
                return this.network;
            }
        }


    }
}
