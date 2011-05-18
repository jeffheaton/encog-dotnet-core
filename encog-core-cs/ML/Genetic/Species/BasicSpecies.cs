using System;
using System.Collections.Generic;
using Encog.MathUtil.Randomize;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Population;

namespace Encog.ML.Genetic.Species
{
    /// <summary>
    /// Provides basic functionality for a species.
    /// </summary>
    ///
    [Serializable]
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The list of genomes.
        /// </summary>
        ///
        private readonly IList<IGenome> members;

        /// <summary>
        /// The age of this species.
        /// </summary>
        ///
        private int age;

        /// <summary>
        /// The best score.
        /// </summary>
        ///
        private double bestScore;

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        ///
        private int gensNoImprovement;

        /// <summary>
        /// The leader.
        /// </summary>
        ///
        private IGenome leader;

        /// <summary>
        /// The id of the leader.
        /// </summary>
        [NonSerialized]
        private long leaderID;

        /// <summary>
        /// The owner class.
        /// </summary>
        ///
        private IPopulation population;

        /// <summary>
        /// The number of spawns required.
        /// </summary>
        ///
        private double spawnsRequired;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long speciesID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BasicSpecies()
        {
            members = new List<IGenome>();
        }

        /// <summary>
        /// Construct a species.
        /// </summary>
        ///
        /// <param name="thePopulation">The population the species belongs to.</param>
        /// <param name="theFirst">The first genome in the species.</param>
        /// <param name="theSpeciesID">The species id.</param>
        public BasicSpecies(IPopulation thePopulation, IGenome theFirst,
                            long theSpeciesID)
        {
            members = new List<IGenome>();
            population = thePopulation;
            speciesID = theSpeciesID;
            bestScore = theFirst.Score;
            gensNoImprovement = 0;
            age = 0;
            leader = theFirst;
            spawnsRequired = 0;
            members.Add(theFirst);
        }

        /// <value>the population to set</value>
        public IPopulation Population
        {
            /// <returns>The population that this species belongs to.</returns>
            get { return population; }
            /// <param name="thePopulation">the population to set</param>
            set { population = value; }
        }

        /// <summary>
        /// Set the leader id. This value is not persisted, it is used only for
        /// loading.
        /// </summary>
        ///
        /// <value>the leaderID to set</value>
        public long TempLeaderID
        {
            /// <returns>the leaderID</returns>
            get { return leaderID; }
            /// <summary>
            /// Set the leader id. This value is not persisted, it is used only for
            /// loading.
            /// </summary>
            ///
            /// <param name="theLeaderID">the leaderID to set</param>
            set { leaderID = value; }
        }

        #region ISpecies Members

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        ///
        public void CalculateSpawnAmount()
        {
            spawnsRequired = 0;

            foreach (IGenome genome  in  members)
            {
                spawnsRequired += genome.AmountToSpawn;
            }
        }

        /// <summary>
        /// Choose a parent to mate. Choose from the population, determined by the
        /// survival rate. From this pool, a random parent is chosen.
        /// </summary>
        ///
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
                // If there are many, then choose the population based on survival
                // rate
                // and select a random genome.
                int maxIndexSize = (int) (population.SurvivalRate*members.Count) + 1;
                var theOne = (int) RangeRandomizer.Randomize(0, maxIndexSize);
                baby = members[theOne];
            }

            return baby;
        }

        /// <summary>
        /// Set the age of this species.
        /// </summary>
        ///
        /// <value>The age of this species.</value>
        public int Age
        {
            /// <returns>The age of this species.</returns>
            get { return age; }
            /// <summary>
            /// Set the age of this species.
            /// </summary>
            ///
            /// <param name="theAge">The age of this species.</param>
            set { age = value; }
        }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        public double BestScore
        {
            /// <returns>The best score for this species.</returns>
            get { return bestScore; }
            /// <summary>
            /// Set the best score.
            /// </summary>
            ///
            /// <param name="theBestScore">The best score.</param>
            set { bestScore = value; }
        }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value>The number of generations.</value>
        public int GensNoImprovement
        {
            /// <returns>The number of generations with no improvement.</returns>
            get { return gensNoImprovement; }
            /// <summary>
            /// Set the number of generations with no improvement.
            /// </summary>
            ///
            /// <param name="theGensNoImprovement">The number of generations.</param>
            set { gensNoImprovement = value; }
        }


        /// <summary>
        /// Set the leader.
        /// </summary>
        ///
        /// <value>The new leader.</value>
        public IGenome Leader
        {
            /// <returns>THe leader of this species.</returns>
            get { return leader; }
            /// <summary>
            /// Set the leader.
            /// </summary>
            ///
            /// <param name="theLeader">The new leader.</param>
            set { leader = value; }
        }


        /// <value>The members of this species.</value>
        public IList<IGenome> Members
        {
            /// <returns>The members of this species.</returns>
            get { return members; }
        }


        /// <value>The number to spawn.</value>
        public double NumToSpawn
        {
            /// <returns>The number to spawn.</returns>
            get { return spawnsRequired; }
        }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        ///
        /// <value>The number of spawns required.</value>
        public double SpawnsRequired
        {
            /// <returns>The spawns required.</returns>
            get { return spawnsRequired; }
            /// <summary>
            /// Set the number of spawns required.
            /// </summary>
            ///
            /// <param name="theSpawnsRequired">The number of spawns required.</param>
            set { spawnsRequired = value; }
        }


        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        ///
        public void Purge()
        {
            members.Clear();
            age++;
            gensNoImprovement++;
            spawnsRequired = 0;
        }

        /// <summary>
        /// Set the species id.
        /// </summary>
        ///
        /// <value>The new species id.</value>
        public long SpeciesID
        {
            get { return speciesID; }
            /// <summary>
            /// Set the species id.
            /// </summary>
            ///
            /// <param name="i">The new species id.</param>
            set { speciesID = value; }
        }

        #endregion
    }
}