using Encog.ML.EA.Train;
using System;
using Encog.ML.EA.Species;
namespace Encog.ML.EA.Opp.Selection
{
    public class TruncationSelection : ISelectionOperator
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        public IEvolutionaryAlgorithm Trainer { get; set; }

        /// <summary>
        /// The number of rounds.
        /// </summary>
        public int Rounds { get; set; }

        /**
         * The trainer.
         */
        private IEvolutionaryAlgorithm trainer;

        /**
         * The percent to select from.
         */
        private double percent;

        /**
         * Construct the truncation selector.
         * @param theTrainer The trainer.
         * @param thePercent The top percent to select from.
         */
        public TruncationSelection(IEvolutionaryAlgorithm theTrainer,
                double thePercent)
        {
            this.trainer = theTrainer;
            this.percent = thePercent;
        }

        /**
         * {@inheritDoc}
         */
        public int PerformSelection(Random rnd, ISpecies species)
        {
            int top = Math.Max((int)(species.Members.Count * this.percent),
                    1);
            int result = rnd.Next(top);
            return result;
        }

        /**
         * {@inheritDoc}
         */
        public int PerformAntiSelection(Random rnd, ISpecies species)
        {
            return species.Members.Count - PerformSelection(rnd, species);
        }

    }
}
