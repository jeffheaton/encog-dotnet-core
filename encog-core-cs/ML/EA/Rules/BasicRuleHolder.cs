//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using System.Collections.Generic;
using System.Linq;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Rules
{
    /// <summary>
    ///     Basic implementation of a rule holder.
    /// </summary>
    public class BasicRuleHolder : IRuleHolder
    {
        /// <summary>
        ///     Rewrite rules that can simplify genomes.
        /// </summary>
        private readonly IList<IConstraintRule> _constraintRules = new List<IConstraintRule>();

        /// <summary>
        ///     Rewrite rules that can simplify genomes.
        /// </summary>
        private readonly IList<IRewriteRule> _rewriteRules = new List<IRewriteRule>();

        /// <inheritdoc />
        public void AddRewriteRule(IRewriteRule rule)
        {
            _rewriteRules.Add(rule);
        }

        /// <inheritdoc />
        public void Rewrite(IGenome prg)
        {
            bool done = false;

            while (!done)
            {
                done = true;

                foreach (IRewriteRule rule in _rewriteRules)
                {
                    if (rule.Rewrite(prg))
                    {
                        done = false;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void AddConstraintRule(IConstraintRule rule)
        {
            _constraintRules.Add(rule);
        }

        /// <inheritdoc />
        public bool IsValid(IGenome genome)
        {
            return _constraintRules.All(rule => rule.IsValid(genome));
        }

        /// <inheritdoc />
        public IList<IConstraintRule> ConstraintRules
        {
            get { return _constraintRules; }
        }

        /// <inheritdoc />
        public IList<IRewriteRule> RewriteRules
        {
            get { return _rewriteRules; }
        }
    }
}
