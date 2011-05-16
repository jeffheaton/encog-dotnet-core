using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.SVM;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// A pattern to create support vector machines.
    /// </summary>
    ///
    public class SVMPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// The kernel type.
        /// </summary>
        ///
        private KernelType kernelType;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        ///
        private int outputNeurons;

        /// <summary>
        /// The SVM type.
        /// </summary>
        ///
        private SVMType svmType;

        public SVMPattern()
        {
            Regression = true;
            kernelType = KernelType.RadialBasisFunction;
            svmType = SVMType.EpsilonSupportVectorRegression;
        }

        /// <summary>
        /// Set if regression is used.
        /// </summary>
        ///
        /// <value>True if regression is used.</value>
        public bool Regression { /// <returns>True, if this is regression.</returns>
            get; /// <summary>
            /// Set if regression is used.
            /// </summary>
            ///
            /// <param name="regression_0">True if regression is used.</param>
            set; }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        ///
        /// <value>The kernel type.</value>
        public KernelType KernelType
        {
            /// <summary>
            /// Set the kernel type.
            /// </summary>
            ///
            /// <param name="kernelType_0">The kernel type.</param>
            set { kernelType = value; }
        }


        /// <summary>
        /// Set the SVM type.
        /// </summary>
        ///
        /// <value>The SVM type.</value>
        public SVMType SVMType
        {
            /// <summary>
            /// Set the SVM type.
            /// </summary>
            ///
            /// <param name="svmType_0">The SVM type.</param>
            set { svmType = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Unused, a BAM has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A SVM network has no hidden layers.");
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        ///
        public void Clear()
        {
            inputNeurons = 0;
            outputNeurons = 0;
        }


        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            if (outputNeurons != 1)
            {
                throw new PatternError("A SVM may only have one output.");
            }
            var network = new SupportVectorMachine(inputNeurons, svmType,
                                                   kernelType);
            return network;
        }

        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        public int InputNeurons
        {
            /// <returns>The input neuron count.</returns>
            get { return inputNeurons; }
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
        /// <value>The output neuron count.</value>
        public int OutputNeurons
        {
            /// <returns>The input output count.</returns>
            get { return outputNeurons; }
            /// <summary>
            /// Set the number of output neurons.
            /// </summary>
            ///
            /// <param name="count">The output neuron count.</param>
            set { outputNeurons = value; }
        }


        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        ///
        /// <value>Not used.</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// Not used, the BAM uses a bipoloar activation function.
            /// </summary>
            ///
            /// <param name="activation">Not used.</param>
            set
            {
                throw new PatternError(
                    "A SVM network can't specify a custom activation function.");
            }
        }

        #endregion
    }
}