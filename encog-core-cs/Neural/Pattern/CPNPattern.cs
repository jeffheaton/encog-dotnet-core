using System;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.CPN;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern that creates a CPN neural network.
    /// </summary>
    ///
    public class CPNPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The tag for the INSTAR layer.
        /// </summary>
        ///
        public const String TAG_INSTAR = "INSTAR";

        /// <summary>
        /// The tag for the OUTSTAR layer.
        /// </summary>
        ///
        public const String TAG_OUTSTAR = "OUTSTAR";

        /// <summary>
        /// The number of neurons in the hidden layer.
        /// </summary>
        ///
        private int inputCount;

        /// <summary>
        /// The number of neurons in the instar layer.
        /// </summary>
        ///
        private int instarCount;

        /// <summary>
        /// The number of neurons in the outstar layer.
        /// </summary>
        ///
        private int outstarCount;

        /// <summary>
        /// Set the number of neurons in the instar layer. This level is essentially
        /// a hidden layer.
        /// </summary>
        ///
        /// <value>The instar count.</value>
        public int InstarCount
        {
            /// <summary>
            /// Set the number of neurons in the instar layer. This level is essentially
            /// a hidden layer.
            /// </summary>
            ///
            /// <param name="instarCount_0">The instar count.</param>
            set { instarCount = value; }
        }

        /// <summary>
        /// Set the number of neurons in the outstar level, this level is mapped to
        /// the "output" level.
        /// </summary>
        ///
        /// <value>The outstar count.</value>
        public int OutstarCount
        {
            /// <summary>
            /// Set the number of neurons in the outstar level, this level is mapped to
            /// the "output" level.
            /// </summary>
            ///
            /// <param name="outstarCount_0">The outstar count.</param>
            set { outstarCount = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not used, will throw an error. CPN networks already have a predefined
        /// hidden layer called the instar layer.
        /// </summary>
        ///
        /// <param name="count">NOT USED</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError(
                "A CPN already has a predefined hidden layer.  No additional"
                + "specification is needed.");
        }

        /// <summary>
        /// Clear any parameters that were set.
        /// </summary>
        ///
        public void Clear()
        {
            inputCount = 0;
            instarCount = 0;
            outstarCount = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public MLMethod Generate()
        {
            return new CPNNetwork(inputCount, instarCount, outstarCount, 1);
        }

        /// <summary>
        /// This method will throw an error. The CPN network uses predefined
        /// activation functions.
        /// </summary>
        ///
        /// <value>NOT USED</value>
        public IActivationFunction ActivationFunction
        {
            /// <summary>
            /// This method will throw an error. The CPN network uses predefined
            /// activation functions.
            /// </summary>
            ///
            /// <param name="activation">NOT USED</param>
            set
            {
                throw new PatternError(
                    "A CPN network will use the BiPolar & competitive activation "
                    + "functions, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>The input neuron count.</value>
        public int InputNeurons
        {
            /// <summary>
            /// Set the number of input neurons.
            /// </summary>
            ///
            /// <param name="count">The input neuron count.</param>
            set { inputCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons. Calling this method maps to setting the
        /// number of neurons in the outstar layer.
        /// </summary>
        ///
        /// <value>The count.</value>
        public int OutputNeurons
        {
            /// <summary>
            /// Set the number of output neurons. Calling this method maps to setting the
            /// number of neurons in the outstar layer.
            /// </summary>
            ///
            /// <param name="count">The count.</param>
            set { outstarCount = value; }
        }

        #endregion
    }
}