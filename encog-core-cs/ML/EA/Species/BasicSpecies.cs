using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Population;
using Encog.ML.EA.Genome;
using Encog.Util;

namespace Encog.ML.EA.Species
{
    /// <summary>
    /// Provides basic functionality for a species.
    /// </summary>
    [Serializable]
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The age of this species.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The best score.
        /// </summary>
        public double BestScore { get; set; }

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        public int GensNoImprovement { get; set; }

        /// <summary>
        /// The leader.
        /// </summary>
        public IGenome Leader { get; set; }

        /// <summary>
        /// The list of genomes.
        /// </summary>
        private List<IGenome> members = new List<IGenome>();

        /// <summary>
        /// The owner class.
        /// </summary>
        public IPopulation Population { get; set; }

        /// <summary>
        ///  The offspring count.
        /// </summary>
        public int OffspringCount { get; set; }

        /// <summary>
        /// The offpsring share (percent).
        /// </summary>
        public double OffspringShare { get; set; }

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        public BasicSpecies()
        {

        }

        /// <summary>
        /// Construct a species.
        /// </summary>
        /// <param name="thePopulation">The population the species belongs to.</param>
        /// <param name="theFirst">The first genome in the species.</param>
        public BasicSpecies(IPopulation thePopulation, IGenome theFirst)
        {
            Population = thePopulation;
            BestScore = theFirst.Score;
            GensNoImprovement = 0;
            Age = 0;
            Leader = theFirst;
            this.members.Add(theFirst);
        }

        /// <inheritdoc/>
        public void Add(IGenome genome)
        {
            genome.Population = this.Population;
            this.members.Add(genome);
        }

        /// <inheritdoc/>
        public double CalculateShare(bool shouldMinimize,
                double maxScore)
        {
            double total = 0;

            int count = 0;
            foreach (IGenome genome in this.members)
            {
                if (!double.IsNaN(genome.AdjustedScore)
                        && !double.IsInfinity(genome.AdjustedScore))
                {
                    double s;
                    if (shouldMinimize)
                    {
                        s = maxScore - genome.AdjustedScore;
                    }
                    else
                    {
                        s = genome.AdjustedScore;
                    }
                    total += s;
                    count++;
                }
            }

            if (count == 0)
            {
                this.OffspringShare = 0;
            }
            else
            {
                this.OffspringShare = total / count;
            }

            return this.OffspringShare;
        }

        /// <inheritdoc/>
        public List<IGenome> Members
        {
            get
            {
                return this.members;
            }
            set
            {
                this.members = value;
            }
        }

        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        public void Purge()
        {
            this.members.Clear();
            if (this.Leader != null)
            {
                this.members.Add(this.Leader);
            }
            Age++;
            GensNoImprovement++;
            OffspringCount = 0;
            OffspringShare = 0;
        }


        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[BasicSpecies: score=");
            result.Append(Format.FormatDouble(BestScore, 2));
            result.Append(", members=");
            result.Append(this.members.Count);
            result.Append(", age=");
            result.Append(Age);
            result.Append(", no_improv=");
            result.Append(GensNoImprovement);
            result.Append(", share=");
            result.Append(OffspringShare);
            result.Append(", offspring count=");
            result.Append(OffspringShare);
            result.Append("]");
            return result.ToString();
        }
    }
}
