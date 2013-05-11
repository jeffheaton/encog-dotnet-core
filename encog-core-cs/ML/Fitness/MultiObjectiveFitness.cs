using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training;

namespace Encog.ML.Fitness
{
    /// <summary>
    /// A multi-objective fitness function.
    /// </summary>
    [Serializable]
    public class MultiObjectiveFitness : ICalculateScore
    {
        /// <summary>
        /// The objectives.
        /// </summary>
        private IList<FitnessObjective> objectives = new List<FitnessObjective>();

        /// <summary>
        /// Is the goal to minimize the score?
        /// </summary>
        private bool min;

        /// <summary>
        /// Add an objective. 
        /// </summary>
        /// <param name="weight">The weight of this objective, 1.0 for full, 0.5 for half, etc.</param>
        /// <param name="fitnessFunction">The fitness function.</param>
        public void AddObjective(double weight, ICalculateScore fitnessFunction)
        {
            if (this.objectives.Count == 0)
            {
                this.min = fitnessFunction.ShouldMinimize;
            }
            else
            {
                if (fitnessFunction.ShouldMinimize != this.min)
                {
                    throw new EncogError("Multi-objective mismatch, some objectives are min and some are max.");
                }
            }
            this.objectives.Add(new FitnessObjective(weight, fitnessFunction));
        }

        /// <inheritdoc/>
        public double CalculateScore(IMLMethod method)
        {
            double result = 0;

            foreach (FitnessObjective obj in this.objectives)
            {
                result += obj.Score.CalculateScore(method) * obj.Weight;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool ShouldMinimize
        {
            get
            {
                return this.min;
            }
        }

        /// <inheritdoc/>
        public bool RequireSingleThreaded
        {
            get
            {
                foreach (FitnessObjective obj in this.objectives)
                {
                    if (obj.Score.RequireSingleThreaded)
                    {
                        return true;
                    }
                }

                return false;
            }

        }
    }
}
