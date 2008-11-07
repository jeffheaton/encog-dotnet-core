using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Activation
{
    class ActivationSigmoid : IActivationFunction
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
        /// <param name="d"></param>
        /// <returns>The output from the function.</returns>
        public double ActivationFunction(double d)
        {
            return 1.0 / (1 + Math.Exp(-1.0 * d));
        }

        /// <summary>
        /// Some training methods require the derivative. 
        /// </summary>
        /// <param name="d">The input to the function.</param>
        /// <returns>The output.</returns>
        public double DerivativeFunction(double d)
        {
            return d * (1.0 - d);
        }
    }
}
