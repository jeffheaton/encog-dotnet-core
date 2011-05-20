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
using System.Collections.Generic;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Innovation;
using Encog.ML.Genetic.Species;

namespace Encog.ML.Genetic.Population
{
    /// <summary>
    /// Defines a population of genomes.
    /// </summary>
    ///
    public interface IPopulation
    {
        /// <value>The best genome in the population.</value>
        IGenome Best { get; }


        /// <value>The genomes in the population.</value>
        IList<IGenome> Genomes { get; }


        /// <summary>
        /// Set the innovations collection.
        /// </summary>
        IInnovationList Innovations { get; set; }


        /// <summary>
        /// Set the old age penalty.
        /// </summary>
        double OldAgePenalty { get; set; }


        /// <summary>
        /// Set the age at which a genome is considered "old".
        /// </summary>
        int OldAgeThreshold { get; set; }


        /// <summary>
        /// Set the max population size.
        /// </summary>
        int PopulationSize { get; set; }


        /// <value>A list of species.</value>
        IList<ISpecies> Species { get; }


        /// <summary>
        /// Set the survival rate.
        /// </summary>
        ///
        /// <value>The survival rate.</value>
        double SurvivalRate { get; set; }


        /// <value>The age, below which, a genome is considered "young".</value>
        int YoungBonusAgeThreshold { get; }


        /// <summary>
        /// Set the youth score bonus.
        /// </summary>
        double YoungScoreBonus { get; set; }


        /// <summary>
        /// Set the age at which genoms are considered young.
        /// </summary>
        ///
        /// <value>The age.</value>
        int YoungBonusAgeThreshhold { set; }

        /// <summary>
        /// Add a genome to the population.
        /// </summary>
        ///
        /// <param name="genome">The genome to add.</param>
        void Add(IGenome genome);

        /// <returns>Assign a gene id.</returns>
        long AssignGeneID();


        /// <returns>Assign a genome id.</returns>
        long AssignGenomeID();


        /// <returns>Assign an innovation id.</returns>
        long AssignInnovationID();


        /// <returns>Assign a species id.</returns>
        long AssignSpeciesID();

        /// <summary>
        /// Clear all genomes from this population.
        /// </summary>
        ///
        void Clear();

        /// <summary>
        /// Get a genome by index.  Index 0 is the best genome.
        /// </summary>
        ///
        /// <param name="i">The genome to get.</param>
        /// <returns>The genome at the specified index.</returns>
        IGenome Get(int i);


        /// <returns>The size of the population.</returns>
        int Size();

        /// <summary>
        /// Sort the population by best score.
        /// </summary>
        ///
        void Sort();

        /// <summary>
        /// Claim the population, before training.
        /// </summary>
        ///
        /// <param name="ga">The GA that is claiming.</param>
        void Claim(GeneticAlgorithm ga);
    }
}
