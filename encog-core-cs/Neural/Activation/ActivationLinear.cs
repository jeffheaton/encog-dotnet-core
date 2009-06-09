using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// The Linear layer is really not an activation function at all. The input is
    /// simply passed on, unmodified, to the output. This activation function is
    /// primarily theoretical and of little actual use. Usually an activation
    /// function that scales between 0 and 1 or -1 and 1 should be used.
    /// </summary>
    public class ActivationLinear : BasicActivationFunction
    {
        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void ActivationFunction(double[] d)
        {

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object cloned.</returns>
        public override Object Clone()
        {
            return new ActivationLinear();
        }

        /// <summary>
        /// Create a Persistor for this activation function.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new ActivationLinearPersistor();
        }

        /// <summary>
        /// Implements the activation function derivative.  The array is modified 
        /// according derivative of the activation function being used.  See the 
        /// class description for more specific information on this type of 
        /// activation function. Propagation training requires the derivative. 
        /// Some activation functions do not support a derivative and will throw
        /// an error.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void DerivativeFunction(double[] d)
        {
            throw new NeuralNetworkError(
                    "Can't use the linear activation function "
                            + "where a derivative is required.");
        }

        /// <summary>
        /// Return false, linear has no derivative.
        /// </summary>
        public override bool HasDerivative
        {
            get
            {
                return false;
            }
        }
    }
}
