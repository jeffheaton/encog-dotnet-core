//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Species;

namespace Encog.ML.EA.Population
{
    /// <summary>
    ///     Defines a population of genomes.
    /// </summary>
    public interface IPopulation : IMLMethod
    {
        /// <summary>
        ///     The best genome in the population.
        /// </summary>
        IGenome BestGenome { get; set; }

        /// <summary>
        ///     A factory used to create genomes.
        /// </summary>
        IGenomeFactory GenomeFactory { get; set; }

        /// <summary>
        ///     The max size that an individual can become.
        /// </summary>
        int MaxIndividualSize { get; }

        /// <summary>
        ///     The max population size.
        /// </summary>
        int PopulationSize { get; set; }

        /// <summary>
        ///     The size of the population.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     The list of species.
        /// </summary>
        List<ISpecies> Species { get; }

        /// <summary>
        ///     Clear all genomes from this population.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Create a species.
        /// </summary>
        /// <returns>The newly created species.</returns>
        ISpecies CreateSpecies();

        /// <summary>
        ///     Determine which species has the top genome.
        /// </summary>
        /// <returns>The species with the top genome.</returns>
        ISpecies DetermineBestSpecies();

        /// <summary>
        ///     Flatten the species into a single list of genomes.
        /// </summary>
        /// <returns>The genomes that make up all species in the population.</returns>
        IList<IGenome> Flatten();

        /// <summary>
        ///     Purge invalid genomes.
        /// </summary>
        void PurgeInvalidGenomes();
    }
}
