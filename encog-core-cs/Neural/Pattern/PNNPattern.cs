using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.PNN;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern to create a PNN.
    /// </summary>
    ///
    public class PNNPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// The kernel type.
        /// </summary>
        ///
        private PNNKernelType kernel;

        /// <summary>
        /// The output model.
        /// </summary>
        ///
        private PNNOutputMode outmodel;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int outputNeurons;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public PNNPattern()
        {
            kernel = PNNKernelType.Gaussian;
            outmodel = PNNOutputMode.Regression;
        }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        public PNNKernelType Kernel
        {
            get { return kernel; }
            set { kernel = value; }
        }


        /// <summary>
        /// Set the output model.
        /// </summary>
        public PNNOutputMode Outmodel
        {
            get { return outmodel; }
            set { outmodel = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. PNN networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        ///
        /// <param name="count">The number of hidden neurons.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A PNN network does not have hidden layers.");
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
            var pnn = new BasicPNN(kernel, outmodel,
                                   inputNeurons, outputNeurons);
            return pnn;
        }

        /// <summary>
        /// Set the input neuron count.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int InputNeurons
        {
            /// <returns>The number of input neurons.</returns>
            get { return inputNeurons; }
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
            /// <returns>The number of output neurons.</returns>
            get { return outputNeurons; }
            /// <summary>
            /// Set the output neuron count.
            /// </summary>
            ///
            /// <param name="count">The number of neurons.</param>
            set { outputNeurons = value; }
        }


        /// <summary>
        /// Set the activation function. A PNN uses a linear activation function, so
        /// this method throws an error.
        /// </summary>
        ///
        /// <value>The activation function to use.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Set the activation function. A PNN uses a linear activation function, so
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

        #endregion
    }
}