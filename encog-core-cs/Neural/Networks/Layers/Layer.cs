using Encog.Engine.Network.Activation;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// This interface defines all necessary methods for a neural network layer.
    /// </summary>
    ///
    public interface Layer
    {
        /// <value>The activation function used for this layer.</value>
        IActivationFunction ActivationFunction { /// <returns>The activation function used for this layer.</returns>
            get; }


        /// <summary>
        /// Set the network that this layer belongs to.
        /// </summary>
        ///
        /// <value>The network.</value>
        BasicNetwork Network { /// <returns>The network that this layer is attached to.</returns>
            get;
            /// <summary>
            /// Set the network that this layer belongs to.
            /// </summary>
            ///
            /// <param name="network">The network.</param>
            set; }


        /// <value>The neuron count.</value>
        int NeuronCount { /// <returns>The neuron count.</returns>
            get; }


        /// <summary>
        /// Most layer types will default this value to one. However, it is possible
        /// to use other values. This is the activation that will be passed over the
        /// bias weights to the inputs of this layer. See the Layer interface
        /// documentation for more information on how Encog handles bias values.
        /// </summary>
        ///
        /// <value>The activation for the bias weights.</value>
        double BiasActivation { /// <summary>
            /// Most layer types will default this value to one. However, it is possible
            /// to use other values. This is the activation that will be passed over the
            /// bias weights to the inputs of this layer. See the Layer interface
            /// documentation for more information on how Encog handles bias values.
            /// </summary>
            ///
            /// <returns>The bias activation for this layer.</returns>
            get;
            /// <summary>
            /// Most layer types will default this value to one. However, it is possible
            /// to use other values. This is the activation that will be passed over the
            /// bias weights to the inputs of this layer. See the Layer interface
            /// documentation for more information on how Encog handles bias values.
            /// </summary>
            ///
            /// <param name="activation">The activation for the bias weights.</param>
            set; }


        /// <summary>
        /// Set the activation function.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        IActivationFunction Activation { /// <summary>
            /// Set the activation function.
            /// </summary>
            ///
            /// <param name="activation">The activation function.</param>
            set; }

        /// <returns>True if this layer has a bias.</returns>
        bool HasBias();
    }
}