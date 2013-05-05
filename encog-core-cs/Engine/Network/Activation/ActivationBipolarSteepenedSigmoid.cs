using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// The bipolar sigmoid activation function is like the regular sigmoid activation function,
    /// except Bipolar sigmoid activation function. TheOutput range is -1 to 1 instead of the more normal 0 to 1.
    /// 
    /// This activation is typically part of a CPPN neural network, such as 
    /// HyperNEAT.
    /// 
    /// It was developed by  Ken Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class ActivationBipolarSteepenedSigmoid : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        private double[] _params;

        /// <inheritdoc/>
        public void ActivationFunction(double[] d, int start, int size)
        {
            for (int i = start; i < start + size; i++)
            {
                if (d[i] < -1.0)
                {
                    d[i] = -1.0;
                }
                if (d[i] > 1.0)
                {
                    d[i] = 1.0;
                }
            }
        }

        /// <inheritdoc/>
        public double DerivativeFunction(double b, double a)
        {
            return 1;
        }

        /// <inheritdoc/>
        public bool HasDerivative
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public Object Clone()
        {
            return new ActivationBipolarSteepenedSigmoid();
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

        /// <inheritdoc/>
        public string FactoryCode
        {
            get
            {
                return null;
            }
        }
    }
}
