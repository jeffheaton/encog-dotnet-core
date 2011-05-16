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
        private readonly double[] paras;

        /// <summary>
        /// Construct a basic sigmoid function, with a slope of 1.
        /// </summary>
        ///
        public ActivationSigmoid()
        {
            paras = new double[0];
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
            get { return paras; }
        }


        /// <inheritdoc />
        public virtual void SetParam(int index, double value_ren)
        {
            paras[index] = value_ren;
        }

        /// <inheritdoc />
        public virtual String GetOpenCLExpression(bool derivative)
        {
            if (derivative)
            {
                return "(x * (1.0f - x))";
            }
            else
            {
                return "(1.0f / (1.0f + exp(-1 * x)))";
            }
        }
    }
}