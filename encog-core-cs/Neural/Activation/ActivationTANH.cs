using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.MathUtil;
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// The hyperbolic tangent activation function takes the curved shape of the
    /// hyperbolic tangent. This activation function produces both positive and
    /// negative output. Use this activation function if both negative and positive
    /// output is desired.
    /// </summary>
    public class ActivationTANH : BasicActivationFunction
    {
        /// <summary>
        /// Internal activation function that performs the TANH.
        /// </summary>
        /// <param name="d">The input value.</param>
        /// <returns>The output value.</returns>
        private double ActivationFunction(double d)
        {
            return (BoundMath.Exp(d * 2.0) - 1.0) / (BoundMath.Exp(d * 2.0) + 1.0);
        }

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
                d[i] = ActivationFunction(d[i]);
            }

        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object cloned.</returns>
        public override Object Clone()
        {
            return new ActivationTANH();
        }

        /// <summary>
        /// Create a Persistor for this activation function.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new ActivationTANHPersistor();
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

            for (int i = 0; i < d.Length; i++)
            {
                d[i] = 1.0 - BoundMath.Pow(ActivationFunction(d[i]), 2.0);
            }
        }

        /// <summary>
        /// Return true, TANH has a derivative.
        /// </summary>
        public override bool HasDerivative
        {
            get
            {
                return true;
            }
        }

    }

}
