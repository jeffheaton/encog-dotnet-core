using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Innovation
{
    /// <summary>
    /// Provides basic functionality for an innovation.
    /// </summary>
    public class BasicInnovation: IInnovation
    {
        /// <summary>
        /// The innovation id.
        /// </summary>
        public long InnovationID { get; set; }
    }
}
