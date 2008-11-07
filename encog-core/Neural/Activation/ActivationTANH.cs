using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Activation
{
    class ActivationTANH : IActivationFunction
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
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        public double ActivationFunction(double d)
        {
            double result = (Math.Exp(d * 2.0) - 1.0)
                    / (Math.Exp(d * 2.0) + 1.0);
            return result;
        }

        /// <summary>
        /// The derivative of the TANH.
        /// </summary>
        /// <param name="d">The input.</param>
        /// <returns>The output.</returns>
        public double DerivativeFunction(double d)
        {
            return 1.0 - Math.Pow(ActivationFunction(d), 2.0);
        }


    }
}
