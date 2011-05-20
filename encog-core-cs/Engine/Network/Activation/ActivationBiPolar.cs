using System;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// BiPolar activation function. This will scale the neural data into the bipolar
    /// range. Greater than zero becomes 1, less than or equal to zero becomes -1.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationBiPolar : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the bipolar activation function.
        /// </summary>
        ///
        public ActivationBiPolar()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Implements the activation function derivative. The array is modified
        /// according derivative of the activation function being used. See the class
        /// description for more specific information on this type of activation
        /// function. Propagation training requires the derivative. Some activation
        /// functions do not support a derivative and will throw an error.
        /// </summary>
        ///
        /// <param name="d">The input array to the activation function.</param>
        /// <returns>The derivative.</returns>
        public virtual double DerivativeFunction(double d)
        {
            return 1;
        }


        /// <returns>Return true, bipolar has a 1 for derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (x[i] > 0)
                {
                    x[i] = 1;
                }
                else
                {
                    x[i] = -1;
                }
            }
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {"slope"};
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }        

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationBiPolar();
        }
    }
}