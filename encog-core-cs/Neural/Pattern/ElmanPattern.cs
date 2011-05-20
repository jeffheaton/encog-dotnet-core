using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// This class is used to generate an Elman style recurrent neural network. This
    /// network type consists of three regular layers, an input output and hidden
    /// layer. There is also a context layer which accepts output from the hidden
    /// layer and outputs back to the hidden layer. This makes it a recurrent neural
    /// network.
    /// The Elman neural network is useful for temporal input data. The specified
    /// activation function will be used on all layers. The Elman neural network is
    /// similar to the Jordan neural network.
    /// </summary>
    ///
    public class ElmanPattern : NeuralNetworkPattern
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
        /// Create an object to generate Elman neural networks.
        /// </summary>
        ///
        public ElmanPattern()
        {
            inputNeurons = -1;
            outputNeurons = -1;
            hiddenNeurons = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer with the specified number of neurons.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in this hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (hiddenNeurons != -1)
            {
                throw new PatternError(
                    "An Elman neural network should have only one hidden layer.");
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
        /// Generate the Elman neural network.
        /// </summary>
        ///
        /// <returns>The Elman neural network.</returns>
        public MLMethod Generate()
        {
            BasicLayer hidden, input;

            var network = new BasicNetwork();
            network.AddLayer(input = new BasicLayer(activation, true,
                                                    inputNeurons));
            network.AddLayer(hidden = new BasicLayer(activation, true,
                                                     hiddenNeurons));
            network.AddLayer(new BasicLayer(null, false, outputNeurons));
            input.ContextFedBy = hidden;
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set { activation = value; }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons { set { inputNeurons = value; } }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {

            set { outputNeurons = value; }
        }

        #endregion
    }
}