
using System;
using Encog.MathUtil;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// An activation function based on the logarithm function.
    /// This type of activation function can be useful to prevent saturation. A
    /// hidden node of a neural network is said to be saturated on a given set of
    /// inputs when its output is approximately 1 or -1 "most of the time". If this
    /// phenomena occurs during training then the learning of the network can be
    /// slowed significantly since the error surface is very at in this instance.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ActivationLOG : IActivationFunction
    {
        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct the activation function.
        /// </summary>
        ///
        public ActivationLOG()
        {
            _paras = new double[0];
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationLOG();
        }


        /// <returns>Return true, log has a derivative.</returns>
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
                if (x[i] >= 0)
                {
                    x[i] = BoundMath.Log(1 + x[i]);
                }
                else
                {
                    x[i] = -BoundMath.Log(1 - x[i]);
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double x)
        {
            if (x >= 0)
            {
                return 1/(1 + x);
            }
            return 1/(1 - x);
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