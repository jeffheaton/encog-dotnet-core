using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// The step activation function is a very simple activation function. It is the
    /// activation function that was used by the original perceptron. Using the
    /// default parameters it will return 1 if the input is 0 or greater. Otherwise
    /// it will return 1.
    /// 
    /// The center, low and high properties allow you to define how this activation
    /// function works. If the input is equal to center or higher the high property
    /// value will be returned, otherwise the low property will be returned. This
    /// activation function does not have a derivative, and can not be used with
    /// propagation training, or any other training that requires a derivative.
    /// </summary>
    public class ActivationStep : BasicActivationFunction
    {
        /// <summary>
        /// The center.
        /// </summary>
        private double center = 0.0;

        /// <summary>
        /// The low value that is returned.
        /// </summary>
        private double low = 0.0;

        /// <summary>
        /// The high value that is returned.
        /// </summary>
        private double high = 1.0;

        /// <summary>
        /// The center value.
        /// </summary>
        public double Center
        {
            get
            {
                return center;
            }
            set
            {
                this.center = value;
            }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get
            {
                return low;
            }
            set
            {
                this.low = value;
            }
        }
        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get
            {
                return high;
            }
            set
            {
                this.high = value;
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override Object Clone()
        {
            ActivationStep result = new ActivationStep();
            result.Center = this.Center;
            result.High = this.High;
            result.Low = this.Low;
            return result;
        }

       
        /// <summary>
        /// The activation function. 
        /// </summary>
        /// <param name="d">The array to calculate the activation function for.</param>
        public override void ActivationFunction(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] >= this.center)
                    d[i] = this.high;
                else
                    d[i] = this.low;
            }

        }

        /// <summary>
        /// Throws an error, there is no derivative. 
        /// </summary>
        /// <param name="d">The array to get the derivative.</param>
        public override void DerivativeFunction(double[] d)
        {
            throw new NeuralNetworkError("Can't use the step activation function "
                    + "where a derivative is required.");

        }

        /// <summary>
        /// Returns false, this activation function has no derivative.
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
