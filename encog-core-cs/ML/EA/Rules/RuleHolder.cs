using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Rules
{
    /// <summary>
    /// Holds a set of rules for an EA.
    /// </summary>
    public interface RuleHolder
    {
        /// <summary>
        /// Add a rewrite rule. Rewrite rules can be used to simplify genomes.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        void AddRewriteRule(IRewriteRule rule);

        /// <summary>
        /// Add a rewrite rule. Rewrite rules can be used to simplify genomes.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        void AddConstraintRule(IConstraintRule rule);

        /// <summary>
        /// Rewrite the specified genome. The genome will still perform the same
        /// function, but it may be shorter.
        /// </summary>
        /// <param name="genome">The genome to rewrite.</param>
        void Rewrite(IGenome genome);

        /// <summary>
        /// Determine if the specified genome is valid according to the constraint rules. 
        /// </summary>
        /// <param name="genome">The gnome to check.</param>
        /// <returns>True, if the genome is valid.</returns>
        bool IsValid(IGenome genome);

        /// <summary>
        /// The list of constraints.
        /// </summary>
        IList<IConstraintRule> getConstraintRules { get; }

        /// <summary>
        /// The rewrite rules.
        /// </summary>
        IList<IRewriteRule> getRewriteRules { get; }
    }
}
