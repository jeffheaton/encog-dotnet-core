using System;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// An error raised by the genetic algorithm.
    /// </summary>
    ///
    [Serializable]
    public class GeneticError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public GeneticError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="msg">A message.</param>
        /// <param name="t">The other exception.</param>
        public GeneticError(String msg, Exception t) : base(msg, t)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public GeneticError(Exception t) : base(t)
        {
        }
    }
}