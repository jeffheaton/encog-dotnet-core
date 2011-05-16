using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.BAM;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Construct a Bidirectional Access Memory (BAM) neural network. This neural
    /// network type learns to associate one pattern with another. The two patterns
    /// do not need to be of the same length. This network has two that are connected
    /// to each other. Though they are labeled as input and output layers to Encog,
    /// they are both equal, and should simply be thought of as the two layers that
    /// make up the net.
    /// </summary>
    ///
    public class BAMPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        ///
        private int f1Neurons;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        ///
        private int f2Neurons;

        /// <summary>
        /// Set the F1 neurons. The BAM really does not have an input and output
        /// layer, so this is simply setting the number of neurons that are in the
        /// first layer.
        /// </summary>
        ///
        /// <value>The number of neurons in the first layer.</value>
        public int F1Neurons
        {
            /// <summary>
            /// Set the F1 neurons. The BAM really does not have an input and output
            /// layer, so this is simply setting the number of neurons that are in the
            /// first layer.
            /// </summary>
            ///
            /// <param name="count">The number of neurons in the first layer.</param>
            set { f1Neurons = value; }
        }


        /// <summary>
        /// Set the output neurons. The BAM really does not have an input and output
        /// layer, so this is simply setting the number of neurons that are in the
        /// second layer.
        /// </summary>
        ///
        /// <value>The number of neurons in the second layer.</value>
        public int F2Neurons
        {
            /// <summary>
            /// Set the output neurons. The BAM really does not have an input and output
            /// layer, so this is simply setting the number of neurons that are in the
            /// second layer.
            /// </summary>
            ///
            /// <param name="count">The number of neurons in the second layer.</param>
            set { f2Neurons = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Unused, a BAM has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A BAM network has no hidden layers.");
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        ///
        public void Clear()
        {
            f1Neurons = 0;
            f2Neurons = 0;
        }


        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            var bam = new BAMNetwork(f1Neurons, f2Neurons);
            return bam;
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
                    "A BAM network can't specify a custom activation function.");
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
            set
            {
                throw new PatternError(
                    "A BAM network has no input layer, consider setting F1 layer.");
            }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        ///
        /// <value>The output neuron count.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the number of output neurons.
            /// </summary>
            ///
            /// <param name="count">The output neuron count.</param>
            set
            {
                throw new PatternError(
                    "A BAM network has no output layer, consider setting F2 layer.");
            }
        }

        #endregion
    }
}