using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Error;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Util;

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
        /// The serial ID.
        /// </summary>
        ///
        private const long serialVersionUID = 1L;

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
        ///
        /// <value>The activation functions.</value>
        public IActivationFunction[] ActivationFunctions
        {
            /// <returns>The activation functions.</returns>
            get { return activationFunctions; }
            /// <summary>
            /// Set the activation functions.
            /// </summary>
            ///
            /// <param name="af">The activation functions.</param>
            set { activationFunctions = value; }
        }


        /// <value>the beginTraining to set</value>
        public int BeginTraining
        {
            /// <returns>the beginTraining</returns>
            get { return beginTraining; }
            /// <param name="beginTraining_0">the beginTraining to set</param>
            set { beginTraining = value; }
        }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        ///
        /// <value>The bias activation.</value>
        public double[] BiasActivation
        {
            /// <returns>The bias activation.</returns>
            get { return biasActivation; }
            /// <summary>
            /// Set the bias activation.
            /// </summary>
            ///
            /// <param name="biasActivation_0">The bias activation.</param>
            set { biasActivation = value; }
        }


        /// <value>the connectionLimit to set</value>
        public double ConnectionLimit
        {
            /// <returns>the connectionLimit</returns>
            get { return connectionLimit; }
            /// <param name="connectionLimit_0">the connectionLimit to set</param>
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
        ///
        /// <value>The context target offset.</value>
        public int[] ContextTargetOffset
        {
            /// <returns>The offset of the context target for each layer.</returns>
            get { return contextTargetOffset; }
            /// <summary>
            /// Set the context target offset.
            /// </summary>
            ///
            /// <param name="contextTargetOffset_0">The context target offset.</param>
            set { contextTargetOffset = value; }
        }


        /// <summary>
        /// Set the context target size.
        /// </summary>
        ///
        /// <value>The context target size.</value>
        public int[] ContextTargetSize
        {
            /// <returns>The context target size for each layer. Zero if the layer does
            /// not feed a context layer.</returns>
            get { return contextTargetSize; }
            /// <summary>
            /// Set the context target size.
            /// </summary>
            ///
            /// <param name="contextTargetSize_0">The context target size.</param>
            set { contextTargetSize = value; }
        }


        /// <value>The length of the array the network would encode to.</value>
        public int EncodeLength
        {
            /// <returns>The length of the array the network would encode to.</returns>
            get { return weights.Length; }
        }


        /// <value>the endTraining to set</value>
        public int EndTraining
        {
            /// <returns>the endTraining</returns>
            get { return endTraining; }
            /// <param name="endTraining_0">the endTraining to set</param>
            set { endTraining = value; }
        }


        /// <summary>
        /// Set the hasContext property.
        /// </summary>
        ///
        /// <value>True if the network has context.</value>
        public bool HasContext
        {
            /// <returns>True if this network has context.</returns>
            get { return hasContext; }
            /// <summary>
            /// Set the hasContext property.
            /// </summary>
            ///
            /// <param name="hasContext_0">True if the network has context.</param>
            set { hasContext = value; }
        }


        /// <summary>
        /// Set the input count.
        /// </summary>
        ///
        /// <value>The input count.</value>
        public int InputCount
        {
            /// <returns>The number of input neurons.</returns>
            get { return inputCount; }
            /// <summary>
            /// Set the input count.
            /// </summary>
            ///
            /// <param name="inputCount_0">The input count.</param>
            set { inputCount = value; }
        }


        /// <summary>
        /// Set the layer context count.
        /// </summary>
        ///
        /// <value>The layer context count.</value>
        public int[] LayerContextCount
        {
            /// <returns>The layer context count.</returns>
            get { return layerContextCount; }
            /// <summary>
            /// Set the layer context count.
            /// </summary>
            ///
            /// <param name="layerContextCount_0">The layer context count.</param>
            set { layerContextCount = value; }
        }


        /// <summary>
        /// Set the layer counts.
        /// </summary>
        ///
        /// <value>The layer counts.</value>
        public int[] LayerCounts
        {
            /// <returns>The number of neurons in each layer.</returns>
            get { return layerCounts; }
            /// <summary>
            /// Set the layer counts.
            /// </summary>
            ///
            /// <param name="layerCounts_0">The layer counts.</param>
            set { layerCounts = value; }
        }


        public int[] LayerFeedCounts
        {
            /// <returns>The number of neurons in each layer that are fed by the previous
            /// layer.</returns>
            get { return layerFeedCounts; }
            set { layerFeedCounts = value; }
        }


        /// <summary>
        /// Set the layer index.
        /// </summary>
        ///
        /// <value>The layer index.</value>
        public int[] LayerIndex
        {
            /// <returns>Indexes into the weights for the start of each layer.</returns>
            get { return layerIndex; }
            /// <summary>
            /// Set the layer index.
            /// </summary>
            ///
            /// <param name="i">The layer index.</param>
            set { layerIndex = value; }
        }


        /// <summary>
        /// Set the layer output.
        /// </summary>
        ///
        /// <value>The layer output.</value>
        public double[] LayerOutput
        {
            /// <returns>The output for each layer.</returns>
            get { return layerOutput; }
            /// <summary>
            /// Set the layer output.
            /// </summary>
            ///
            /// <param name="layerOutput_0">The layer output.</param>
            set { layerOutput = value; }
        }


        /// <value>The neuron count.</value>
        public int NeuronCount
        {
            /// <returns>The neuron count.</returns>
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
        ///
        /// <value>The output count.</value>
        public int OutputCount
        {
            /// <returns>The number of output neurons.</returns>
            get { return outputCount; }
            /// <summary>
            /// Set the output count.
            /// </summary>
            ///
            /// <param name="outputCount_0">The output count.</param>
            set { outputCount = value; }
        }


        /// <summary>
        /// Set the weight index.
        /// </summary>
        ///
        /// <value>The weight index.</value>
        public int[] WeightIndex
        {
            /// <returns>The index of each layer in the weight and threshold array.</returns>
            get { return weightIndex; }
            /// <summary>
            /// Set the weight index.
            /// </summary>
            ///
            /// <param name="weightIndex_0">The weight index.</param>
            set { weightIndex = value; }
        }


        /// <summary>
        /// Set the weights.
        /// </summary>
        ///
        /// <value>The weights.</value>
        public double[] Weights
        {
            /// <returns>The index of each layer in the weight and threshold array.</returns>
            get { return weights; }
            /// <summary>
            /// Set the weights.
            /// </summary>
            ///
            /// <param name="weights_0">The weights.</param>
            set { weights = value; }
        }

        /// <value>the isLimited</value>
        public bool Limited
        {
            /// <returns>the isLimited</returns>
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
                weights[i] = ((new Random()).Next()*(hi - lo)) + lo;
            }
        }
    }
}