
using System;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The Linear layer is really not an activation function at all. The input is
    /// simply passed on, unmodified, to the output. This activation function is
    /// primarily theoretical and of little actual use. Usually an activation
    /// function that scales between 0 and 1 or -1 and 1 should be used.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationLinear : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a linear activation function, with a slope of 1.
        /// </summary>
        ///
        public ActivationLinear()
        {
            _paras = new double[0];
        }


        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationLinear();
        }


        /// <returns>Return true, linear has a 1 derivative.</returns>
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
                x[i] = x[i];
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double d)
        {
            return 1;
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