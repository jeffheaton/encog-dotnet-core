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
        public const double DEFAULT_BIAS_ACTIVATION = 1.0d;

        /// <summary>
        /// The value that indicates that there is no bias activation.
        /// </summary>
        ///
        public const double NO_BIAS_ACTIVATION = 0.0d;

        /// <summary>
        /// The activation types.
        /// </summary>
        ///
        private IActivationFunction[] activationFunctions;

        /// <summary>
        /// The layer that training should begin on.
        /// </summary>
        ///
        private int beginTraining;

        /// <summary>
        /// The bias activation for each layer. This is usually either 1, for a bias,
        /// or zero for no bias.
        /// </summary>
        ///
        private double[] biasActivation;

        /// <summary>
        /// The limit, under which, all a cconnection is not considered to exist.
        /// </summary>
        ///
        private double connectionLimit;

        /// <summary>
        /// The context target for each layer. This is how the backwards connections
        /// are formed for the recurrent neural network. Each layer either has a
        /// zero, which means no context target, or a layer number that indicates the
        /// target layer.
        /// </summary>
        ///
        private int[] contextTargetOffset;

        /// <summary>
        /// The size of each of the context targets. If a layer's contextTargetOffset
        /// is zero, its contextTargetSize should also be zero. The contextTargetSize
        /// should always match the feed count of the targeted context layer.
        /// </summary>
        ///
        private int[] contextTargetSize;

        /// <summary>
        /// The layer that training should end on.
        /// </summary>
        ///
        private int endTraining;

        /// <summary>
        /// True if the network has context.
        /// </summary>
        ///
        private bool hasContext;

        /// <summary>
        /// The number of input neurons in this network.
        /// </summary>
        ///
        private int inputCount;

        /// <summary>
        /// Does this network have some connections disabled.
        /// </summary>
        ///
        private bool isLimited;

        /// <summary>
        /// The number of context neurons in each layer. These context neurons will
        /// feed the next layer.
        /// </summary>
        ///
        private int[] layerContextCount;

        /// <summary>
        /// The number of neurons in each of the layers.
        /// </summary>
        ///
        private int[] layerCounts;

        /// <summary>
        /// The number of neurons in each layer that are actually fed by neurons in
        /// the previous layer. Bias neurons, as well as context neurons, are not fed
        /// from the previous layer.
        /// </summary>
        ///
        private int[] layerFeedCounts;

        /// <summary>
        /// An index to where each layer begins (based on the number of neurons in
        /// each layer).
        /// </summary>
        ///
        private int[] layerIndex;

        /// <summary>
        /// The outputs from each of the neurons.
        /// </summary>
        ///
        private double[] layerOutput;

        /// <summary>
        /// The number of output neurons in this network.
        /// </summary>
        ///
        private int outputCount;

        /// <summary>
        /// The index to where the weights that are stored at for a given layer.
        /// </summary>
        ///
        private int[] weightIndex;

        /// <summary>
        /// The weights for a neural network.
        /// </summary>
        ///
        private double[] weights;

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
                                          DEFAULT_BIAS_ACTIVATION);
                layers[1] = new FlatLayer(act, output,
                                          NO_BIAS_ACTIVATION);
            }
            else if ((hidden1 == 0) || (hidden2 == 0))
            {
                int count = Math.Max(hidden1, hidden2);
                layers = new FlatLayer[3];
                layers[0] = new FlatLayer(linearAct, input,
                                          DEFAULT_BIAS_ACTIVATION);
                layers[1] = new FlatLayer(act, count,
                                          DEFAULT_BIAS_ACTIVATION);
                layers[2] = new FlatLayer(act, output,
                                          NO_BIAS_ACTIVATION);
            }
            else
            {
                layers = new FlatLayer[4];
                layers[0] = new FlatLayer(linearAct, input,
                                          DEFAULT_BIAS_ACTIVATION);
                layers[1] = new FlatLayer(act, hidden1,
                                          DEFAULT_BIAS_ACTIVATION);
                layers[2] = new FlatLayer(act, hidden2,
                                          DEFAULT_BIAS_ACTIVATION);
                layers[3] = new FlatLayer(act, output,
                                          NO_BIAS_ACTIVATION);
            }

            isLimited = false;
            connectionLimit = 0.0d;

            Init(layers);
        }

        /// <summary>
        /// Set the activation functions.
        /// </summary>
        public IActivationFunction[] ActivationFunctions
        {
            get { return activationFunctions; }
            set { activationFunctions = value; }
        }


        /// <value>the beginTraining to set</value>
        public int BeginTraining
        {
            get { return beginTraining; }
            set { beginTraining = value; }
        }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        public double[] BiasActivation
        {
            get { return biasActivation; }
            set { biasActivation = value; }
        }


        /// <value>the connectionLimit to set</value>
        public double ConnectionLimit
        {
            get { return connectionLimit; }
            set
            {
                connectionLimit = value;
                if (Math.Abs(connectionLimit
                             - BasicNetwork.DEFAULT_CONNECTION_LIMIT) < EncogFramework.DEFAULT_DOUBLE_EQUAL)
                {
                    isLimited = true;
                }
            }
        }


        /// <summary>
        /// Set the context target offset.
        /// </summary>
        public int[] ContextTargetOffset
        {
            get { return contextTargetOffset; }
            set { contextTargetOffset = value; }
        }


        /// <summary>
        /// Set the context target size.
        /// </summary>
        public int[] ContextTargetSize
        {
            get { return contextTargetSize; }
            set { contextTargetSize = value; }
        }


        /// <value>The length of the array the network would encode to.</value>
        public int EncodeLength
        {
            get { return weights.Length; }
        }


        /// <value>the endTraining to set</value>
        public int EndTraining
        {
            get { return endTraining; }
            set { endTraining = value; }
        }


        /// <summary>
        /// Set the hasContext property.
        /// </summary>
        public bool HasContext
        {
            get { return hasContext; }
            set { hasContext = value; }
        }


        /// <summary>
        /// Set the input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }


        /// <summary>
        /// Set the layer context count.
        /// </summary>
        public int[] LayerContextCount
        {
            get { return layerContextCount; }
            set { layerContextCount = value; }
        }


        /// <summary>
        /// Set the layer counts.
        /// </summary>
        public int[] LayerCounts
        {
            get { return layerCounts; }
            set { layerCounts = value; }
        }

        /// <summary>
        /// The layer feed counts. The number of neurons in each layer that are fed by the previous
        /// layer.
        /// </summary>
        public int[] LayerFeedCounts
        {
            get { return layerFeedCounts; }
            set { layerFeedCounts = value; }
        }


        /// <summary>
        /// Set the layer index.
        /// </summary>
        public int[] LayerIndex
        {
            get { return layerIndex; }
            set { layerIndex = value; }
        }


        /// <summary>
        /// Set the layer output.
        /// </summary>
        public double[] LayerOutput
        {
            get { return layerOutput; }
            set { layerOutput = value; }
        }


        /// <value>The neuron count.</value>
        public int NeuronCount
        {
            get
            {
                int result = 0;

                foreach (int element  in  layerCounts)
                {
                    result += element;
                }
                return result;
            }
        }


        /// <summary>
        /// Set the output count.
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }


        /// <summary>
        /// Set the weight index.
        /// </summary>
        public int[] WeightIndex
        {
            get { return weightIndex; }
            set { weightIndex = value; }
        }


        /// <summary>
        /// Set the weights.
        /// </summary>
        public double[] Weights
        {
            get { return weights; }
            set { weights = value; }
        }

        /// <value>the isLimited</value>
        public bool Limited
        {
            get { return isLimited; }
        }

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(MLDataSet data)
        {
            var errorCalculation = new ErrorCalculation();

            var actual = new double[outputCount];
            MLDataPair pair = BasicMLDataPair.CreatePair(data.InputSize,
                                                         data.IdealSize);

            for (int i = 0; i < data.Count; i++)
            {
                data.GetRecord(i, pair);
                Compute(pair.InputArray, actual);
                errorCalculation.UpdateError(actual, pair.IdealArray);
            }
            return errorCalculation.Calculate();
        }

        /// <summary>
        /// Clear any connection limits.
        /// </summary>
        ///
        public void ClearConnectionLimit()
        {
            connectionLimit = 0.0d;
            isLimited = false;
        }

        /// <summary>
        /// Clear any context neurons.
        /// </summary>
        ///
        public void ClearContext()
        {
            int index = 0;

            for (int i = 0; i < layerIndex.Length; i++)
            {
                bool hasBias = (layerContextCount[i] + layerFeedCounts[i]) != layerCounts[i];

                // fill in regular neurons
                for (int j = 0; j < layerFeedCounts[i]; j++)
                {
                    layerOutput[index++] = 0;
                }

                // fill in the bias
                if (hasBias)
                {
                    layerOutput[index++] = biasActivation[i];
                }

                // fill in context
                for (int j_0 = 0; j_0 < layerContextCount[i]; j_0++)
                {
                    layerOutput[index++] = 0;
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
            result.inputCount = inputCount;
            result.layerCounts = EngineArray.ArrayCopy(layerCounts);
            result.layerIndex = EngineArray.ArrayCopy(layerIndex);
            result.layerOutput = EngineArray.ArrayCopy(layerOutput);
            result.layerFeedCounts = EngineArray.ArrayCopy(layerFeedCounts);
            result.contextTargetOffset = EngineArray
                .ArrayCopy(contextTargetOffset);
            result.contextTargetSize = EngineArray
                .ArrayCopy(contextTargetSize);
            result.layerContextCount = EngineArray
                .ArrayCopy(layerContextCount);
            result.biasActivation = EngineArray.ArrayCopy(biasActivation);
            result.outputCount = outputCount;
            result.weightIndex = weightIndex;
            result.weights = weights;

            result.activationFunctions = new IActivationFunction[activationFunctions.Length];
            for (int i = 0; i < result.activationFunctions.Length; i++)
            {
                result.activationFunctions[i] = (IActivationFunction) activationFunctions[i].Clone();
            }

            result.beginTraining = beginTraining;
            result.endTraining = endTraining;
        }

        /// <summary>
        /// Calculate the output for the given input.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public virtual void Compute(double[] input, double[] output)
        {
            int sourceIndex = layerOutput.Length
                              - layerCounts[layerCounts.Length - 1];

            EngineArray.ArrayCopy(input, 0, layerOutput, sourceIndex,
                                  inputCount);

            for (int i = layerIndex.Length - 1; i > 0; i--)
            {
                ComputeLayer(i);
            }

            EngineArray.ArrayCopy(layerOutput, 0, output, 0, outputCount);
        }

        /// <summary>
        /// Calculate a layer.
        /// </summary>
        ///
        /// <param name="currentLayer">The layer to calculate.</param>
        protected internal void ComputeLayer(int currentLayer)
        {
            int inputIndex = layerIndex[currentLayer];
            int outputIndex = layerIndex[currentLayer - 1];
            int inputSize = layerCounts[currentLayer];
            int outputSize = layerFeedCounts[currentLayer - 1];

            int index = weightIndex[currentLayer - 1];

            int limitX = outputIndex + outputSize;
            int limitY = inputIndex + inputSize;

            // weight values
            for (int x = outputIndex; x < limitX; x++)
            {
                double sum = 0;
                for (int y = inputIndex; y < limitY; y++)
                {
                    sum += weights[index++]*layerOutput[y];
                }
                layerOutput[x] = sum;
            }

            activationFunctions[currentLayer - 1].ActivationFunction(
                layerOutput, outputIndex, outputSize);

            // update context values
            int offset = contextTargetOffset[currentLayer];

            for (int x = 0; x < contextTargetSize[currentLayer]; x++)
            {
                layerOutput[offset + x] = layerOutput[outputIndex + x];
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
            if (data.Length != weights.Length)
            {
                throw new EncogError(
                    "Incompatable weight sizes, can't assign length="
                    + data.Length + " to length=" + data.Length);
            }
            weights = data;
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
            return weights;
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


            foreach (IActivationFunction activation  in  activationFunctions)
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
            else
            {
                return map[0];
            }
        }

        /// <summary>
        /// Construct a flat network.
        /// </summary>
        ///
        /// <param name="layers">The layers of the network to create.</param>
        public void Init(FlatLayer[] layers)
        {
            int layerCount = layers.Length;

            inputCount = layers[0].Count;
            outputCount = layers[layerCount - 1].Count;

            layerCounts = new int[layerCount];
            layerContextCount = new int[layerCount];
            weightIndex = new int[layerCount];
            layerIndex = new int[layerCount];
            activationFunctions = new IActivationFunction[layerCount];
            layerFeedCounts = new int[layerCount];
            contextTargetOffset = new int[layerCount];
            contextTargetSize = new int[layerCount];
            biasActivation = new double[layerCount];

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

                biasActivation[index] = layer.BiasActivation;
                layerCounts[index] = layer.TotalCount;
                layerFeedCounts[index] = layer.Count;
                layerContextCount[index] = layer.ContextCount;
                activationFunctions[index] = layer.Activation;

                neuronCount += layer.TotalCount;

                if (nextLayer != null)
                {
                    weightCount += layer.Count*nextLayer.TotalCount;
                }

                if (index == 0)
                {
                    weightIndex[index] = 0;
                    layerIndex[index] = 0;
                }
                else
                {
                    weightIndex[index] = weightIndex[index - 1]
                                         + (layerCounts[index]*layerFeedCounts[index - 1]);
                    layerIndex[index] = layerIndex[index - 1]
                                        + layerCounts[index - 1];
                }

                int neuronIndex = 0;
                for (int j = layers.Length - 1; j >= 0; j--)
                {
                    if (layers[j].ContextFedBy == layer)
                    {
                        hasContext = true;
                        contextTargetSize[index] = layers[j].ContextCount;
                        contextTargetOffset[index] = neuronIndex
                                                     + (layers[j].TotalCount - layers[j].ContextCount);
                    }
                    neuronIndex += layers[j].TotalCount;
                }

                index++;
            }

            beginTraining = 0;
            endTraining = layerCounts.Length - 1;

            weights = new double[weightCount];
            layerOutput = new double[neuronCount];

            ClearContext();
        }


        /// <summary>
        /// Perform a simple randomization of the weights of the neural network
        /// between -1 and 1.
        /// </summary>
        ///
        public void Randomize()
        {
            Randomize(1, -1);
        }

        /// <summary>
        /// Perform a simple randomization of the weights of the neural network
        /// between the specified hi and lo.
        /// </summary>
        ///
        /// <param name="hi">The network high.</param>
        /// <param name="lo">The network low.</param>
        public void Randomize(double hi, double lo)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (ThreadSafeRandom.NextDouble()*(hi - lo)) + lo;
            }
        }
    }
}
