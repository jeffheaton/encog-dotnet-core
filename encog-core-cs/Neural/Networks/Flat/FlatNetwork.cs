using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Activation;
using Encog.Util;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Implements a flat (vector based) neural network in Encog. This is meant to be
    /// a very highly efficient feedforward neural network. It uses a minimum of
    /// objects and is designed with one principal in mind-- SPEED. Readability, code
    /// reuse, object oriented programming are all secondary in consideration.
    /// 
    /// Currently, the flat networks only support feedforward networks with either a
    /// sigmoid or tanh activation function.  Specifically, a flat network must:
    /// 
    /// 1. Feedforward only, no self-connections or recurrent links
    /// 2. Sigmoid or TANH activation only
    /// 3. All layers the same activation function
    /// 4. Must have threshold values
    /// 
    /// Vector based neural networks are also very good for CL processing. The flat
    /// network classes will make use of the CL if you have enabled CL processing.
    /// See the Encog class for more info.
    /// </summary>
    public class FlatNetwork
    {
        public const int ACTIVATION_LINEAR = 0;
        public const int ACTIVATION_TANH = 1;
        public const int ACTIVATION_SIGMOID = 2;

        /// <summary>
        /// The number of input neurons in this network.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The number of neurons in each of the layers.
        /// </summary>
        private int[] layerCounts;

        /// <summary>
        /// An index to where each layer begins (based on the number of neurons in
        /// each layer).
        /// </summary>
        private int[] layerIndex;

        /// <summary>
        /// The outputs from each of the neurons.
        /// </summary>
        private double[] layerOutput;

        /// <summary>
        /// The number of output neurons in this network.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The index to where the weights and thresholds are stored at for a given
        /// layer.
        /// </summary>
        private int[] weightIndex;

        /// <summary>
        /// The weights and thresholds for a neural network.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The activation types.
        /// </summary>
        private int[] activationType;

        /// <summary>
        /// Construct a flat network.
        /// </summary>
        /// <param name="network">The network to construct the flat network from.</param>
        public FlatNetwork(BasicNetwork network)
        {
            ValidateForFlat.ValidateNetwork(network);

            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer output = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            inputCount = input.NeuronCount;
            outputCount = output.NeuronCount;

            int layerCount = network.Structure.Layers.Count;

            layerCounts = new int[layerCount];
            weightIndex = new int[layerCount];
            layerIndex = new int[layerCount];
            activationType = new int[layerCount];

            int index = 0;
            int neuronCount = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                layerCounts[index] = layer.NeuronCount;

                if (layer.ActivationFunction is ActivationLinear)
                    activationType[index] = FlatNetwork.ACTIVATION_LINEAR;
                else if (layer.ActivationFunction is ActivationTANH)
                    activationType[index] = FlatNetwork.ACTIVATION_TANH;
                else if (layer.ActivationFunction is ActivationSigmoid)
                    activationType[index] = FlatNetwork.ACTIVATION_SIGMOID;

                neuronCount += layer.NeuronCount;

                if (index == 0)
                {
                    weightIndex[index] = 0;
                    layerIndex[index] = 0;
                }
                else
                {
                    weightIndex[index] = weightIndex[index - 1]
                            + (layerCounts[index - 1] + (layerCounts[index] * layerCounts[index - 1]));
                    layerIndex[index] = layerIndex[index - 1]
                            + layerCounts[index - 1];
                }

                index++;
            }

            weights = NetworkCODEC.NetworkToArray(network);
            layerOutput = new double[neuronCount];

        }

        /// <summary>
        /// Generate a regular Encog neural network from this flat network. 
        /// </summary>
        /// <returns>A regular Encog neural network.</returns>
        public BasicNetwork Unflatten()
        {
            BasicNetwork result = new BasicNetwork();

            for (int i = this.layerCounts.Length - 1; i >= 0; i--)
            {
                IActivationFunction activation;

                switch (this.activationType[i])
                {
                    case FlatNetwork.ACTIVATION_LINEAR:
                        activation = new ActivationLinear();
                        break;
                    case FlatNetwork.ACTIVATION_SIGMOID:
                        activation = new ActivationSigmoid();
                        break;
                    case FlatNetwork.ACTIVATION_TANH:
                        activation = new ActivationTANH();
                        break;
                    default:
                        activation = null;
                        break;
                }

                ILayer layer = new BasicLayer(activation, true, this.layerCounts[i]);
                result.AddLayer(layer);
            }
            result.Structure.FinalizeStructure();

            NetworkCODEC.ArrayToNetwork(this.weights, result);

            return result;
        }

        /// <summary>
        /// Calculate the output for the given input. 
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public void Compute(double[] input, double[] output)
        {
            int sourceIndex = layerOutput.Length - inputCount;

            EncogArray.ArrayCopy(input, 0, layerOutput, sourceIndex, inputCount);

            for (int i = layerIndex.Length - 1; i > 0; i--)
            {
                ComputeLayer(i);
            }

            EncogArray.ArrayCopy(layerOutput, 0, output, 0, outputCount);
        }


        /// <summary>
        /// Calculate a layer. 
        /// </summary>
        /// <param name="currentLayer">The layer to calculate.</param>
        private void ComputeLayer(int currentLayer)
        {

            int inputIndex = layerIndex[currentLayer];
            int outputIndex = layerIndex[currentLayer - 1];
            int inputSize = layerCounts[currentLayer];
            int outputSize = layerCounts[currentLayer - 1];

            int index = weightIndex[currentLayer - 1];

            // threshold values
            for (int i = 0; i < outputSize; i++)
            {
                layerOutput[i + outputIndex] = weights[index++];
            }

            // weight values
            for (int x = 0; x < outputSize; x++)
            {
                double sum = 0;
                for (int y = 0; y < inputSize; y++)
                {
                    sum += weights[index++] * layerOutput[inputIndex + y];
                }
                layerOutput[outputIndex + x] += sum;

                layerOutput[outputIndex + x] = FlatNetwork.CalculateActivation(
                    this.activationType[0],
                    layerOutput[outputIndex + x]);
            }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputCount
        {
            get
            {
                return inputCount;
            }
        }

        /// <summary>
        /// The number of neurons in each layer.
        /// </summary>
        public int[] LayerCounts
        {
            get
            {
                return layerCounts;
            }
        }

        /// <summary>
        /// Indexes into the weights for the start of each layer.
        /// </summary>
        public int[] LayerIndex
        {
            get
            {
                return layerIndex;
            }
        }

        /// <summary>
        /// The output for each layer.
        /// </summary>
        public double[] LayerOutput
        {
            get
            {
                return layerOutput;
            }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get
            {
                return outputCount;
            }
        }

        /// <summary>
        /// The index of each layer in the weight and threshold array.
        /// </summary>
        public int[] WeightIndex
        {
            get
            {
                return weightIndex;
            }
        }

        /// <summary>
        /// The index of each layer in the weight and threshold array.
        /// </summary>
        public double[] Weights
        {
            get
            {
                return weights;
            }
        }

        /// <summary>
        /// Clone the network.
        /// </summary>
        /// <returns>A clone of the network.</returns>
        public FlatNetwork Clone()
        {
            BasicNetwork temp = this.Unflatten();
            return new FlatNetwork(temp);
        }

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(INeuralDataSet data)
        {
            ErrorCalculation errorCalculation = new ErrorCalculation();

            double[] actual = new double[OutputCount];

            foreach (INeuralDataPair pair in data)
            {
                Compute(pair.Input.Data, actual);
                errorCalculation.UpdateError(actual, pair.Ideal.Data);
            }
            return errorCalculation.CalculateRMS();
        }


        public int[] ActivationType
        {
            get
            {
                return this.activationType;
            }
        }

        public static double CalculateActivation(int type, double x)
        {
            switch(type)
            {
                case FlatNetwork.ACTIVATION_LINEAR:
                    return x;
                case FlatNetwork.ACTIVATION_TANH:
                    return -1.0 + (2.0 / (1.0 + BoundMath.Exp(-2.0 * x)));
                case FlatNetwork.ACTIVATION_SIGMOID:
                    return 1.0 / (1.0 + BoundMath.Exp(-1.0 * x));
                default:
                    throw new NeuralNetworkError("Unknown activation type: " + type);
            }
        }

        public static double CalculateActivationDerivative(int type, double x)
        {
            switch (type)
            {
                case FlatNetwork.ACTIVATION_LINEAR:
                    return 1;
                case FlatNetwork.ACTIVATION_TANH:
                    return (1.0 + x) * (1.0 - x);
                case FlatNetwork.ACTIVATION_SIGMOID:
                    return x * (1.0 - x);
                default:
                    throw new NeuralNetworkError("Unknown activation type: " + type);
            }
        }

    }
}
