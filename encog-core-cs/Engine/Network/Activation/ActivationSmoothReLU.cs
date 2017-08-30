using System;
using Encog.MathUtil;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// Smooth ReLU.
    /// https://en.wikipedia.org/wiki/Rectifier_(neural_networks)
    /// </summary>
    [Serializable]
    public class ActivationSmoothReLU : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly double[] _paras;

        /// <summary>
        /// Construct a basic Rectifier, slope of 1.
        /// </summary>
        public ActivationSmoothReLU()
        {
            _paras = new double[0];
        }

        /// <returns>Return true, Rectifier has a derivative.</returns>
        public bool HasDerivative
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationSmoothReLU();
        }

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start, int size)
        {
            for (int i = start; i < start + size; i++)
            {
                x[i] = Math.Log(1.0 + Math.Pow(Math.E, x[i])); 
            }
        }

        /// <inheritdoc />
        public double DerivativeFunction(double b, double a)
        {
            return 1 / (1 + Math.Pow(Math.E, -a));
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = { };
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
