
using System;
using Encog.MathUtil;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The softmax activation function.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationSoftMax : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the soft-max activation function.
        /// </summary>
        ///
        public ActivationSoftMax()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSoftMax();
        }

        /// <returns>Return false, softmax has no derivative.</returns>
        public virtual bool HasDerivative()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            double sum = 0;
            for (int i = start; i < start + size; i++)
            {
                x[i] = BoundMath.Exp(x[i]);
                sum += x[i];
            }
            for (int i = start; i < start + size; i++)
            {
                x[i] = x[i]/sum;
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double d)
        {
            return 1.0d;
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