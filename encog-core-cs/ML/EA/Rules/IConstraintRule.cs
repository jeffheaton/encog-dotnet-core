using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Rules
{
    /// <summary>
    /// Defines a constraint.  A constraint specifies if a genome is invalid.
    /// </summary>
    public interface IConstraintRule
    {
        /// <summary>
        /// Is this genome valid?
        /// </summary>
        /// <param name="genome">The genome.</param>
        /// <returns>True if this genome is valid.</returns>
        bool IsValid(IGenome genome);
    }
}
