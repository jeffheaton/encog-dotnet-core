using System;
using Encog.Engine.Network.Activation;
using Encog.Neural.Flat;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// Basic functionality that most of the neural layers require. The basic layer
    /// is often used by itself to implement forward or recurrent layers. Other layer
    /// types are based on the basic layer as well.
    /// The following summarizes how basic layers calculate the output for a neural
    /// network.
    /// Example of a simple XOR network.
    /// Input: BasicLayer: 2 Neurons, null biasWeights, null biasActivation
    /// Hidden: BasicLayer: 2 Neurons, 2 biasWeights, 1 biasActivation
    /// Output: BasicLayer: 1 Neuron, 1 biasWeights, 1 biasActivation
    /// Input1Output and Input2Output are both provided.
    /// Synapse 1: Input to Hidden Hidden1Activation = (Input1Output 
    /// Input1->Hidden1Weight) + (Input2Output/// Input2->Hidden1Weight) +
    /// (HiddenBiasActivation/// Hidden1BiasWeight)
    /// Hidden1Output = calculate(Hidden1Activation, HiddenActivationFunction)
    /// Hidden2Activation = (Input1Output/// Input1->Hidden2Weight) + (Input2Output 
    /// Input2->Hidden2Weight) + (HiddenBiasActivation/// Hidden2BiasWeight)
    /// Hidden2Output = calculate(Hidden2Activation, HiddenActivationFunction)
    /// Synapse 2: Hidden to Output
    /// Output1Activation = (Hidden1Output/// Hidden1->Output1Weight)
    /// + (Hidden2Output 
    /// Hidden2->Output1Weight) + (OutputBiasActivation/// Output1BiasWeight)
    /// Output1Output = calculate(Output1Activation, OutputActivationFunction)
    /// </summary>
    ///
    [Serializable]
    public class BasicLayer : FlatLayer, Layer
    {
        /// <summary>
        /// The network that this layer belongs to.
        /// </summary>
        ///
        private BasicNetwork network;

        /// <summary>
        /// Construct this layer with a non-default activation function, also
        /// determine if a bias is desired or not.
        /// </summary>
        ///
        /// <param name="activationFunction">The activation function to use.</param>
        /// <param name="neuronCount">How many neurons in this layer.</param>
        /// <param name="hasBias">True if this layer has a bias.</param>
        public BasicLayer(IActivationFunction activationFunction,
                          bool hasBias, int neuronCount)
            : base(activationFunction, neuronCount, (hasBias) ? 1.0d : 0.0d)
        {
        }

        /// <summary>
        /// Construct this layer with a sigmoid activation function.
        /// </summary>
        ///
        /// <param name="neuronCount">How many neurons in this layer.</param>
        public BasicLayer(int neuronCount) : this(new ActivationTANH(), true, neuronCount)
        {
        }

        #region Layer Members

        /// <summary>
        /// Set the network for this layer.
        /// </summary>
        ///
        /// <value>The network for this layer.</value>
        public virtual BasicNetwork Network
        {
            /// <returns>The network that owns this layer.</returns>
            get { return network; }
            /// <summary>
            /// Set the network for this layer.
            /// </summary>
            ///
            /// <param name="network_0">The network for this layer.</param>
            set { network = value; }
        }

        /// <summary>
        /// THe neuron count.
        /// </summary>
        public virtual int NeuronCount
        {
            get { return Count; }
        }

        /// <summary>
        /// The activation function.
        /// </summary>
        public virtual IActivationFunction ActivationFunction
        {
            get { return base.Activation; }
        }

        #endregion
    }
}