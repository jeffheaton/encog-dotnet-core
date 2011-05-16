using Encog.Engine.Network.Activation;
using Encog.MathUtil.RBF;
using Encog.ML;
using Encog.Neural.RBF;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// A radial basis function (RBF) network uses several radial basis functions to
    /// provide a more dynamic hidden layer activation function than many other types
    /// of neural network. It consists of a input, output and hidden layer.
    /// </summary>
    ///
    public class RadialBasisPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int hiddenNeurons;

        /// <summary>
        /// The number of input neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int outputNeurons;

        private RBFEnum rbfType;

        public RadialBasisPattern()
        {
            rbfType = RBFEnum.Gaussian;
            inputNeurons = -1;
            outputNeurons = -1;
            hiddenNeurons = -1;
        }

        public RBFEnum RBF
        {
            set { rbfType = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add the hidden layer, this should be called once, as a RBF has a single
        /// hidden layer.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in the hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (hiddenNeurons != -1)
            {
                throw new PatternError("A RBF network usually has a single "
                                       + "hidden layer.");
            }
            else
            {
                hiddenNeurons = count;
            }
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
        /// Generate the RBF network.
        /// </summary>
        ///
        /// <returns>The neural network.</returns>
        public MLMethod Generate()
        {
            var result = new RBFNetwork(inputNeurons, hiddenNeurons,
                                        outputNeurons, rbfType);
            return result;
        }

        /// <summary>
        /// Set the activation function, this is an error. The activation function
        /// may not be set on a RBF layer.
        /// </summary>
        ///
        /// <value>The new activation function.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Set the activation function, this is an error. The activation function
            /// may not be set on a RBF layer.
            /// </summary>
            ///
            /// <param name="activation">The new activation function.</param>
            set
            {
                throw new PatternError("Can't set the activation function for "
                                       + "a radial basis function network.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the number of input neurons.
            /// </summary>
            ///
            /// <param name="count">The number of input neurons.</param>
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        ///
        /// <value>The number of output neurons.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the number of output neurons.
            /// </summary>
            ///
            /// <param name="count">The number of output neurons.</param>
            set { outputNeurons = value; }
        }

        #endregion
    }
}