using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Used to create feedforward neural networks. A feedforward network has an
    /// input and output layers separated by zero or more hidden layers. The
    /// feedforward neural network is one of the most common neural network patterns.
    /// </summary>
    ///
    public class FeedForwardPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        ///
        private readonly IList<Int32> hidden;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction activationHidden;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction activationOutput;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int outputNeurons;

        public FeedForwardPattern()
        {
            hidden = new List<Int32>();
        }

        /// <value>the activationOutput to set</value>
        public IActivationFunction ActivationOutput
        {
            /// <returns>the activationOutput</returns>
            get { return activationOutput; }
            /// <param name="activationOutput_0">the activationOutput to set</param>
            set { activationOutput = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer, with the specified number of neurons.
        /// </summary>
        ///
        /// <param name="count">The number of neurons to add.</param>
        public void AddHiddenLayer(int count)
        {
            hidden.Add(count);
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public void Clear()
        {
            hidden.Clear();
        }

        /// <summary>
        /// Generate the feedforward neural network.
        /// </summary>
        ///
        /// <returns>The feedforward neural network.</returns>
        public MLMethod Generate()
        {
            if (activationOutput == null)
                activationOutput = activationHidden;

            Layer input = new BasicLayer(null, true, inputNeurons);

            var result = new BasicNetwork();
            result.AddLayer(input);


            foreach (Int32 count  in  hidden)
            {
                Layer hidden_0 = new BasicLayer(activationHidden, true,
                                                (count));

                result.AddLayer(hidden_0);
            }

            Layer output = new BasicLayer(activationOutput, false,
                                          outputNeurons);
            result.AddLayer(output);

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Set the activation function to use on each of the layers.
            /// </summary>
            ///
            /// <param name="activation">The activation function.</param>
            set { activationHidden = value; }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>Neuron count.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the number of input neurons.
            /// </summary>
            ///
            /// <param name="count">Neuron count.</param>
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        ///
        /// <value>Neuron count.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the number of output neurons.
            /// </summary>
            ///
            /// <param name="count">Neuron count.</param>
            set { outputNeurons = value; }
        }

        #endregion
    }
}