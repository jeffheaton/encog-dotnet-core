using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Rules
{
    /// <summary>
    /// Defines a rewrite rule. Rewrite rules are used to rewrite a genome into a
    /// more simple, yet equivalent, form.
    /// </summary>
    public interface IRewriteRule
    {
        /// <summary>
        /// Rewrite the specified genome.
        /// </summary>
        /// <param name="genome">The genome to rewrite.</param>
        /// <returns>True, if the genome was rewritten.</returns>
        bool Rewrite(IGenome genome);
    }
}
