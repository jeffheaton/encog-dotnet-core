using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// Linear activation function that bounds the output to [-1,+1].  This
    /// activation is typically part of a CPPN neural network, such as 
    /// HyperNEAT.
    /// 
    /// The idea for this activation function was developed by  Ken Stanley, of  
    /// the University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public class ActivationClippedLinear : IActivationFunction
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
                d[i] = (2.0 / (1.0 + Math.Exp(-4.9 * d[i]))) - 1.0;
            }
        }

        /// <inheritdoc/>
        public double DerivativeFunction(double b, double a)
        {
            return 1;
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

        public bool HasDerivative { get { return true; } }
    }
}
