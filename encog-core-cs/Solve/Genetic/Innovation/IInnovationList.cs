using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Innovation
{
    /// <summary>
    /// A list of innovations.
    /// </summary>
    public interface IInnovationList
    {
        /// <summary>
        /// The innovations.
        /// </summary>
        IList<IInnovation> Innovations { get; }
    }
}
