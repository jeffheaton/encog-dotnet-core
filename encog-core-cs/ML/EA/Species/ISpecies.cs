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
using Encog.ML.EA.Population;

namespace Encog.ML.EA.Species
{
    /// <summary>
    ///     Defines a species.
    /// </summary>
    public interface ISpecies
    {
        /// <summary>
        ///     The age of this species.
        /// </summary>
        int Age { get; set; }

        /// <summary>
        ///     The best score for this species.
        /// </summary>
        double BestScore { get; set; }

        /// <summary>
        ///     The number of generations with no imrpvement.
        /// </summary>
        int GensNoImprovement { get; set; }

        /// <summary>
        ///     The leader of this species.
        /// </summary>
        IGenome Leader { get; set; }

        /// <summary>
        ///     The members of this species.
        /// </summary>
        List<IGenome> Members { get; set; }

        /// <summary>
        ///     Get the offspring count.
        /// </summary>
        int OffspringCount { get; set; }

        /// <summary>
        ///     The offspring share for the next iteration's population.
        /// </summary>
        double OffspringShare { get; }

        /// <summary>
        ///     The population.
        /// </summary>
        IPopulation Population { get; set; }

        /// <summary>
        ///     Add a genome to this species.
        /// </summary>
        /// <param name="genome">The genome to add.</param>
        void Add(IGenome genome);

        /// <summary>
        ///     Calculate this genome's share of the next population.
        /// </summary>
        /// <param name="shouldMinimize">True if we see to minimize the score.</param>
        /// <param name="maxScore">The best score.</param>
        /// <returns>The share of this species, as a percent ratio.</returns>
        double CalculateShare(bool shouldMinimize, double maxScore);
    }
}
