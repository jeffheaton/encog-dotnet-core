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

namespace Encog.Solve.Genetic.Species
{
    /// <summary>
    /// A species is a set of related genomes.  Crossover occurs within a species.
    /// </summary>
    public interface ISpecies
    {
        /// <summary>
        /// Calculate the amount that a species will spawn.
        /// </summary>
        void CalculateSpawnAmount();

        /// <summary>
        /// Choose a worthy parent for mating.
        /// </summary>
        /// <returns>The parent genome.</returns>
        IGenome ChooseParent();

        /// <summary>
        /// The age of this species.
        /// </summary>
        int Age { get; set; }

        /// <summary>
        /// The best score for this species.
        /// </summary>
        double BestScore { get; set; }

        /// <summary>
        /// How many generations with no improvement.
        /// </summary>
        int GensNoImprovement { get; set; }

        /// <summary>
        /// Get the leader for this species. The leader is the genome with
        /// the best score.
        /// </summary>
        IGenome Leader { get; set; }

        /// <summary>
        /// The numbers of this species.
        /// </summary>
        IList<IGenome> Members { get; }

        /// <summary>
        /// The number of genomes this species will try to spawn into the
        /// next generation.
        /// </summary>
        double NumToSpawn { get; set; }

        /// <summary>
        /// The species ID.
        /// </summary>
        long SpeciesID { get; set; }

        /// <summary>
        /// Purge old unsuccessful genomes.
        /// </summary>
        void Purge();

    }
}
