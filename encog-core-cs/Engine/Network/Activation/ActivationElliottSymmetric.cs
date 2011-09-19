using System;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// Computationally efficient alternative to ActivationTANH.
    /// Its output is in the range [-1, 1], and it is derivable.
    /// 
    /// It will approach the -1 and 1 more slowly than Tanh so it 
    /// might be more suitable to classification tasks than predictions tasks.
    /// 
    /// Elliott, D.L. "A better activation function for artificial neural networks", 1993
    /// http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.46.7204&rep=rep1&type=pdf
    /// </summary>
    public class ActivationElliottSymmetric : IActivationFunction
    {
        /// <summary>
        /// The params.
        /// </summary>
        private readonly double[] _p;

        /// <summary>
        /// Construct a basic Elliott activation function, with a slope of 1.
        /// </summary>
        public ActivationElliottSymmetric()
        {
            _p = new double[1];
            _p[0] = 1.0;
        }

        #region IActivationFunction Members

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start,
                                       int size)
        {
            for (int i = start; i < start + size; i++)
            {
                double s = _p[0];
                x[i] = (x[i] * s) / (1 + Math.Abs(x[i] * s));
            }
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The object to be cloned.</returns>
        public object Clone()
        {
            return new ActivationElliottSymmetric();
        }

        /// <inheritdoc />
        public double DerivativeFunction(double b, double a)
        {
            double s = _p[0];
    	    double d = (1.0+Math.Abs(b*s));
    	    return  (s*1.0)/(d*d);
        }

        /// <inheritdoc />
        public String[] ParamNames
        {
            get
            {
                String[] result = { "Slope" };
                return result;
            }
        }

        /// <inheritdoc />
        public double[] Params
        {
            get { return _p; }
        }

        /// <summary>
        /// Return true, Elliott activation has a derivative.
        /// </summary>
        /// <returns>Return true, Elliott activation has a derivative.</returns>
        public bool HasDerivative()
        {
            return true;
        }

        #endregion

        /// <inheritdoc />
        public void SetParam(int index, double value)
        {
            _p[index] = value;
        }
    }
}