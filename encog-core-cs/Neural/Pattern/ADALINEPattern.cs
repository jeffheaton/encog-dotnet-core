using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Construct an ADALINE neural network.
    /// </summary>
    ///
    public class ADALINEPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// The number of neurons in the output layer.
        /// </summary>
        ///
        private int outputNeurons;

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not used, the ADALINE has no hidden layers, this will throw an error.
        /// </summary>
        ///
        /// <param name="count">The neuron count.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("An ADALINE network has no hidden layers.");
        }

        /// <summary>
        /// Clear out any parameters.
        /// </summary>
        ///
        public void Clear()
        {
            inputNeurons = 0;
            outputNeurons = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            var network = new BasicNetwork();

            Layer inputLayer = new BasicLayer(new ActivationLinear(), true,
                                              inputNeurons);
            Layer outputLayer = new BasicLayer(new ActivationLinear(), false,
                                               outputNeurons);

            network.AddLayer(inputLayer);
            network.AddLayer(outputLayer);
            network.Structure.FinalizeStructure();

            (new RangeRandomizer(-0.5d, 0.5d)).Randomize(network);

            return network;
        }

        /// <summary>
        /// Not used, ADALINE does not use custom activation functions.
        /// </summary>
        ///
        /// <value>Not used.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Not used, ADALINE does not use custom activation functions.
            /// </summary>
            ///
            /// <param name="activation">Not used.</param>
            set
            {
                throw new PatternError(
                    "A ADALINE network can't specify a custom activation function.");
            }
        }


        /// <summary>
        /// Set the input neurons.
        /// </summary>
        ///
        /// <value>The number of neurons in the input layer.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the input neurons.
            /// </summary>
            ///
            /// <param name="count">The number of neurons in the input layer.</param>
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neurons.
        /// </summary>
        ///
        /// <value>The number of neurons in the output layer.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the output neurons.
            /// </summary>
            ///
            /// <param name="count">The number of neurons in the output layer.</param>
            set { outputNeurons = value; }
        }

        #endregion
    }
}