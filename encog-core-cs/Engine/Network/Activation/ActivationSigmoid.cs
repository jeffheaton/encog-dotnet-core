using System;
using Encog.MathUtil;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The sigmoid activation function takes on a sigmoidal shape. Only positive
    /// numbers are generated. Do not use this activation function if negative number
    /// output is desired.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a basic sigmoid function, with a slope of 1.
        /// </summary>
        ///
        public ActivationSigmoid()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSigmoid();
        }

        /// <returns>True, sigmoid has a derivative.</returns>
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
                x[i] = 1.0d/(1.0d + BoundMath.Exp(-1*x[i]));
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            return x*(1.0d - x);
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] results = {};
                return results;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }


    }
}