using Encog.Engine.Network.Activation;
using Encog.ML;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Patterns are used to create common sorts of neural networks. Information
    /// about the structure of the neural network is communicated to the pattern, and
    /// then generate is called to produce a neural network of this type.
    /// </summary>
    ///
    public interface NeuralNetworkPattern
    {
        /// <summary>
        /// Set the activation function to be used for all created layers that allow
        /// an activation function to be specified. Not all patterns allow the
        /// activation function to be specified.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        IActivationFunction ActivationFunction { /// <summary>
            /// Set the activation function to be used for all created layers that allow
            /// an activation function to be specified. Not all patterns allow the
            /// activation function to be specified.
            /// </summary>
            ///
            /// <param name="activation">The activation function.</param>
            set; }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        int InputNeurons { /// <summary>
            /// Set the number of input neurons.
            /// </summary>
            ///
            /// <param name="count">The number of input neurons.</param>
            set; }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        ///
        /// <value>The output neuron count.</value>
        int OutputNeurons { /// <summary>
            /// Set the number of output neurons.
            /// </summary>
            ///
            /// <param name="count">The output neuron count.</param>
            set; }

        /// <summary>
        /// Add the specified hidden layer.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in the hidden layer.</param>
        void AddHiddenLayer(int count);

        /// <summary>
        /// Clear the hidden layers so that they can be redefined.
        /// </summary>
        ///
        void Clear();

        /// <summary>
        /// Generate the specified neural network.
        /// </summary>
        ///
        /// <returns>The resulting neural network.</returns>
        MLMethod Generate();
    }
}