using System;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Thrown when a training error occurs.
    /// </summary>
    ///
    [Serializable]
    public class TrainingError : NeuralNetworkError
    {
        /// <summary>
        /// The serial id. 
        /// </summary>
        ///
        private const long serialVersionUID = 9138367057650889570L;

        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public TrainingError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public TrainingError(Exception t) : base(t)
        {
        }
    }
}