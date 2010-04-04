using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil.Randomize;
using Encog.Persist.Attributes;

namespace Encog.Solve.Genetic.Species
{
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The age of this species.
        /// </summary>
        [EGAttribute]
        private int age;

        [EGIgnore]
        private GeneticAlgorithm owner;

        /// <summary>
        /// The best score for this species.
        /// </summary>
        [EGAttribute]
        private double bestScore;

        /// <summary>
        /// How many generations with no improvement.
        /// </summary>
        [EGAttribute]
        private int gensNoImprovement;

        /// <summary>
        /// Get the leader for this species. The leader is the genome with
        /// the best score.
        /// </summary>
        [EGReference]
        private IGenome leader;

        /// <summary>
        /// The number of spawns this species requires.
        /// </summary>
        [EGAttribute]
        private double spawnsRequired;

        /// <summary>
        /// The species ID.
        /// </summary>
        [EGAttribute]
        private long speciesID;

        /// <summary>
        /// The age of this species.
        /// </summary>
        public int Age 
        {
            get
            {
                return this.age;
            }
            set
            {
                this.age = value;
            }
        }

        /// <summary>
        /// The best score for this species.
        /// </summary>
        public double BestScore 
        {
            get
            {
                return this.bestScore;
            }
            set
            {
                this.bestScore = value;
            }
        }

        /// <summary>
        /// How many generations with no improvement.
        /// </summary>
        public int GensNoImprovement 
        {
            get
            {
                return this.gensNoImprovement;
            }
            set
            {
                this.gensNoImprovement = value;
            }
        }

        /// <summary>
        /// Get the leader for this species. The leader is the genome with
        /// the best score.
        /// </summary>
        public IGenome Leader 
        {
            get
            {
                return this.leader;
            }
            set
            {
                this.leader = value;
            }
        }

        /// <summary>
        /// The number of genomes this species will try to spawn into the
        /// next generation.
        /// </summary>
        public double NumToSpawn 
        {
            get
            {
                return this.spawnsRequired;
            }
            set
            {
                this.spawnsRequired = value;
            }
        }

        /// <summary>
        /// The species ID.
        /// </summary>
        public long SpeciesID 
        {
            get
            {
                return this.speciesID;
            }
            set
            {
                this.speciesID = value;
            }

        }

        public GeneticAlgorithm Owner 
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }




        /**
         * The list of genomes.
         */
        private IList<IGenome> members = new List<IGenome>();

        
        /// <summary>
        /// Construct a species. 
        /// </summary>
        /// <param name="training">The training data to use.</param>
        /// <param name="first">The first genome.</param>
        /// <param name="speciesID">The species id.</param>
        public BasicSpecies(GeneticAlgorithm training,
                IGenome first, long speciesID)
        {
            this.Owner = training;
            this.SpeciesID = speciesID;
            BestScore = first.Score;
            GensNoImprovement = 0;
            Age = 0;
            Leader = first;
            spawnsRequired = 0;
            members.Add(first);
        }

        
        /// <summary>
        /// Add a genome. 
        /// </summary>
        /// <param name="genome">The genome to add.</param>
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
        /// The adjustment goes into the adjusted score.
        /// </summary>
        public void AdjustScore()
        {

            foreach (IGenome member in members)
            {
                double score = member.Score;

                // apply a youth bonus
                if (Age < Owner.Population.YoungBonusAgeThreshold)
                {
                    score = Owner.Comparator.ApplyBonus(score,
                            Owner.Population.YoungScoreBonus);
                }
                // apply an old age penalty
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
            this.spawnsRequired = 0;
            foreach (IGenome genome in members)
            {
                spawnsRequired += genome.AmountToSpawn;
            }

        }

        
        /// <summary>
        /// Choose a parent to mate. Choose from the population,
	    /// determined by the survival rate.  From this pool, a random
	    /// parent is chosen.
        /// </summary>
        /// <returns>The parent.</returns>
        public IGenome ChooseParent()
        {
            IGenome baby;

            // If there is a single member, then choose that one.
            if (members.Count == 1)
            {
                baby = members[0];
            }

            else
            {
                // If there are many, then choose the population based on survival rate
                // and select a random genome.

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
        /// Purge all members, increase age by one and count the number of generations
	    /// with no improvement.
        /// </summary>
        public void Purge()
        {
            members.Clear();

            Age++;

            GensNoImprovement++;

            spawnsRequired = 0;

        }

    }
}
