//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;
using Encog.Neural.Flat;
using Encog.ML.Data;
using Encog.Util;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    /// A threaded worker that is used to calculate the first derivatives of the
    /// output of the neural network. These values are ultimatly used to calculate
    /// the Hessian.
    /// </summary>
    public class ChainRuleWorker : IEngineTask
    {
        /// <summary>
        /// The actual values from the neural network.
        /// </summary>
        private double[] actual;

        /// <summary>
        /// The deltas for each layer.
        /// </summary>
        private double[] layerDelta;

        /// <summary>
        /// The neuron counts, per layer.
        /// </summary>
        private int[] layerCounts;

        /// <summary>
        /// The feed counts, per layer.
        /// </summary>
        private int[] layerFeedCounts;

        /// <summary>
        /// The layer indexes.
        /// </summary>
        private int[] layerIndex;

        /// <summary>
        /// The index to each layer's weights and thresholds.
        /// </summary>
        private int[] weightIndex;

        /// <summary>
        /// The output from each layer.
        /// </summary>
        private double[] layerOutput;

        /// <summary>
        /// The sums.
        /// </summary>
        private double[] layerSums;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The flat network.
        /// </summary>
        private FlatNetwork flat;

        /// <summary>
        /// The current first derivatives.
        /// </summary>
        private double[] derivative;

        /// <summary>
        /// The training data.
        /// </summary>
        private IMLDataSet training;

        /// <summary>
        /// The output neuron to calculate for.
        /// </summary>
        private int outputNeuron;

        /// <summary>
        /// The total first derivatives.
        /// </summary>
        private double[] totDeriv;

        /// <summary>
        /// The gradients.
        /// </summary>
        private double[] gradients;

        /// <summary>
        /// The error.
        /// </summary>
        private double error;

        /// <summary>
        /// The low range.
        /// </summary>
        private int low;

        /// <summary>
        /// The high range.
        /// </summary>
        private int high;

        /// <summary>
        /// The pair to use for training.
        /// </summary>
        private IMLDataPair pair;

        /// <summary>
        /// Construct the chain rule worker. 
        /// </summary>
        /// <param name="theNetwork">The network to calculate a Hessian for.</param>
        /// <param name="theTraining">The training data.</param>
        /// <param name="theLow">The low range.</param>
        /// <param name="theHigh">The high range.</param>
        public ChainRuleWorker(FlatNetwork theNetwork, IMLDataSet theTraining, int theLow, int theHigh)
        {

            int weightCount = theNetwork.Weights.Length;

            this.training = theTraining;
            this.flat = theNetwork;

            this.layerDelta = new double[flat.LayerOutput.Length];
            this.actual = new double[flat.OutputCount];
            this.derivative = new double[weightCount];
            this.totDeriv = new double[weightCount];
            this.gradients = new double[weightCount];

            this.weights = flat.Weights;
            this.layerIndex = flat.LayerIndex;
            this.layerCounts = flat.LayerCounts;
            this.weightIndex = flat.WeightIndex;
            this.layerOutput = flat.LayerOutput;
            this.layerSums = flat.LayerSums;
            this.layerFeedCounts = flat.LayerFeedCounts;
            this.low = theLow;
            this.high = theHigh;
            this.pair = BasicMLDataPair.CreatePair(flat.InputCount, flat.OutputCount);
        }


        /// <inheritdoc/>
        public void Run()
        {
            this.error = 0;
            EngineArray.Fill(this.totDeriv, 0);
            EngineArray.Fill(this.gradients, 0);


            // Loop over every training element
            for (int i = this.low; i <= this.high; i++)
            {
                this.training.GetRecord(i, this.pair);

                EngineArray.Fill(this.derivative, 0);
                Process(outputNeuron, pair.InputArray, pair.IdealArray);
            }
        }


        /// <summary>
        /// Process one training set element.
        /// </summary>
        /// <param name="outputNeuron">The output neuron.</param>
        /// <param name="input">The network input.</param>
        /// <param name="ideal">The ideal values.</param>
        private void Process(int outputNeuron, double[] input, double[] ideal)
        {

            this.flat.Compute(input, this.actual);

            double e = ideal[outputNeuron] - this.actual[outputNeuron];
            this.error += e * e;

            for (int i = 0; i < this.actual.Length; i++)
            {

                if (i == outputNeuron)
                {
                    this.layerDelta[i] = this.flat.ActivationFunctions[0]
                            .DerivativeFunction(this.layerSums[i],
                                    this.layerOutput[i]);
                }
                else
                {
                    this.layerDelta[i] = 0;
                }
            }

            for (int i = this.flat.BeginTraining; i < this.flat.EndTraining; i++)
            {
                ProcessLevel(i);
            }

            // calculate gradients
            for (int j = 0; j < this.weights.Length; j++)
            {
                this.gradients[j] += e * this.derivative[j];
                totDeriv[j] += this.derivative[j];
            }
        }

        /// <summary>
        /// Process one level. 
        /// </summary>
        /// <param name="currentLevel">The level.</param>
        private void ProcessLevel(int currentLevel)
        {
            int fromLayerIndex = this.layerIndex[currentLevel + 1];
            int toLayerIndex = this.layerIndex[currentLevel];
            int fromLayerSize = this.layerCounts[currentLevel + 1];
            int toLayerSize = this.layerFeedCounts[currentLevel];

            int index = this.weightIndex[currentLevel];
            IActivationFunction activation = this.flat
                    .ActivationFunctions[currentLevel + 1];

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
                    this.derivative[wi] += output * this.layerDelta[xi];
                    sum += this.weights[wi] * this.layerDelta[xi];
                    wi += fromLayerSize;
                    xi++;
                }

                this.layerDelta[yi] = sum
                        * (activation.DerivativeFunction(this.layerSums[yi], this.layerOutput[yi]));
                yi++;
            }
        }


        /// <summary>
        /// The output neuron we are processing.
        /// </summary>
        public int OutputNeuron
        {
            get
            {
                return outputNeuron;
            }
            set
            {
                this.outputNeuron = value;
            }
        }
        
        /// <summary>
        /// The first derivatives, used to calculate the Hessian.
        /// </summary>
        public double[] Derivative
        {
            get
            {
                return this.totDeriv;
            }
        }


        /// <summary>
        /// The gradients.
        /// </summary>
        public double[] Gradients
        {
            get
            {
                return gradients;
            }
        }

        /// <summary>
        /// The SSE error.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error;
            }
        }

        /// <summary>
        /// The flat network.
        /// </summary>
        public FlatNetwork Network
        {
            get
            {
                return this.flat;
            }
        }

    }
}
