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
    /// Vector based neural networks are also very good for GPU processing. The flat
    /// network classes will make use of the GPU if you have enabled GPU processing.
    /// See the Encog class for more info.
    /// </summary>
    public class FlatNetwork
    {
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
        /// Are we using the TANH function? If not, then the sigmoid.
        /// </summary>
        private bool tanh;

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

            int index = 0;
            int neuronCount = 0;

            foreach (ILayer layer in network.Structure.Layers)
            {
                layerCounts[index] = layer.NeuronCount;
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

            if (input.ActivationFunction is ActivationSigmoid)
            {
                tanh = false;
            }
            else
            {
                tanh = true;
            }
        }

        /// <summary>
        /// Generate a regular Encog neural network from this flat network. 
        /// </summary>
        /// <returns>A regular Encog neural network.</returns>
        public BasicNetwork Unflatten()
        {
            IActivationFunction activation;
            BasicNetwork result = new BasicNetwork();

            if (this.tanh)
                activation = new ActivationTANH();
            else
                activation = new ActivationSigmoid();

            for (int i = this.layerCounts.Length - 1; i >= 0; i--)
            {
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
        /// Calculate the output for the given input, using the GPU(if enabled).
        /// Normally, you would not want to calculate a single neural network
        /// with the GPU, as it would be faster to use the CPU.  However
        /// this can be a quick test to verify the GPU is online and working
        /// with Encog.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public void ComputeForceGPU(double[] input, double[] output)
        {
            if (Encog.Instance.GPU != null)
            {
                Encog.Instance.GPU.ChooseAdapter().SingleNetworkCalculate.Calculate(this, input, output);
            }
            else
                throw new NeuralNetworkError("GPU processing is not enabled.");
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

                if (tanh)
                {
                    layerOutput[outputIndex + x] = ActivateTANH(layerOutput[outputIndex + x]);
                }
                else
                {
                    layerOutput[outputIndex + x] = ActivateSigmoid(layerOutput[outputIndex
                            + x]);
                }
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
        /// True if this is a TANH activation function.
        /// </summary>
        public bool IsTanh
        {
            get
            {
                return tanh;
            }
        }

        /// <summary>
        /// Implements a sigmoid activation function. 
        /// </summary>
        /// <param name="d">The value to take the sigmoid of.</param>
        /// <returns>The result.</returns>
        private double ActivateSigmoid(double d)
        {
            return 1.0 / (1 + BoundMath.Exp(-1.0 * d));
        }

        /// <summary>
        /// Implements a hyperbolic tangent function.
        /// </summary>
        /// <param name="d">The value to take the htan of.</param>
        /// <returns>The htan of the specified value.</returns>
        private double ActivateTANH(double d)
        {
            return -1 + (2 / (1 + BoundMath.Exp(-2 * d)));
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

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateErrorGPU(INeuralDataSet data)
        {
            ErrorCalculation errorCalculation = new ErrorCalculation();

            double[][] actual = Encog.Instance.GPU.ChooseAdapter().NetworkCalculate.Calculate(this, data);

            int index = 0;
            foreach (INeuralDataPair pair in data)
            {
                errorCalculation.UpdateError(actual[index++], pair.Ideal.Data);
            }
            return errorCalculation.CalculateRMS();
        }

    }
}
