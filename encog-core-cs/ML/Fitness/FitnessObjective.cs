using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training;

namespace Encog.ML.Fitness
{
    /// <summary>
    /// A fitness objective.
    /// </summary>
    public class FitnessObjective
    {
        /// <summary>
        /// The weight.
        /// </summary>
        private double weight;

        /// <summary>
        /// The score function.
        /// </summary>
        private ICalculateScore score;

        /// <summary>
        /// Construct the fitness objective. 
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <param name="score">The score.</param>
        public FitnessObjective(double weight, ICalculateScore score)
        {
            this.weight = weight;
            this.score = score;
        }

        /// <summary>
        /// The weight.
        /// </summary>
        public double Weight
        {
            get
            {
                return weight;
            }
        }

        /// <summary>
        /// The score.
        /// </summary>
        public ICalculateScore Score
        {
            get
            {
                return score;
            }
        }
    }
}
