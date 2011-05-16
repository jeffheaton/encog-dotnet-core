using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.SOM;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// A self organizing map is a neural network pattern with an input and output
    /// layer. There is no hidden layer. The winning neuron, which is that neuron
    /// with the higest output is the winner, this winning neuron is often used to
    /// classify the input into a group.
    /// </summary>
    ///
    public class SOMPattern : NeuralNetworkPattern
    {
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

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. SOM networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        ///
        /// <param name="count">The number of hidden neurons.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A SOM network does not have hidden layers.");
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the RSOM network.
        /// </summary>
        ///
        /// <returns>The neural network.</returns>
        public MLMethod Generate()
        {
            var som = new SOMNetwork(inputNeurons, outputNeurons);
            som.Reset();
            return som;
        }

        /// <summary>
        /// Set the activation function. A SOM uses a linear activation function, so
        /// this method throws an error.
        /// </summary>
        ///
        /// <value>The activation function to use.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Set the activation function. A SOM uses a linear activation function, so
            /// this method throws an error.
            /// </summary>
            ///
            /// <param name="activation">The activation function to use.</param>
            set
            {
                throw new PatternError(
                    "A SOM network can't define an activation function.");
            }
        }


        /// <summary>
        /// Set the input neuron count.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the input neuron count.
            /// </summary>
            ///
            /// <param name="count">The number of neurons.</param>
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron count.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the output neuron count.
            /// </summary>
            ///
            /// <param name="count">The number of neurons.</param>
            set { outputNeurons = value; }
        }

        #endregion
    }
}