using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// ReLU activation function. This function has a high and low threshold. If
    /// the high threshold is exceeded a fixed value is returned.Likewise, if the
    /// low value is exceeded another fixed value is returned.
    /// </summary>
    [Serializable]
    public class ActivationReLU: IActivationFunction
    {

        /// <summary>
        /// The ramp low threshold parameter.
        /// </summary>
        public const int PARAM_RELU_LOW_THRESHOLD = 0;

        /// <summary>
        /// The ramp low parameter.
        /// </summary>
        public const int PARAM_RELU_LOW = 0;

        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly double[] _params;




        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActivationReLU():
            this(0,0)
        {
        }
        
        /// <summary>
        /// Construct a ramp activation function. 
        /// </summary>
        /// <param name="thresholdLow">The low threshold value.</param>
        /// <param name="low">The low value, replaced if the low threshold is exceeded.</param>
        public ActivationReLU(double thresholdLow, double low)
        {
            _params = new double[2];
            _params[ActivationReLU.PARAM_RELU_LOW_THRESHOLD] = thresholdLow;
		    _params[ActivationReLU.PARAM_RELU_LOW] = low;
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
            return new ActivationReLU();
        }

        /// <inheritdoc />
        public void ActivationFunction(double[] x, int start, int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (x[i] <= _params[ActivationReLU.PARAM_RELU_LOW_THRESHOLD]) {
				x[i] = _params[ActivationReLU.PARAM_RELU_LOW];
			}
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
            get { return _params; }
        }

    }
}
