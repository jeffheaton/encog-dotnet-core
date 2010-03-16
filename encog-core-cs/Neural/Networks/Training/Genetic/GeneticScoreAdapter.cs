using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;

namespace Encog.Neural.Networks.Training.Genetic
{
    public class GeneticScoreAdapter : ICalculateGenomeScore
    {
        private ICalculateScore calculateScore;

        public GeneticScoreAdapter(ICalculateScore calculateScore)
        {
            this.calculateScore = calculateScore;
        }

        public double CalculateScore(IGenome genome)
        {
            BasicNetwork network = (BasicNetwork)genome.Organism;
            return this.calculateScore.CalculateScore(network);
        }

        public bool ShouldMinimize
        {
            get
            {
                return this.calculateScore.ShouldMinimize;
            }
        }
    }
}
