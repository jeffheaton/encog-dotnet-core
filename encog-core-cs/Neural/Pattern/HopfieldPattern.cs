using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Thermal;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Create a Hopfield pattern. A Hopfield neural network has a single layer that
    /// functions both as the input and output layers. There are no hidden layers.
    /// Hopfield networks are used for basic pattern recognition. When a Hopfield
    /// network recognizes a pattern, it "echos" that pattern on the output.
    /// </summary>
    ///
    public class HopfieldPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// How many neurons in the Hopfield network. Default to -1, which is
        /// invalid. Therefore this value must be set.
        /// </summary>
        ///
        private int neuronCount;

        public HopfieldPattern()
        {
            neuronCount = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. This will throw an error, because the Hopfield neural
        /// network has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">The number of neurons.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A Hopfield network has no hidden layers.");
        }

        /// <summary>
        /// Nothing to clear.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the Hopfield neural network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            var logic = new HopfieldNetwork(neuronCount);
            return logic;
        }

        /// <summary>
        /// Set the activation function to use. This function will throw an error,
        /// because the Hopfield network must use the BiPolar activation function.
        /// </summary>
        ///
        /// <value>The activation function to use.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Set the activation function to use. This function will throw an error,
            /// because the Hopfield network must use the BiPolar activation function.
            /// </summary>
            ///
            /// <param name="activation">The activation function to use.</param>
            set
            {
                throw new PatternError(
                    "A Hopfield network will use the BiPolar activation "
                    + "function, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons, this must match the output neurons.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the number of input neurons, this must match the output neurons.
            /// </summary>
            ///
            /// <param name="count">The number of neurons.</param>
            set { neuronCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons, should not be used with a hopfield
        /// neural network, because the number of input neurons defines the number of
        /// output neurons.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the number of output neurons, should not be used with a hopfield
            /// neural network, because the number of input neurons defines the number of
            /// output neurons.
            /// </summary>
            ///
            /// <param name="count">The number of neurons.</param>
            set
            {
                throw new PatternError(
                    "A Hopfield network has a single layer, so no need "
                    + "to specify the output count.");
            }
        }

        #endregion
    }
}