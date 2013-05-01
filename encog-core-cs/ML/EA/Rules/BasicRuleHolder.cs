using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Rules
{
    /// <summary>
    /// Basic implementation of a rule holder.
    /// </summary>
    public class BasicRuleHolder : IRuleHolder
    {
        /// <summary>
        /// Rewrite rules that can simplify genomes.
        /// </summary>
        private IList<IRewriteRule> rewriteRules = new List<IRewriteRule>();

        /// <summary>
        /// Rewrite rules that can simplify genomes.
        /// </summary>
        private IList<IConstraintRule> constraintRules = new List<IConstraintRule>();

        /// <inheritdoc/>
        public void AddRewriteRule(IRewriteRule rule)
        {
            this.rewriteRules.Add(rule);
        }

        /// <inheritdoc/>
        public void Rewrite(IGenome prg)
        {
            bool done = false;

            while (!done)
            {
                done = true;

                foreach (IRewriteRule rule in this.rewriteRules)
                {
                    if (rule.Rewrite(prg))
                    {
                        done = false;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void AddConstraintRule(IConstraintRule rule)
        {
            this.constraintRules.Add(rule);

        }

        /// <inheritdoc/>
        public bool IsValid(IGenome genome)
        {
            foreach (IConstraintRule rule in this.constraintRules)
            {
                if (!rule.IsValid(genome))
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc/>
        public IList<IConstraintRule> ConstraintRules
        {
            get
            {
                return this.constraintRules;
            }
        }

        /// <inheritdoc/>
        public IList<IRewriteRule> RewriteRules
        {
            get
            {
                return this.rewriteRules;
            }
        }
    }
}
