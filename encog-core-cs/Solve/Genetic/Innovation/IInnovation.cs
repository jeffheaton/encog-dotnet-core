using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Innovation
{
    /// <summary>
    /// Provides the interface for an innovation. An innovation is a enhancement that
    /// was tried to the genome.
    /// </summary>
    public interface IInnovation
    {
        /// <summary>
        /// The innovation id.
        /// </summary>
        long InnovationID { get; set; }
    }
}
