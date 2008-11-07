using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;

namespace Encog.Neural.Activation
{
    public interface IActivationFunction : IEncogPersistedObject
    {
        /// <summary>
        /// A activation function for a neural network. 
        /// </summary>
        /// <param name="d">The input to the function.</param>
        /// <returns>The output from the function.</returns>
        double ActivationFunction(double d);

        /// <summary>
        /// Performs the derivative of the activation function function on the input.
        /// </summary>
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        double DerivativeFunction(double d);
    }
}
