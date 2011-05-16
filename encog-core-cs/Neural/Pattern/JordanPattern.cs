using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// This class is used to generate an Jordan style recurrent neural network. This
    /// network type consists of three regular layers, an input output and hidden
    /// layer. There is also a context layer which accepts output from the output
    /// layer and outputs back to the hidden layer. This makes it a recurrent neural
    /// network.
    /// The Jordan neural network is useful for temporal input data. The specified
    /// activation function will be used on all layers. The Jordan neural network is
    /// similar to the Elman neural network.
    /// </summary>
    ///
    public class JordanPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction activation;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        ///
        private int hiddenNeurons;

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

        /// <summary>
        /// Construct an object to create a Jordan type neural network.
        /// </summary>
        ///
        public JordanPattern()
        {
            inputNeurons = -1;
            outputNeurons = -1;
            hiddenNeurons = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer, there should be only one.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in this hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (hiddenNeurons != -1)
            {
                throw new PatternError(
                    "A Jordan neural network should have only one hidden "
                    + "layer.");
            }

            hiddenNeurons = count;
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public void Clear()
        {
            hiddenNeurons = -1;
        }

        /// <summary>
        /// Generate a Jordan neural network.
        /// </summary>
        ///
        /// <returns>A Jordan neural network.</returns>
        public MLMethod Generate()
        {
            BasicLayer hidden, output;

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(activation, true,
                                            inputNeurons));
            network.AddLayer(hidden = new BasicLayer(activation, true,
                                                     hiddenNeurons));
            network.AddLayer(output = new BasicLayer(null, false,
                                                     outputNeurons));
            hidden.ContextFedBy = output;
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
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
            /// <param name="activation_0">The activation function.</param>
            set { activation = value; }
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