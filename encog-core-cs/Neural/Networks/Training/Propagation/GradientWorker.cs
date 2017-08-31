//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Error;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Error;
using Encog.Neural.Flat;
using Encog.Util;
using Encog.Util.Concurrency;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Worker class for the mulithreaded training of flat networks.
    /// </summary>
    ///
    public class GradientWorker
    {
        /// <summary>
        /// The actual values from the neural network.
        /// </summary>
        ///
        private readonly double[] _actual;

        /// <summary>
        /// The error calculation method.
        /// </summary>
        ///
        private readonly ErrorCalculation _errorCalculation;

        /// <summary>
        /// The gradients.
        /// </summary>
        ///
        private readonly double[] _gradients;

        /// <summary>
        /// The low end of the training.
        /// </summary>
        ///
        private readonly int _high;

        /// <summary>
        /// The neuron counts, per layer.
        /// </summary>
        ///
        private readonly int[] _layerCounts;

        /// <summary>
        /// The deltas for each layer.
        /// </summary>
        ///
        private readonly double[] _layerDelta;

        /// <summary>
        /// The feed counts, per layer.
        /// </summary>
        ///
        private readonly int[] _layerFeedCounts;

        /// <summary>
        /// The layer indexes.
        /// </summary>
        ///
        private readonly int[] _layerIndex;

        /// <summary>
        /// The output from each layer.
        /// </summary>
        ///
        private readonly double[] _layerOutput;

        /// <summary>
        /// The sum from each layer.
        /// </summary>
        ///
        private readonly double[] _layerSums;

        /// <summary>
        /// The high end of the training data.
        /// </summary>
        ///
        private readonly int _low;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly FlatNetwork _network;

        /// <summary>
        /// The owner.
        /// </summary>
        ///
        private readonly Propagation _owner;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The index to each layer's weights and thresholds.
        /// </summary>
        ///
        private readonly int[] _weightIndex;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        ///
        private readonly double[] _weights;

        /// <summary>
        /// Derivative add constant.  Used to combat flat spot.
        /// </summary>
        private readonly double[] _flatSpot;

        /// <summary>
        /// The error function.
        /// </summary>
        private readonly IErrorFunction _ef;


        /// <summary>
        /// Construct a gradient worker.
        /// </summary>
        ///
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theOwner">The owner that is doing the training.</param>
        /// <param name="theTraining">The training data.</param>
        /// <param name="theLow">The low index to use in the training data.</param>
        /// <param name="theHigh">The high index to use in the training data.</param>
        /// <param name="theFlatSpots">Holds an array of flat spot constants.</param>
        public GradientWorker(FlatNetwork theNetwork,
                                 Propagation theOwner, IMLDataSet theTraining,
                                 int theLow, int theHigh, double[] theFlatSpots, IErrorFunction ef)
        {
            _errorCalculation = new ErrorCalculation();
            _network = theNetwork;
            _training = theTraining;
            _low = theLow;
            _high = theHigh;
            _owner = theOwner;
            _flatSpot = theFlatSpots;

            _layerDelta = new double[_network.LayerOutput.Length];
            _gradients = new double[_network.Weights.Length];
            _actual = new double[_network.OutputCount];

            _weights = _network.Weights;
            _layerIndex = _network.LayerIndex;
            _layerCounts = _network.LayerCounts;
            _weightIndex = _network.WeightIndex;
            _layerOutput = _network.LayerOutput;
            _layerSums = _network.LayerSums;
            _layerFeedCounts = _network.LayerFeedCounts;
            _ef = ef;
        }

        #region FlatGradientWorker Members

        /// <inheritdoc/>
        public FlatNetwork Network
        {
            get { return _network; }
        }


        /// <value>The weights for this network.</value>
        public double[] Weights
        {
            get { return _weights; }
        }

        /// <summary>
        /// Perform the gradient calculation for the specified index range.
        /// </summary>
        ///
        public void Run()
        {
            try
            {
                _errorCalculation.Reset();
                for (int i = _low; i <= _high; i++)
                {
                    var pair = _training[i];
                    Process(pair);
                }
                double error = _errorCalculation.Calculate();
                _owner.Report(_gradients, error, null);
                EngineArray.Fill(_gradients, 0);
            }
            catch (Exception ex)
            {
                _owner.Report(null, 0, ex);
            }
        }

        #endregion

        /// <summary>
        /// Process one training set element.
        /// </summary>
        ///
        /// <param name="input">The network input.</param>
        /// <param name="ideal">The ideal values.</param>
        /// <param name="s">The significance of this error.</param>
        private void Process(IMLDataPair pair)
        {
            _network.Compute(pair.Input, _actual);

            _errorCalculation.UpdateError(_actual, pair.Ideal, pair.Significance);

            // Calculate error for the output layer.
            _ef.CalculateError(
                    _network.ActivationFunctions[0], _layerSums, _layerOutput,
                    pair.Ideal, _actual, _layerDelta, _flatSpot[0],
                    pair.Significance);

            // Apply regularization, if requested.
            if (_owner.L1 > EncogFramework.DefaultDoubleEqual
                    || _owner.L1 > EncogFramework.DefaultDoubleEqual)
            {
                double[] lp = new double[2];
                CalculateRegularizationPenalty(lp);
                for (int i = 0; i < _actual.Length; i++)
                {
                    double p = (lp[0] * _owner.L1) + (lp[1] * _owner.L2);
                    _layerDelta[i] += p;
                }
            }

            // Propagate backwards (chain rule from calculus).
            for (int i = _network.BeginTraining; i < _network
                    .EndTraining; i++)
            {
                ProcessLevel(i);
            }
            
        }

        /// <summary>
        /// The error calculation to use.
        /// </summary>
        public ErrorCalculation CalculateError { get { return _errorCalculation; }}

        public void Run(int index) 
        {
		    IMLDataPair pair = _training[index];
		    Process(pair);
		    _owner.Report(_gradients, 0, null);
		    EngineArray.Fill(_gradients, 0);
	    }

        /// <summary>
        /// Process one level.
        /// </summary>
        ///
        /// <param name="currentLevel">The level.</param>
        private void ProcessLevel(int currentLevel)
        {
            int fromLayerIndex = _layerIndex[currentLevel + 1];
            int toLayerIndex = _layerIndex[currentLevel];
            int fromLayerSize = _layerCounts[currentLevel + 1];
            int toLayerSize = _layerFeedCounts[currentLevel];

            int index = _weightIndex[currentLevel];
            IActivationFunction activation = _network.ActivationFunctions[currentLevel + 1];
            double currentFlatSpot = _flatSpot[currentLevel + 1];

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
                    _gradients[wi] += output*_layerDelta[xi];
                    sum += _weights[wi]*_layerDelta[xi];
                    wi += fromLayerSize;
                    xi++;
                }

                _layerDelta[yi] = sum
                                 *(activation.DerivativeFunction(_layerSums[yi],_layerOutput[yi])+currentFlatSpot);
                yi++;
            }
        }

        public void CalculateRegularizationPenalty(double[] l)
        {
            for (int i = 0; i < _network.LayerCounts.Length - 1; i++)
            {
                LayerRegularizationPenalty(i, l);
            }
        }

        public void LayerRegularizationPenalty(int fromLayer, double[] l)
        {
            int fromCount = _network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = _network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double w = _network.GetWeight(fromLayer, fromNeuron, toNeuron);
                    l[0] += Math.Abs(w);
                    l[1] += w * w;
                }
            }
        }
    }
}
