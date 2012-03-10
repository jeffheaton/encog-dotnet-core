//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Flat;
using Encog.Util;
using Encog.Util.Concurrency;

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
        private readonly double[] _actual;

        /// <summary>
        /// The current first derivatives.
        /// </summary>
        private readonly double[] _derivative;

        /// <summary>
        /// The flat network.
        /// </summary>
        private readonly FlatNetwork _flat;

        /// <summary>
        /// The gradients.
        /// </summary>
        private readonly double[] _gradients;

        /// <summary>
        /// The high range.
        /// </summary>
        private readonly int _high;

        /// <summary>
        /// The neuron counts, per layer.
        /// </summary>
        private readonly int[] _layerCounts;

        /// <summary>
        /// The deltas for each layer.
        /// </summary>
        private readonly double[] _layerDelta;

        /// <summary>
        /// The feed counts, per layer.
        /// </summary>
        private readonly int[] _layerFeedCounts;

        /// <summary>
        /// The layer indexes.
        /// </summary>
        private readonly int[] _layerIndex;

        /// <summary>
        /// The output from each layer.
        /// </summary>
        private readonly double[] _layerOutput;

        /// <summary>
        /// The sums.
        /// </summary>
        private readonly double[] _layerSums;

        /// <summary>
        /// The low range.
        /// </summary>
        private readonly int _low;

        /// <summary>
        /// The pair to use for training.
        /// </summary>
        private readonly IMLDataPair _pair;

        /// <summary>
        /// The total first derivatives.
        /// </summary>
        private readonly double[] _totDeriv;

        /// <summary>
        /// The training data.
        /// </summary>
        private readonly IMLDataSet _training;

        /// <summary>
        /// The index to each layer's weights and thresholds.
        /// </summary>
        private readonly int[] _weightIndex;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        private readonly double[] _weights;

        /// <summary>
        /// The error.
        /// </summary>
        private double _error;

        /// <summary>
        /// The output neuron to calculate for.
        /// </summary>
        private int _outputNeuron;

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

            _training = theTraining;
            _flat = theNetwork;

            _layerDelta = new double[_flat.LayerOutput.Length];
            _actual = new double[_flat.OutputCount];
            _derivative = new double[weightCount];
            _totDeriv = new double[weightCount];
            _gradients = new double[weightCount];

            _weights = _flat.Weights;
            _layerIndex = _flat.LayerIndex;
            _layerCounts = _flat.LayerCounts;
            _weightIndex = _flat.WeightIndex;
            _layerOutput = _flat.LayerOutput;
            _layerSums = _flat.LayerSums;
            _layerFeedCounts = _flat.LayerFeedCounts;
            _low = theLow;
            _high = theHigh;
            _pair = BasicMLDataPair.CreatePair(_flat.InputCount, _flat.OutputCount);
        }


        /// <summary>
        /// The output neuron we are processing.
        /// </summary>
        public int OutputNeuron
        {
            get { return _outputNeuron; }
            set { _outputNeuron = value; }
        }

        /// <summary>
        /// The first derivatives, used to calculate the Hessian.
        /// </summary>
        public double[] Derivative
        {
            get { return _totDeriv; }
        }


        /// <summary>
        /// The gradients.
        /// </summary>
        public double[] Gradients
        {
            get { return _gradients; }
        }

        /// <summary>
        /// The SSE error.
        /// </summary>
        public double Error
        {
            get { return _error; }
        }

        /// <summary>
        /// The flat network.
        /// </summary>
        public FlatNetwork Network
        {
            get { return _flat; }
        }

        #region IEngineTask Members

        /// <inheritdoc/>
        public void Run()
        {
            _error = 0;
            EngineArray.Fill(_totDeriv, 0);
            EngineArray.Fill(_gradients, 0);


            // Loop over every training element
            for (int i = _low; i <= _high; i++)
            {
                _training.GetRecord(i, _pair);

                EngineArray.Fill(_derivative, 0);
                Process(_outputNeuron, _pair.InputArray, _pair.IdealArray);
            }
        }

        #endregion

        /// <summary>
        /// Process one training set element.
        /// </summary>
        /// <param name="outputNeuron">The output neuron.</param>
        /// <param name="input">The network input.</param>
        /// <param name="ideal">The ideal values.</param>
        private void Process(int outputNeuron, double[] input, double[] ideal)
        {
            _flat.Compute(input, _actual);

            double e = ideal[outputNeuron] - _actual[outputNeuron];
            _error += e*e;

            for (int i = 0; i < _actual.Length; i++)
            {
                if (i == outputNeuron)
                {
                    _layerDelta[i] = _flat.ActivationFunctions[0]
                        .DerivativeFunction(_layerSums[i],
                                            _layerOutput[i]);
                }
                else
                {
                    _layerDelta[i] = 0;
                }
            }

            for (int i = _flat.BeginTraining; i < _flat.EndTraining; i++)
            {
                ProcessLevel(i);
            }

            // calculate gradients
            for (int j = 0; j < _weights.Length; j++)
            {
                _gradients[j] += e*_derivative[j];
                _totDeriv[j] += _derivative[j];
            }
        }

        /// <summary>
        /// Process one level. 
        /// </summary>
        /// <param name="currentLevel">The level.</param>
        private void ProcessLevel(int currentLevel)
        {
            int fromLayerIndex = _layerIndex[currentLevel + 1];
            int toLayerIndex = _layerIndex[currentLevel];
            int fromLayerSize = _layerCounts[currentLevel + 1];
            int toLayerSize = _layerFeedCounts[currentLevel];

            int index = _weightIndex[currentLevel];
            IActivationFunction activation = _flat
                .ActivationFunctions[currentLevel + 1];

            // handle weights
            int yi = fromLayerIndex;
            for (int y = 0; y < fromLayerSize; y++)
            {
                double output = _layerOutput[yi];
                double sum = 0;
                int xi = toLayerIndex;
                int wi = index + y;
                for (int x = 0; x < toLayerSize; x++)
                {
                    _derivative[wi] += output*_layerDelta[xi];
                    sum += _weights[wi]*_layerDelta[xi];
                    wi += fromLayerSize;
                    xi++;
                }

                _layerDelta[yi] = sum
                                 *(activation.DerivativeFunction(_layerSums[yi], _layerOutput[yi]));
                yi++;
            }
        }
    }
}
