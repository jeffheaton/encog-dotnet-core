using System;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The hyperbolic tangent activation function takes the curved shape of the
    /// hyperbolic tangent. This activation function produces both positive and
    /// negative output. Use this activation function if both negative and positive
    /// output is desired.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationTANH : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a basic HTAN activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationTANH()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationTANH();
        }


        /// <returns>Return true, TANH has a derivative.</returns>
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
                x[i] = Math.Tanh(x[i]);
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            return (1.0d - x*x);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {};
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }
}