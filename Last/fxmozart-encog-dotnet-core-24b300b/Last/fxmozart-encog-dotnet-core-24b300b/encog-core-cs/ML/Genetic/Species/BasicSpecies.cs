//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
        private readonly IList<IGenome> _members;

        /// <summary>
        /// The age of this species.
        /// </summary>
        ///
        private int _age;

        /// <summary>
        /// The best score.
        /// </summary>
        ///
        private double _bestScore;

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        ///
        private int _gensNoImprovement;

        /// <summary>
        /// The leader.
        /// </summary>
        ///
        private IGenome _leader;

        /// <summary>
        /// The id of the leader.
        /// </summary>
        [NonSerialized]
        private long _leaderID;

        /// <summary>
        /// The owner class.
        /// </summary>
        ///
        private IPopulation _population;

        /// <summary>
        /// The number of spawns required.
        /// </summary>
        ///
        private double _spawnsRequired;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long _speciesID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BasicSpecies()
        {
            _members = new List<IGenome>();
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
            _members = new List<IGenome>();
            _population = thePopulation;
            _speciesID = theSpeciesID;
            _bestScore = theFirst.Score;
            _gensNoImprovement = 0;
            _age = 0;
            _leader = theFirst;
            _spawnsRequired = 0;
            _members.Add(theFirst);
        }

        /// <value>the population to set</value>
        public IPopulation Population
        {
            get { return _population; }
            set { _population = value; }
        }

        /// <summary>
        /// Set the leader id. This value is not persisted, it is used only for
        /// loading.
        /// </summary>
        ///
        /// <value>the leaderID to set</value>
        public long TempLeaderID
        {
            get { return _leaderID; }
            set { _leaderID = value; }
        }

        #region ISpecies Members

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        ///
        public void CalculateSpawnAmount()
        {
            _spawnsRequired = 0;

            foreach (IGenome genome  in  _members)
            {
                _spawnsRequired += genome.AmountToSpawn;
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
            if (_members.Count == 1)
            {
                baby = _members[0];
            }
            else
            {
                // If there are many, then choose the population based on survival
                // rate
                // and select a random genome.
                int maxIndexSize = (int) (_population.SurvivalRate*_members.Count) + 1;
                var theOne = (int) RangeRandomizer.Randomize(0, maxIndexSize);
                baby = _members[theOne];
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
            get { return _age; }
            set { _age = value; }
        }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        public double BestScore
        {
            get { return _bestScore; }
            set { _bestScore = value; }
        }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value>The number of generations.</value>
        public int GensNoImprovement
        {
            get { return _gensNoImprovement; }
            set { _gensNoImprovement = value; }
        }


        /// <summary>
        /// Set the leader.
        /// </summary>
        ///
        /// <value>The new leader.</value>
        public IGenome Leader
        {
            get { return _leader; }
            set { _leader = value; }
        }


        /// <value>The members of this species.</value>
        public IList<IGenome> Members
        {
            get { return _members; }
        }


        /// <value>The number to spawn.</value>
        public double NumToSpawn
        {
            get { return _spawnsRequired; }
        }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        public double SpawnsRequired
        {
            get { return _spawnsRequired; }
            set { _spawnsRequired = value; }
        }


        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        ///
        public void Purge()
        {
            _members.Clear();
            _age++;
            _gensNoImprovement++;
            _spawnsRequired = 0;
        }

        /// <summary>
        /// Set the species id.
        /// </summary>
        public long SpeciesID
        {
            get { return _speciesID; }
            set { _speciesID = value; }
        }

        #endregion
    }
}
