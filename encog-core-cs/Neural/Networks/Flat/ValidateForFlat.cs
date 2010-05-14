using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Only certain types of networks can be converted to a flat network.
    /// This class validates this.  Specifically the network must be:
    /// 
    /// 1. Feedforward only, no self-connections or recurrent links
    /// 2. Sigmoid or TANH activation only
    /// 3. All layers the same activation function
    /// 4. Must have bias weight values
    /// </summary>
    public class ValidateForFlat
    {
        /// <summary>
        /// Validate the specified network. 
        /// </summary>
        /// <param name="network">The network to validate.</param>
        public static void ValidateNetwork(BasicNetwork network)
        {
            IActivationFunction lastActivation = null;

            foreach (ILayer layer in network.Structure.Layers)
            {
                // only feedforward
                if (layer.Next.Count > 1)
                {
                    throw new NeuralNetworkError(
                            "To convert to flat a network must be feedforward only.");
                }

                if (!(layer.ActivationFunction is ActivationSigmoid)
                        && !(layer.ActivationFunction is ActivationTANH))
                {
                    throw new NeuralNetworkError(
                            "To convert to flat a network must only use sigmoid and tanh activation.");
                }

                if (lastActivation != null)
                {
                    if (layer.ActivationFunction.GetType() != lastActivation.GetType())
                    {
                        throw new NeuralNetworkError(
                                "To convert to flat, a network must use the same activation function on each layer.");
                    }
                }

                if (!layer.HasBias && (lastActivation != null))
                {
                    throw new NeuralNetworkError(
                            "To convert to flat, all non-input layers must have bias weight values.");
                }

                lastActivation = layer.ActivationFunction;
            }
        }
    }
}
