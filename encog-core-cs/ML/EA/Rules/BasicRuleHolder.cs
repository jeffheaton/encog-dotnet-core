//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
