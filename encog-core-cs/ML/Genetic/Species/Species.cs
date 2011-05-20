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

namespace Encog.ML.Genetic.Species
{
    /// <summary>
    /// Defines the features used in a species. A species is a group of genomes.
    /// </summary>
    ///
    public interface ISpecies
    {
        /// <summary>
        /// Set the age of this species.
        /// </summary>
        int Age { get; set; }


        /// <summary>
        /// Set the best score.
        /// </summary>
        double BestScore { get; set; }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        int GensNoImprovement { get; set; }


        /// <summary>
        /// Set the leader of this species.
        /// </summary>
        IGenome Leader { get; set; }


        /// <value>The numbers of this species.</value>
        IList<IGenome> Members { get; }


        /// <value>The number of genomes this species will try to spawn into the
        /// next generation.</value>
        double NumToSpawn { get; }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        double SpawnsRequired { get; set; }


        /// <value>The species ID.</value>
        long SpeciesID { get; }

        /// <summary>
        /// Calculate the amount that a species will spawn.
        /// </summary>
        ///
        void CalculateSpawnAmount();

        /// <summary>
        /// Choose a worthy parent for mating.
        /// </summary>
        ///
        /// <returns>The parent genome.</returns>
        IGenome ChooseParent();


        /// <summary>
        /// Purge old unsuccessful genomes.
        /// </summary>
        ///
        void Purge();
    }
}
