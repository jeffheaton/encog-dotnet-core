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
using System;
using Encog.ML.EA.Species;
using Encog.ML.EA.Train;

namespace Encog.ML.EA.Opp.Selection
{
    /// <summary>
    ///     Provides the interface to a selection operator. This allows genomes to be
    ///     selected for offspring production or elimination.
    /// </summary>
    public interface ISelectionOperator
    {
        /// <summary>
        ///     The trainer being used.
        /// </summary>
        IEvolutionaryAlgorithm Trainer { get; }

        /// <summary>
        ///     Selects an fit genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="species">The species to select the genome from.</param>
        /// <returns>The selected genome.</returns>
        int PerformSelection(Random rnd, ISpecies species);

        /// <summary>
        ///     Selects an unfit genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="species">The species to select the genome from.</param>
        /// <returns>The selected genome.</returns>
        int PerformAntiSelection(Random rnd, ISpecies species);
    }
}
