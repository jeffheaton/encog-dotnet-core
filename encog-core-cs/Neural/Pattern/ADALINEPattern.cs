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
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A ADALINE network can't specify a custom activation function.");
            }
        }


        /// <summary>
        /// Set the input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { outputNeurons = value; }
        }

        #endregion
    }
}