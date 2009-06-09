using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Util.MathUtil;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// The softmax activation function.
    /// </summary>
    public class ActivationSoftMax : BasicActivationFunction
    {
        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void ActivationFunction(double[] d)
        {

            double sum = 0;
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = BoundMath.Exp(d[i]);
                sum += d[i];
            }
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = d[i] / sum;
            }
        }

        /// <summary>
        /// Clone the specified object.
        /// </summary>
        /// <returns>The object cloned.</returns>
        public override Object Clone()
        {
            return new ActivationSoftMax();
        }

        /// <summary>
        /// Create a Persistor for this activation function.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new ActivationSoftMaxPersistor();
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
                    "Can't use the softmax activation function "
                            + "where a derivative is required.");
        }

        /// <summary>
        /// Return false, softmax has no derivative.
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
