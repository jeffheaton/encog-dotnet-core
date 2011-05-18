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

        /// <summary>
        /// Construct the object.
        /// </summary>
        public SVMPattern()
        {
            Regression = true;
            kernelType = KernelType.RadialBasisFunction;
            svmType = SVMType.EpsilonSupportVectorRegression;
        }

        /// <summary>
        /// Set if regression is used.
        /// </summary>
        public bool Regression { get; set; }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        public KernelType KernelType
        {
            set { kernelType = value; }
        }


        /// <summary>
        /// Set the SVM type.
        /// </summary>
        public SVMType SVMType
        {
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
        public int InputNeurons
        {
            get { return inputNeurons; }
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get { return outputNeurons; }
            set { outputNeurons = value; }
        }


        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A SVM network can't specify a custom activation function.");
            }
        }

        #endregion
    }
}