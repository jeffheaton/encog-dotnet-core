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

namespace Encog.Solve.Genetic.Genome
{
    /// <summary>
    /// A genome is the basic blueprint for creating an organism in Encog. A genome
    /// is made up of one or more chromosomes, which are in turn made up of genes.
    /// </summary>
    public interface IGenome: IComparable<IGenome>
    {
        /// <summary>
        /// Calculate the number of genes in this genome.
        /// </summary>
        /// <returns>The number of genes in this genome.</returns>
        int CalculateGeneCount();

        /// <summary>
        /// Use the genes to update the organism.
        /// </summary>
        void Decode();

        /// <summary>
        /// Use the organism to update the genes.
        /// </summary>
        void Encode();

        /// <summary>
        /// Get the adjusted score, this considers old-age penalties and youth
        /// bonuses. If there are no such bonuses or penalties, this is the same as
        /// the score.
        /// </summary>
        double AdjustedScore { get; set; }

        /// <summary>
        /// The amount of offspring this genome will have.
        /// </summary>
        double AmountToSpawn { get; set; }

        /// <summary>
        /// The chromosomes that make up this genome.
        /// </summary>
        IList<Chromosome> Chromosomes { get; }

        /**
         * @return The genome ID.
         */
        long GenomeID { get; set; }

        /**
         * @return The organism produced by this genome.
         */
        Object Organism { get; set; }

        /**
         * @return The score for this genome.
         */
        double Score { get; set; }

        /**
         * Mate with another genome and produce two children.
         * @param father The father genome.
         * @param child1 The first child.
         * @param child2 The second child.
         */
        void Mate(IGenome father, IGenome child1, IGenome child2);

    }
}
