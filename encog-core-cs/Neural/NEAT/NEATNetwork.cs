using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.Simple;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Implements a NEAT network as a synapse between two layers. In Encog, a NEAT
    /// network is created by using a NEATSynapse between an input and output layer.
    /// NEAT networks only have an input and an output layer. There are no actual
    /// hidden layers. Rather this synapse will evolve many hidden neurons that have
    /// connections that are not easily defined by layers. Connections can be
    /// feedforward, recurrent, or self-connected.
    /// NEAT networks relieve the programmer of the need to define the hidden layer
    /// structure of the neural network.
    /// The output from the neural network can be calculated normally or using a
    /// snapshot. The snapshot mode is slower, but it can be more accurate. The
    /// snapshot handles recurrent layers better, as it takes the time to loop
    /// through the network multiple times to "flush out" the recurrent links.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class NEATNetwork : BasicML, MLContext, MLRegression,
                               MLError
    {
        /// <summary>
        /// The depth property.
        /// </summary>
        public const String PROPERTY_NETWORK_DEPTH = "depth";

        /// <summary>
        /// The links property.
        /// </summary>
        public const String PROPERTY_LINKS = "links";

        /// <summary>
        /// The snapshot property.
        /// </summary>
        public const String PROPERTY_SNAPSHOT = "snapshot";

        /// <summary>
        /// The neurons that make up this network.
        /// </summary>
        ///
        private readonly IList<NEATNeuron> neurons;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction activationFunction;

        private int inputCount;

        /// <summary>
        /// The depth of the network.
        /// </summary>
        ///
        private int networkDepth;

        private IActivationFunction outputActivationFunction;

        private int outputCount;

        /// <summary>
        /// Should snapshot be used to calculate the output of the neural network.
        /// </summary>
        ///
        private bool snapshot;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public NEATNetwork()
        {
            neurons = new List<NEATNeuron>();
            snapshot = false;
        }

        /// <summary>
        /// Construct a NEAT synapse.
        /// </summary>
        ///
        /// <param name="inputCount_0">The number of input neurons.</param>
        /// <param name="outputCount_1">The number of output neurons.</param>
        /// <param name="neurons_2">The neurons in this synapse.</param>
        /// <param name="activationFunction_3">The activation function to use.</param>
        /// <param name="networkDepth_5">The depth of the network.</param>
        public NEATNetwork(int inputCount_0, int outputCount_1,
                           IList<NEATNeuron> neurons_2,
                           IActivationFunction activationFunction_3,
                           IActivationFunction outputActivationFunction_4,
                           int networkDepth_5)
        {
            neurons = new List<NEATNeuron>();
            snapshot = false;
            inputCount = inputCount_0;
            outputCount = outputCount_1;
            outputActivationFunction = outputActivationFunction_4;

            foreach (NEATNeuron neuron in neurons_2)
            {
                neurons.Add(neuron);
            }

            networkDepth = networkDepth_5;
            activationFunction = activationFunction_3;
        }

        /// <summary>
        /// Construct a NEAT network.
        /// </summary>
        ///
        /// <param name="inputCount_0">The input count.</param>
        /// <param name="outputCount_1">The output count.</param>
        public NEATNetwork(int inputCount_0, int outputCount_1)
        {
            neurons = new List<NEATNeuron>();
            snapshot = false;
            inputCount = inputCount_0;
            outputCount = outputCount_1;
            networkDepth = 0;
            activationFunction = new ActivationSigmoid();
        }

        /// <summary>
        /// Set the activation function.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        public IActivationFunction ActivationFunction
        {
            /// <returns>The activation function.</returns>
            get { return activationFunction; }
            /// <summary>
            /// Set the activation function.
            /// </summary>
            ///
            /// <param name="activationFunction_0">The activation function.</param>
            set { activationFunction = value; }
        }

        /// <summary>
        /// The network depth.
        /// </summary>
        public int NetworkDepth
        {
            get { return networkDepth; }
            set { networkDepth = value; }
        }


        /// <value>The NEAT neurons.</value>
        public IList<NEATNeuron> Neurons
        {
            get { return neurons; }
        }


        /// <summary>
        /// Sets if snapshot is used.
        /// </summary>
        ///
        /// <value>True if snapshot is used.</value>
        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        /// <value>the outputActivationFunction to set</value>
        public IActivationFunction OutputActivationFunction
        {
            get { return outputActivationFunction; }
            set { outputActivationFunction = value; }
        }

        #region MLContext Members

        /// <summary>
        /// Clear any context from previous runs. This sets the activation of all
        /// neurons to zero.
        /// </summary>
        ///
        public virtual void ClearContext()
        {
            foreach (NEATNeuron neuron  in  neurons)
            {
                neuron.Output = 0;
            }
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network. 
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public virtual double CalculateError(MLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        ///
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public virtual MLData Compute(MLData input)
        {
            MLData result = new BasicMLData(outputCount);

            if (neurons.Count == 0)
            {
                throw new NeuralNetworkError(
                    "This network has not been evolved yet, it has no neurons in the NEAT synapse.");
            }

            int flushCount = 1;

            if (snapshot)
            {
                flushCount = networkDepth;
            }

            // iterate through the network FlushCount times
            for (int i = 0; i < flushCount; ++i)
            {
                int outputIndex = 0;
                int index = 0;

                result.Clear();

                // populate the input neurons
                while (neurons[index].NeuronType == NEATNeuronType.Input)
                {
                    neurons[index].Output = input[index];

                    index++;
                }

                // set the bias neuron
                neurons[index++].Output = 1;

                while (index < neurons.Count)
                {
                    NEATNeuron currentNeuron = neurons[index];

                    double sum = 0;


                    foreach (NEATLink link  in  currentNeuron.InboundLinks)
                    {
                        double weight = link.Weight;
                        double neuronOutput = link.FromNeuron.Output;
                        sum += weight*neuronOutput;
                    }

                    var d = new double[1];
                    d[0] = sum/currentNeuron.ActivationResponse;
                    activationFunction.ActivationFunction(d, 0, d.Length);

                    neurons[index].Output = d[0];

                    if (currentNeuron.NeuronType == NEATNeuronType.Output)
                    {
                        result[outputIndex++] = currentNeuron.Output;
                    }
                    index++;
                }
            }

            outputActivationFunction.ActivationFunction(result.Data, 0,
                                                        result.Count);

            return result;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public virtual int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }

        /// <summary>
        /// The output count.
        /// </summary>
        public virtual int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }

        #endregion

        /// <summary>
        /// Not needed.
        /// </summary>
        public override void UpdateProperties()
        {
        }
    }
}