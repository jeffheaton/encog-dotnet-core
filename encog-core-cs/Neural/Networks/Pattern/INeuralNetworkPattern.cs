using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Activation;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Patterns are used to create common sorts of neural networks.
    /// Information about the structure of the neural network is 
    /// communicated to the pattern, and then generate is called to
    /// produce a neural network of this type.
    /// </summary>
    public interface INeuralNetworkPattern
    {
        /// <summary>
        /// Add the specified hidden layer.
        /// </summary>
        /// <param name="count">The number of neurons in the hidden layer.</param>
        void AddHiddenLayer(int count);

        /// <summary>
        /// Generate the specified neural network.
        /// </summary>
        /// <returns>The resulting neural network.</returns>
        BasicNetwork Generate();

        /// <summary>
        /// Set the activation function to be used for all created layers
        /// that allow an activation function to be specified.  Not all
        /// patterns allow the activation function to be specified.
        /// </summary>
        IActivationFunction ActivationFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        int InputNeurons
        {
            get;
            set;
        }

        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        int OutputNeurons
        {
            get;
            set;
        }

        /// <summary>
        /// Clear the hidden layers so that they can be redefined.
        /// </summary>
        void Clear();
    }
}
