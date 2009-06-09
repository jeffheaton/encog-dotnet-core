using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// Holds basic functionality that all activation functions will likely have use
    /// of. Specifically it implements a name and description for the
    /// EncogPersistedObject class.
    /// </summary>
    public abstract class BasicActivationFunction : IActivationFunction
    {
        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual Object Clone()
        {
            return null;
        }

        /// <summary>
        /// Create a persistor.  Not implemented at this level.
        /// </summary>
        /// <returns>The persistor.</returns>
        public virtual IPersistor CreatePersistor()
        {
            return null;
        }

        /// <summary>
        /// Always returns null, descriptions and names are not used
        /// for activation functions.
        /// </summary>
        public String Description
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Always returns null, descriptions and names are not used
        /// for activation functions.
        /// </summary>
        public String Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Return true if this function has a derivative.
        /// </summary>
        public abstract bool HasDerivative
        {
            get;
        }

        /// <summary>
        /// Implements the activation function derivative.  The array is modified 
        /// according derivative of the activation function being used.  See the 
        /// class description for more specific information on this type of 
        /// activation function. Propagation training requires the derivative. 
        /// Some activation functions do not support a derivative and will throw
        /// an error.
        /// </summary>
        /// <param name="d"></param>
        public abstract void DerivativeFunction(double[] d);

        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public abstract void ActivationFunction(double[] d);
    }
}
