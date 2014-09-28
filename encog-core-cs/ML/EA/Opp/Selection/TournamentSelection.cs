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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Species;
using Encog.ML.EA.Train;

namespace Encog.ML.EA.Opp.Selection
{
    /// <summary>
    ///     Tournament select can be used to select a fit (or unfit) genome from a
    ///     species. The selection is run a set number of rounds. Each round two random
    ///     participants are chosen. The more fit participant continues to the next
    ///     round.
    ///     http://en.wikipedia.org/wiki/Tournament_selection
    /// </summary>
    [Serializable]
    public class TournamentSelection : ISelectionOperator
    {
        /// <summary>
        ///     Construct a tournament selection.
        /// </summary>
        /// <param name="theTrainer">The trainer.</param>
        /// <param name="theRounds">The number of rounds to use.</param>
        public TournamentSelection(IEvolutionaryAlgorithm theTrainer,
                                   int theRounds)
        {
            Trainer = theTrainer;
            Rounds = theRounds;
        }

        /// <summary>
        ///     The number of rounds.
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        ///     The trainer being used.
        /// </summary>
        public IEvolutionaryAlgorithm Trainer { get; set; }

        /// <inheritdoc />
        public int PerformAntiSelection(Random rnd, ISpecies species)
        {
            int worstIndex = rnd.Next(species.Members.Count);
            IGenome worst = species.Members[worstIndex];
            BasicEA.CalculateScoreAdjustment(worst,
                                             Trainer.ScoreAdjusters);

            for (int i = 0; i < Rounds; i++)
            {
                int competitorIndex = rnd.Next(species.Members.Count - 1);
                IGenome competitor = species.Members[competitorIndex];

                // force an invalid genome to lose
                if (Double.IsInfinity(competitor.AdjustedScore)
                    || Double.IsNaN(competitor.AdjustedScore))
                {
                    return competitorIndex;
                }

                BasicEA.CalculateScoreAdjustment(competitor,
                                                 Trainer.ScoreAdjusters);
                if (!Trainer.SelectionComparer.IsBetterThan(competitor,
                                                            worst))
                {
                    worst = competitor;
                    worstIndex = competitorIndex;
                }
            }
            return worstIndex;
        }

        /// <inheritdoc />
        public int PerformSelection(Random rnd, ISpecies species)
        {
            int bestIndex = rnd.Next(species.Members.Count);
            IGenome best = species.Members[bestIndex];
            BasicEA.CalculateScoreAdjustment(best, Trainer.ScoreAdjusters);

            for (int i = 0; i < Rounds; i++)
            {
                int competitorIndex = rnd.Next(species.Members.Count - 1);
                IGenome competitor = species.Members[competitorIndex];

                // only evaluate valid genomes
                if (!Double.IsInfinity(competitor.AdjustedScore)
                    && !Double.IsNaN(competitor.AdjustedScore))
                {
                    BasicEA.CalculateScoreAdjustment(competitor,
                                                     Trainer.ScoreAdjusters);
                    if (Trainer.SelectionComparer.IsBetterThan(
                        competitor, best))
                    {
                        best = competitor;
                        bestIndex = competitorIndex;
                    }
                }
            }
            return bestIndex;
        }
    }
}
