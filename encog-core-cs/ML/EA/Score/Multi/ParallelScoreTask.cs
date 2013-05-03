using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Exceptions;
using Encog.ML.EA.Train;

namespace Encog.ML.EA.Score.Multi
{
    public class ParallelScoreTask
    {
        /// <summary>
        /// The genome to calculate the score for.
        /// </summary>
        private IGenome genome;

        /// <summary>
        /// The score function.
        /// </summary>
        private ICalculateScore scoreFunction;

        /// <summary>
        /// The score adjusters.
        /// </summary>
        private IList<IAdjustScore> adjusters;

        /// <summary>
        /// The owners.
        /// </summary>
        private ParallelScore owner;

        /// <summary>
        /// Construct the parallel task. 
        /// </summary>
        /// <param name="genome">The genome.</param>
        /// <param name="theOwner">The owner.</param>
        public ParallelScoreTask(IGenome genome, ParallelScore theOwner)
        {
            this.owner = theOwner;
            this.genome = genome;
            this.scoreFunction = theOwner.ScoreFunction;
            this.adjusters = theOwner.Adjusters;
        }

        /// <summary>
        /// Perform the task.
        /// </summary>
        public void PerformTask()
        {
            IMLMethod phenotype = this.owner.CODEC.Decode(this.genome);
            if (phenotype != null)
            {
                double score;
                try
                {
                    score = this.scoreFunction.CalculateScore(phenotype);
                }
                catch (EARuntimeError e)
                {
                    score = Double.NaN;
                }
                genome.Score = score;
                genome.AdjustedScore = score;
                BasicEA.CalculateScoreAdjustment(genome, adjusters);
            }
        }
    }
}
