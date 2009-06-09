using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// BiPolar activation function. This will scale the neural data into the bipolar
 /// range. Greater than zero becomes 1, less than or equal to zero becomes -1.
    /// </summary>
    public class ActivationBiPolar : BasicActivationFunction
    {
        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void ActivationFunction(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] > 0)
                {
                    d[i] = 1;
                }
                else
                {
                    d[i] = -1;
                }
            }

        }

        /// <summary>
        /// The object cloned.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override Object Clone()
        {
            return new ActivationBiPolar();
        }

        /// <summary>
        /// Create a Persistor for this activation function.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new ActivationBiPolarPersistor();
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
                    "Can't use the bipolar activation function "
                            + "where a derivative is required.");

        }

        /// <summary>
        /// Return false, bipolar has no derivative.
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
