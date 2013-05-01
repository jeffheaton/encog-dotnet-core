using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Species;

namespace Encog.ML.EA.Opp.Selection
{
    /// <summary>
    /// Tournament select can be used to select a fit (or unfit) genome from a
    /// species. The selection is run a set number of rounds. Each round two random
    /// participants are chosen. The more fit participant continues to the next
    /// round.
    /// 
    /// http://en.wikipedia.org/wiki/Tournament_selection
    /// </summary>
    [Serializable]
    public class TournamentSelection : ISelectionOperator
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        public IEvolutionaryAlgorithm Trainer { get; set; }

        /// <summary>
        /// The number of rounds.
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        /// Construct a tournament selection.
        /// </summary>
        /// <param name="theTrainer">The trainer.</param>
        /// <param name="theRounds">The number of rounds to use.</param>
        public TournamentSelection(IEvolutionaryAlgorithm theTrainer,
                int theRounds)
        {
            Trainer = theTrainer;
            Rounds = theRounds;
        }

        /// <inheritdoc/>
        public int PerformAntiSelection(Random rnd, ISpecies species)
        {
            int worstIndex = rnd.Next(species.Members.Count);
            IGenome worst = species.Members[worstIndex];
            BasicEA.CalculateScoreAdjustment(worst,
                    Trainer.ScoreAdjusters);

            for (int i = 0; i < this.Rounds; i++)
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

        /// <inheritdoc/>
        public int PerformSelection(Random rnd, ISpecies species)
        {
            int bestIndex = rnd.Next(species.Members.Count);
            IGenome best = species.Members[bestIndex];
            BasicEA.CalculateScoreAdjustment(best, Trainer.ScoreAdjusters );

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
