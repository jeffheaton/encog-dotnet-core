using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// This interface allows various activation functions to be used with the neural
    /// network. Activation functions are applied to the output from each layer of a
    /// neural network. Activation functions scale the output into the desired range.
    /// 
    /// Methods are provided both to process the activation function, as well as the
    /// derivative of the function. Some training algorithms, particularly back
    /// propagation, require that it be possible to take the derivative of the
    /// activation function.
    /// 
    /// Not all activation functions support derivatives. If you implement an
    /// activation function that is not derivable then an exception should be thrown
    /// inside of the derivativeFunction method implementation.
    /// 
    /// Non-derivable activation functions are perfectly valid, they simply cannot be
    /// used with every training algorithm.
    /// </summary>
    public interface IActivationFunction : IEncogPersistedObject
    {
        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        void ActivationFunction(double[] d);

        /// <summary>
        /// Implements the activation function derivative.  The array is modified 
        /// according derivative of the activation function being used.  See the 
        /// class description for more specific information on this type of 
        /// activation function. Propagation training requires the derivative. 
        /// Some activation functions do not support a derivative and will throw
        /// an error.
        /// </summary>
        /// <param name="d"></param>
        void DerivativeFunction(double[] d);

        /// <summary>
        /// Return true if this function has a derivative.
        /// </summary>
        bool HasDerivative
        {
            get;
        }

    }

}
