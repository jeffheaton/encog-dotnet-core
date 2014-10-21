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
using System.Collections.Generic;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Error;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util;
using Encog.MathUtil;

namespace Encog.Neural.Flat
{
    /// <summary>
    /// Implements a flat (vector based) neural network in the Encog Engine. This is
    /// meant to be a very highly efficient feedforward, or simple recurrent, neural
    /// network. It uses a minimum of objects and is designed with one principal in
    /// mind-- SPEED. Readability, code reuse, object oriented programming are all
    /// secondary in consideration.
    /// Vector based neural networks are also very good for GPU processing. The flat
    /// network classes will make use of the GPU if you have enabled GPU processing.
    /// See the Encog class for more info.
    /// </summary>
    ///
    [Serializable]
    public class FlatNetwork
    {
        /// <summary>
        /// The default bias activation.
        /// </summary>
        ///
        public const double DefaultBiasActivation = 1.0d;

        /// <summary>
        /// The value that indicates that there is no bias activation.
        /// </summary>
        ///
        public const double NoBiasActivation = 0.0d;

        /// <summary>
        /// The activation types.
        /// </summary>
        ///
        private IActivationFunction[] _activationFunctions;

        /// <summary>
        /// The layer that training should begin on.
        /// </summary>
        ///
        private int _beginTraining;

        /// <summary>
        /// The bias activation for each layer. This is usually either 1, for a bias,
        /// or zero for no bias.
        /// </summary>
        ///
        private double[] _biasActivation;

        /// <summary>
        /// The limit, under which, all a cconnection is not considered to exist.
        /// </summary>
        ///
        private double _connectionLimit;

        /// <summary>
        /// The context target for each layer. This is how the backwards connections
        /// are formed for the recurrent neural network. Each layer either has a
        /// zero, which means no context target, or a layer number that indicates the
        /// target layer.
        /// </summary>
        ///
        private int[] _contextTargetOffset;

        /// <summary>
        /// The size of each of the context targets. If a layer's contextTargetOffset
        /// is zero, its contextTargetSize should also be zero. The contextTargetSize
        /// should always match the feed count of the targeted context layer.
        /// </summary>
        ///
        private int[] _contextTargetSize;

        /// <summary>
        /// The layer that training should end on.
        /// </summary>
        ///
        private int _endTraining;

        /// <summary>
        /// True if the network has context.
        /// </summary>
        ///
        private bool _hasContext;

        /// <summary>
        /// The number of input neurons in this network.
        /// </summary>
        ///
        private int _inputCount;

        /// <summary>
        /// Does this network have some connections disabled.
        /// </summary>
        ///
        private bool _isLimited;

        /// <summary>
        /// The number of context neurons in each layer. These context neurons will
        /// feed the next layer.
        /// </summary>
        ///
        private int[] _layerContextCount;

        /// <summary>
        /// The number of neurons in each of the layers.
        /// </summary>
        ///
        private int[] _layerCounts;

        /// <summary>
        /// The number of neurons in each layer that are actually fed by neurons in
        /// the previous layer. Bias neurons, as well as context neurons, are not fed
        /// from the previous layer.
        /// </summary>
        ///
        private int[] _layerFeedCounts;

        /// <summary>
        /// An index to where each layer begins (based on the number of neurons in
        /// each layer).
        /// </summary>
        ///
        private int[] _layerIndex;

        /// <summary>
        /// The outputs from each of the neurons, after activation applied.
        /// </summary>
        ///
        private double[] _layerOutput;

        /// <summary>
        /// The sums from each of the neurons, before activation applied.
        /// </summary>
        ///
        private double[] _layerSums;

        /// <summary>
        /// The number of output neurons in this network.
        /// </summary>
        ///
        private int _outputCount;

        /// <summary>
        /// The index to where the weights that are stored at for a given layer.
        /// </summary>
        ///
        private int[] _weightIndex;

        /// <summary>
        /// The weights for a neural network.
        /// </summary>
        ///
        private double[] _weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public FlatNetwork()
        {
        }

        /// <summary>
        /// Create a flat network from an array of layers.
        /// </summary>
        ///
        /// <param name="layers">The layers.</param>
        public FlatNetwork(FlatLayer[] layers)
        {
            Init(layers);
        }

        /// <summary>
        /// Construct a flat neural network.
        /// </summary>
        ///
        /// <param name="input">Neurons in the input layer.</param>
        /// <param name="hidden1"></param>
        /// <param name="hidden2"></param>
        /// <param name="output">Neurons in the output layer.</param>
        /// <param name="tanh">True if this is a tanh activation, false for sigmoid.</param>
        public FlatNetwork(int input, int hidden1, int hidden2,
                           int output, bool tanh)
        {
            IActivationFunction linearAct = new ActivationLinear();
            FlatLayer[] layers;
            IActivationFunction act = (tanh)
                                          ? (new ActivationTANH())
                                          : (IActivationFunction) (new ActivationSigmoid());

            if ((hidden1 == 0) && (hidden2 == 0))
            {
                layers = new FlatLayer[2];
                layers[0] = new FlatLayer(linearAct, input,
                                          DefaultBiasActivation);
                layers[1] = new FlatLayer(act, output,
                                          NoBiasActivation);
            }
            else if ((hidden1 == 0) || (hidden2 == 0))
            {
                int count = Math.Max(hidden1, hidden2);
                layers = new FlatLayer[3];
                layers[0] = new FlatLayer(linearAct, input,
                                          DefaultBiasActivation);
                layers[1] = new FlatLayer(act, count,
                                          DefaultBiasActivation);
                layers[2] = new FlatLayer(act, output,
                                          NoBiasActivation);
            }
            else
            {
                layers = new FlatLayer[4];
                layers[0] = new FlatLayer(linearAct, input,
                                          DefaultBiasActivation);
                layers[1] = new FlatLayer(act, hidden1,
                                          DefaultBiasActivation);
                layers[2] = new FlatLayer(act, hidden2,
                                          DefaultBiasActivation);
                layers[3] = new FlatLayer(act, output,
                                          NoBiasActivation);
            }

            _isLimited = false;
            _connectionLimit = 0.0d;

            Init(layers);
        }

        /// <summary>
        /// Set the activation functions.
        /// </summary>
        public IActivationFunction[] ActivationFunctions
        {
            get { return _activationFunctions; }
            set { _activationFunctions = value; }
        }


        /// <value>the beginTraining to set</value>
        public int BeginTraining
        {
            get { return _beginTraining; }
            set { _beginTraining = value; }
        }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        public double[] BiasActivation
        {
            get { return _biasActivation; }
            set { _biasActivation = value; }
        }


        /// <value>the connectionLimit to set</value>
        public double ConnectionLimit
        {
            get { return _connectionLimit; }
            set
            {
                _connectionLimit = value;
                if (Math.Abs(_connectionLimit
                             - BasicNetwork.DefaultConnectionLimit) < EncogFramework.DefaultDoubleEqual)
                {
                    _isLimited = true;
                }
            }
        }


        /// <summary>
        /// Set the context target offset.
        /// </summary>
        public int[] ContextTargetOffset
        {
            get { return _contextTargetOffset; }
            set { _contextTargetOffset = value; }
        }


        /// <summary>
        /// Set the context target size.
        /// </summary>
        public int[] ContextTargetSize
        {
            get { return _contextTargetSize; }
            set { _contextTargetSize = value; }
        }


        /// <value>The length of the array the network would encode to.</value>
        public int EncodeLength
        {
            get { return _weights.Length; }
        }


        /// <value>the endTraining to set</value>
        public int EndTraining
        {
            get { return _endTraining; }
            set { _endTraining = value; }
        }


        /// <summary>
        /// Set the hasContext property.
        /// </summary>
        public bool HasContext
        {
            get { return _hasContext; }
            set { _hasContext = value; }
        }


        /// <summary>
        /// Set the input count.
        /// </summary>
        public int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }


        /// <summary>
        /// Set the layer context count.
        /// </summary>
        public int[] LayerContextCount
        {
            get { return _layerContextCount; }
            set { _layerContextCount = value; }
        }


        /// <summary>
        /// Set the layer counts.
        /// </summary>
        public int[] LayerCounts
        {
            get { return _layerCounts; }
            set { _layerCounts = value; }
        }

        /// <summary>
        /// The layer feed counts. The number of neurons in each layer that are fed by the previous
        /// layer.
        /// </summary>
        public int[] LayerFeedCounts
        {
            get { return _layerFeedCounts; }
            set { _layerFeedCounts = value; }
        }


        /// <summary>
        /// Set the layer index.
        /// </summary>
        public int[] LayerIndex
        {
            get { return _layerIndex; }
            set { _layerIndex = value; }
        }


        /// <summary>
        /// Set the layer output.
        /// </summary>
        public double[] LayerOutput
        {
            get { return _layerOutput; }
            set { _layerOutput = value; }
        }


        /// <value>The neuron count.</value>
        public int NeuronCount
        {
            get
            {
                return _layerCounts.Sum();
            }
        }


        /// <summary>
        /// Set the output count.
        /// </summary>
        public int OutputCount
        {
            get { return _outputCount; }
            set { _outputCount = value; }
        }


        /// <summary>
        /// Set the weight index.
        /// </summary>
        public int[] WeightIndex
        {
            get { return _weightIndex; }
            set { _weightIndex = value; }
        }


        /// <summary>
        /// Set the weights.
        /// </summary>
        public double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <value>the isLimited</value>
        public bool Limited
        {
            get { return _isLimited; }
        }

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();

            var actual = new double[_outputCount];
			IMLDataPair pair;

            for (int i = 0; i < data.Count; i++)
            {
                pair = data[i];
                Compute(pair.Input, actual);
                errorCalculation.UpdateError(actual, pair.Ideal, pair.Significance);
            }
            return errorCalculation.Calculate();
        }

        /// <summary>
        /// Clear any connection limits.
        /// </summary>
        ///
        public void ClearConnectionLimit()
        {
            _connectionLimit = 0.0d;
            _isLimited = false;
        }

        /// <summary>
        /// Clear any context neurons.
        /// </summary>
        ///
        public void ClearContext()
        {
            int index = 0;

            for (int i = 0; i < _layerIndex.Length; i++)
            {
                bool hasBias = (_layerContextCount[i] + _layerFeedCounts[i]) != _layerCounts[i];

                // fill in regular neurons
                for (int j = 0; j < _layerFeedCounts[i]; j++)
                {
                    _layerOutput[index++] = 0;
                }

                // fill in the bias
                if (hasBias)
                {
                    _layerOutput[index++] = _biasActivation[i];
                }

                // fill in context
                for (int j = 0; j < _layerContextCount[i]; j++)
                {
                    _layerOutput[index++] = 0;
                }
            }
        }

        /// <summary>
        /// Clone the network.
        /// </summary>
        ///
        /// <returns>A clone of the network.</returns>
        public virtual Object Clone()
        {
            var result = new FlatNetwork();
            CloneFlatNetwork(result);
            return result;
        }

        /// <summary>
        /// Clone into the flat network passed in.
        /// </summary>
        ///
        /// <param name="result">The network to copy into.</param>
        public void CloneFlatNetwork(FlatNetwork result)
        {
            result._inputCount = _inputCount;
            result._layerCounts = EngineArray.ArrayCopy(_layerCounts);
            result._layerIndex = EngineArray.ArrayCopy(_layerIndex);
            result._layerOutput = EngineArray.ArrayCopy(_layerOutput);
            result._layerSums = EngineArray.ArrayCopy(_layerSums);
            result._layerFeedCounts = EngineArray.ArrayCopy(_layerFeedCounts);
            result._contextTargetOffset = EngineArray
                .ArrayCopy(_contextTargetOffset);
            result._contextTargetSize = EngineArray
                .ArrayCopy(_contextTargetSize);
            result._layerContextCount = EngineArray
                .ArrayCopy(_layerContextCount);
            result._biasActivation = EngineArray.ArrayCopy(_biasActivation);
            result._outputCount = _outputCount;
            result._weightIndex = _weightIndex;
            result._weights = _weights;

            result._activationFunctions = new IActivationFunction[_activationFunctions.Length];
            for (int i = 0; i < result._activationFunctions.Length; i++)
            {
                result._activationFunctions[i] = (IActivationFunction) _activationFunctions[i].Clone();
            }

            result._beginTraining = _beginTraining;
            result._endTraining = _endTraining;
        }

		public virtual void Compute(IMLData input, double[] output)
		{
			int sourceIndex = _layerOutput.Length
							  - _layerCounts[_layerCounts.Length - 1];

			input.CopyTo(_layerOutput, sourceIndex, _inputCount);

			InnerCompute(output);
		}

        /// <summary>
        /// Calculate the output for the given input.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public virtual void Compute(double[] input, double[] output)
        {
            int sourceIndex = _layerOutput.Length
                              - _layerCounts[_layerCounts.Length - 1];

            EngineArray.ArrayCopy(input, 0, _layerOutput, sourceIndex,
                                  _inputCount);

			InnerCompute(output);
        }

		private void InnerCompute(double[] output)
		{
			for(int i = _layerIndex.Length - 1; i > 0; i--)
			{
				ComputeLayer(i);
			}

			// update context values
			int offset = _contextTargetOffset[0];

			for(int x = 0; x < _contextTargetSize[0]; x++)
			{
				_layerOutput[offset + x] = _layerOutput[x];
			}

			EngineArray.ArrayCopy(_layerOutput, 0, output, 0, _outputCount);
		}

		/// <summary>
        /// Calculate a layer.
        /// </summary>
        ///
        /// <param name="currentLayer">The layer to calculate.</param>
        protected internal void ComputeLayer(int currentLayer)
        {
            int inputIndex = _layerIndex[currentLayer];
            int outputIndex = _layerIndex[currentLayer - 1];
            int inputSize = _layerCounts[currentLayer];
            int outputSize = _layerFeedCounts[currentLayer - 1];

            int index = _weightIndex[currentLayer - 1];

            int limitX = outputIndex + outputSize;
            int limitY = inputIndex + inputSize;

            // weight values
            for (int x = outputIndex; x < limitX; x++)
            {
                double sum = 0;
                for (int y = inputIndex; y < limitY; y++)
                {
                    sum += _weights[index++]*_layerOutput[y];
                }
                _layerOutput[x] = sum;
                _layerSums[x] = sum;
            }

            _activationFunctions[currentLayer - 1].ActivationFunction(
                _layerOutput, outputIndex, outputSize);

            // update context values
            int offset = _contextTargetOffset[currentLayer];

            for (int x = 0; x < _contextTargetSize[currentLayer]; x++)
            {
                _layerOutput[offset + x] = _layerOutput[outputIndex + x];
            }
        }

        /// <summary>
        /// Decode the specified data into the weights of the neural network. This
        /// method performs the opposite of encodeNetwork.
        /// </summary>
        ///
        /// <param name="data">The data to be decoded.</param>
        public void DecodeNetwork(double[] data)
        {
            if (data.Length != _weights.Length)
            {
                throw new EncogError(
                    "Incompatable weight sizes, can't assign length="
                    + data.Length + " to length=" + data.Length);
            }
            _weights = data;
        }

        /// <summary>
        /// Encode the neural network to an array of doubles. This includes the
        /// network weights. To read this into a neural network, use the
        /// decodeNetwork method.
        /// </summary>
        ///
        /// <returns>The encoded network.</returns>
        public double[] EncodeNetwork()
        {
            return _weights;
        }


        /// <summary>
        /// Neural networks with only one type of activation function offer certain
        /// optimization options. This method determines if only a single activation
        /// function is used.
        /// </summary>
        ///
        /// <returns>The number of the single activation function, or -1 if there are
        /// no activation functions or more than one type of activation
        /// function.</returns>
        public Type HasSameActivationFunction()
        {
            IList<Type> map = new List<Type>();


            foreach (IActivationFunction activation  in  _activationFunctions)
            {
                if (!map.Contains(activation.GetType()))
                {
                    map.Add(activation.GetType());
                }
            }

            if (map.Count != 1)
            {
                return null;
            }
            return map[0];
        }

        /// <summary>
        /// Construct a flat network.
        /// </summary>
        ///
        /// <param name="layers">The layers of the network to create.</param>
        public void Init(FlatLayer[] layers)
        {
            int layerCount = layers.Length;

            _inputCount = layers[0].Count;
            _outputCount = layers[layerCount - 1].Count;

            _layerCounts = new int[layerCount];
            _layerContextCount = new int[layerCount];
            _weightIndex = new int[layerCount];
            _layerIndex = new int[layerCount];
            _activationFunctions = new IActivationFunction[layerCount];
            _layerFeedCounts = new int[layerCount];
            _contextTargetOffset = new int[layerCount];
            _contextTargetSize = new int[layerCount];
            _biasActivation = new double[layerCount];

            int index = 0;
            int neuronCount = 0;
            int weightCount = 0;

            for (int i = layers.Length - 1; i >= 0; i--)
            {
                FlatLayer layer = layers[i];
                FlatLayer nextLayer = null;

                if (i > 0)
                {
                    nextLayer = layers[i - 1];
                }

                _biasActivation[index] = layer.BiasActivation;
                _layerCounts[index] = layer.TotalCount;
                _layerFeedCounts[index] = layer.Count;
                _layerContextCount[index] = layer.ContextCount;
                _activationFunctions[index] = layer.Activation;

                neuronCount += layer.TotalCount;

                if (nextLayer != null)
                {
                    weightCount += layer.Count*nextLayer.TotalCount;
                }

                if (index == 0)
                {
                    _weightIndex[index] = 0;
                    _layerIndex[index] = 0;
                }
                else
                {
                    _weightIndex[index] = _weightIndex[index - 1]
                                         + (_layerCounts[index]*_layerFeedCounts[index - 1]);
                    _layerIndex[index] = _layerIndex[index - 1]
                                        + _layerCounts[index - 1];
                }

                int neuronIndex = 0;
                for (int j = layers.Length - 1; j >= 0; j--)
                {
                    if (layers[j].ContextFedBy == layer)
                    {
                        _hasContext = true;
                        _contextTargetSize[index] = layers[j].ContextCount;
                        _contextTargetOffset[index] = neuronIndex
                                                     + (layers[j].TotalCount - layers[j].ContextCount);
                    }
                    neuronIndex += layers[j].TotalCount;
                }

                index++;
            }

            _beginTraining = 0;
            _endTraining = _layerCounts.Length - 1;

            _weights = new double[weightCount];
            _layerOutput = new double[neuronCount];
            _layerSums = new double[neuronCount];

            ClearContext();
        }


        /// <summary>
        /// Perform a simple randomization of the weights of the neural network
        /// between the specified hi and lo.
        /// </summary>
        ///
        /// <param name="hi">The network high.</param>
        /// <param name="lo">The network low.</param>
		/// <param name="seed">Pass in an integer here to seed the random number generator or leave null for default behavior (seed from system clock).</param>
        public void Randomize(double hi = 1.0, double lo = -1.0, int? seed = null)
        {
			var random = seed == null ? new Random() : new Random(seed.Value);
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = (random.NextDouble()*(hi - lo)) + lo;
            }
        }

        /// <summary>
        /// The layer sums, before the activation is applied.
        /// </summary>
        public double[] LayerSums
        {
            get { return _layerSums; }
            set { _layerSums = value; }
        }
    }
}
