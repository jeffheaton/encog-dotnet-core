using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural
{
    /// <summary>
    /// Indicates an error has occurred in the Neural Network classes..
    /// </summary>
    public class NeuralNetworkError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public NeuralNetworkError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public NeuralNetworkError(Exception e)
            : base(e)
        {
        }
    }
}
