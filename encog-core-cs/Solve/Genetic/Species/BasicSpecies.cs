using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil.Randomize;
using Encog.Persist.Attributes;
using Encog.Solve.Genetic.Population;

namespace Encog.Solve.Genetic.Species
{
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The age of this species.
        /// </summary>
        [EGAttribute]
        private int age;

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
        /// The population this species belongs to.
        /// </summary>
        [EGReference]
        private IPopulation population;

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

        public IPopulation Population 
        {
            get
            {
                return this.population;
            }
            set
            {
                this.population = value;
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
        public BasicSpecies(IPopulation population,
                IGenome first, long speciesID)
        {
            this.Population = population;
            this.SpeciesID = speciesID;
            BestScore = first.Score;
            GensNoImprovement = 0;
            Age = 0;
            Leader = first;
            spawnsRequired = 0;
            members.Add(first);
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

                int maxIndexSize = (int)(this.Population
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
