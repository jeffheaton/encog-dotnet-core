using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Activation
{
    public class ActivationLinear: IActivationFunction
    {
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private String description;
        private String name;

        /// <summary>
        /// A threshold function for a neural network.
        /// </summary>
        /// <param name="d">The input to the function.</param>
        /// <returns>The output from the function.</returns>
        public double ActivationFunction(double d)
        {
            return d;
        }

        /// <summary>
        /// Some training methods require the derivative.
        /// </summary>
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        public double DerivativeFunction(double d)
        {
            throw new NeuralNetworkError(
                    "Can't use the linear activation function "
                    + "where a derivative is required.");
        }
    }
}
