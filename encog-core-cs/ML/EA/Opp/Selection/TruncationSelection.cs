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
    /// Perform a selection by truncation.
    /// </summary>
    public class TruncationSelection : ISelectionOperator
    {
        /// <summary>
        /// The percent to select from.
        /// </summary>
        private readonly double _percent;

        /// <summary>
        /// The trainer.
        /// </summary>
        private IEvolutionaryAlgorithm _trainer;

        /// <summary>
        /// Construct the truncation selector.
        /// </summary>
        /// <param name="theTrainer">The trainer.</param>
        /// <param name="thePercent">The top percent to select from.</param>
        public TruncationSelection(IEvolutionaryAlgorithm theTrainer,
                                   double thePercent)
        {
            _trainer = theTrainer;
            _percent = thePercent;
        }

        /// <summary>
        ///     The number of rounds.
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        ///     The trainer being used.
        /// </summary>
        public IEvolutionaryAlgorithm Trainer { get; set; }

        /// <inheritdoc/>
        public int PerformSelection(Random rnd, ISpecies species)
        {
            int top = Math.Max((int) (species.Members.Count*_percent),
                               1);
            int result = rnd.Next(top);
            return result;
        }

        /// <inheritdoc/>
        public int PerformAntiSelection(Random rnd, ISpecies species)
        {
            return species.Members.Count - PerformSelection(rnd, species);
        }
    }
}
