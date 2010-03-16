using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil.Randomize;

namespace Encog.Solve.Genetic.Species
{
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The age of this species.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The best score for this species.
        /// </summary>
        public double BestScore { get; set; }

        /// <summary>
        /// How many generations with no improvement.
        /// </summary>
        public int GensNoImprovement { get; set; }

        /// <summary>
        /// Get the leader for this species. The leader is the genome with
        /// the best score.
        /// </summary>
        public IGenome Leader { get; set; }

        /// <summary>
        /// The number of genomes this species will try to spawn into the
        /// next generation.
        /// </summary>
        public double NumToSpawn { get; set; }

        /// <summary>
        /// The number of spawns this species requires.
        /// </summary>
        public double SpawnsRequired { get; set; }

        /// <summary>
        /// The species ID.
        /// </summary>
        public long SpeciesID { get; set; }

        public GeneticAlgorithm Owner { get; set; }



        /**
         * The list of genomes.
         */
        private IList<IGenome> members = new List<IGenome>();

        /**
         * Construct a species.
         * @param training
         * @param first
         * @param speciesID
         */
        public BasicSpecies(GeneticAlgorithm training,
                IGenome first, long speciesID)
        {
            this.Owner = training;
            this.SpeciesID = speciesID;
            BestScore = first.Score;
            GensNoImprovement = 0;
            Age = 0;
            Leader = first;
            SpawnsRequired = 0;
            members.Add(first);
        }

        /**
         * Add a genome.
         * @param genome The genome to add.
         */
        public void AddMember(IGenome genome)
        {

            if (Owner.Comparator.IsBetterThan(genome.Score,
                    BestScore))
            {
                BestScore = genome.Score;
                GensNoImprovement = 0;
                Leader = genome;
            }

            members.Add(genome);

        }

        /// <summary>
        /// Adjust the score.  This is to give bonus or penalty.
        /// </summary>
        public void AdjustScore()
        {

            foreach (IGenome member in members)
            {
                double score = member.Score;

                if (Age < Owner.Population.YoungBonusAgeThreshold)
                {
                    score = Owner.Comparator.ApplyBonus(score,
                            Owner.Population.YoungScoreBonus);
                }

                if (Age > Owner.Population.OldAgeThreshold)
                {
                    score = Owner.Comparator.ApplyPenalty(score,
                            Owner.Population.OldAgePenalty);
                }

                double adjustedScore = score / members.Count;

                member.AdjustedScore = adjustedScore;

            }
        }

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        public void CalculateSpawnAmount()
        {
            this.SpawnsRequired = 0;
            foreach (IGenome genome in members)
            {
                SpawnsRequired += genome.AmountToSpawn;
            }

        }

        
        /// <summary>
        /// Choose a parent to mate.
        /// </summary>
        /// <returns>The parent.</returns>
        public IGenome ChooseParent()
        {
            IGenome baby;

            if (members.Count == 1)
            {
                baby = members[0];
            }

            else
            {
                int maxIndexSize = (int)(Owner.Population
                        .SurvivalRate * members.Count) + 1;
                int theOne = (int)RangeRandomizer.Randomize(0, maxIndexSize);
                baby = members[theOne];
            }

            return baby;
        }


        /// <summary>
        /// The members of this species.
        /// </summary>
        public IList<IGenome> Members
        {
            get
            {
                return members;
            }
        }



        /// <summary>
        /// Purge all members.
        /// </summary>
        public void Purge()
        {
            members.Clear();

            Age++;

            GensNoImprovement++;

            SpawnsRequired = 0;

        }

    }
}
