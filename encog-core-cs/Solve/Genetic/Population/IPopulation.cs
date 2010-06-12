// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Innovation;
using Encog.Solve.Genetic.Species;
using Encog.Persist;

namespace Encog.Solve.Genetic.Population
{
    /// <summary>
    /// A population of genomes.
    /// </summary>
    public interface IPopulation: IEncogPersistedObject
    {
        /// <summary>
        /// Add a genome to the population.
        /// </summary>
        /// <param name="genome">The genome to add.</param>
        void Add(IGenome genome);

        /// <summary>
        /// Add all of the specified members to this population.
        /// </summary>
        /// <param name="newPop">A list of new genomes to add.</param>
        void AddAll(IList<IGenome> newPop);

        /// <summary>
        /// Assign a gene id.
        /// </summary>
        /// <returns>The gene id.</returns>
        long AssignGeneID();

        /// <summary>
        /// Assign a genome id.
        /// </summary>
        /// <returns>The genome id.</returns>
        long AssignGenomeID();

        /// <summary>
        /// Assign an innovation id.
        /// </summary>
        /// <returns>The innovation id.</returns>
        long AssignInnovationID();

        /// <summary>
        /// Assign a species id.
        /// </summary>
        /// <returns></returns>
        long AssignSpeciesID();

        /// <summary>
        /// Clear all genomes from this population.
        /// </summary>
        void Clear();

        /// <summary>
        /// The best genome in the population.
        /// </summary>
        /// <returns></returns>
        IGenome GetBest();

        /// <summary>
        /// The genomes in the population.
        /// </summary>
        IList<IGenome> Genomes { get; }

        /// <summary>
        /// A list of innovations in this population.
        /// </summary>
        IInnovationList Innovations { get; set; }

        /// <summary>
        /// The percent to decrease "old" genom's score by.
        /// </summary>
        double OldAgePenalty { get; set; }

        /// <summary>
        /// The age at which to consider a genome "old".
        /// </summary>
        int OldAgeThreshold { get; set; }

        /// <summary>
        /// The max population size.
        /// </summary>
        int PopulationSize { get; set; }

        /// <summary>
        /// A list of species.
        /// </summary>
        IList<ISpecies> Species { get; }

        /// <summary>
        /// The survival rate.
        /// </summary>
        double SurvivalRate { get; set; }

        /// <summary>
        /// The age, below which, a genome is considered "young".
        /// </summary>
        int YoungBonusAgeThreshold { get; set; }

        /// <summary>
        /// The bonus given to "young" genomes.
        /// </summary>
        double YoungScoreBonus { get; set; }

        /// <summary>
        /// Sort the population by best score.
        /// </summary>
        void Sort();

    }
}
