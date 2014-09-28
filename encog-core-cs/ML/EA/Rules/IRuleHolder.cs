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
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Rules
{
    /// <summary>
    ///     Holds a set of rules for an EA.
    /// </summary>
    public interface IRuleHolder
    {
        /// <summary>
        ///     The list of constraints.
        /// </summary>
        IList<IConstraintRule> ConstraintRules { get; }

        /// <summary>
        ///     The rewrite rules.
        /// </summary>
        IList<IRewriteRule> RewriteRules { get; }

        /// <summary>
        ///     Add a rewrite rule. Rewrite rules can be used to simplify genomes.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        void AddRewriteRule(IRewriteRule rule);

        /// <summary>
        ///     Add a constraint rule.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        void AddConstraintRule(IConstraintRule rule);

        /// <summary>
        ///     Rewrite the specified genome. The genome will still perform the same
        ///     function, but it may be shorter.
        /// </summary>
        /// <param name="genome">The genome to rewrite.</param>
        void Rewrite(IGenome genome);

        /// <summary>
        ///     Determine if the specified genome is valid according to the constraint rules.
        /// </summary>
        /// <param name="genome">The gnome to check.</param>
        /// <returns>True, if the genome is valid.</returns>
        bool IsValid(IGenome genome);
    }
}
