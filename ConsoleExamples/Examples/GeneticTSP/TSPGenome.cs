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
using Encog.Examples.Util;
using Encog.MathUtil;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic;
using Encog.ML.Genetic.Genes;

namespace Encog.Examples.GeneticTSP
{
    public class TSPGenome : BasicGenome
    {
        private Chromosome pathChromosome;

        public TSPGenome(GeneticAlgorithm owner, City[] cities)
        {

            int[] organism = new int[cities.Length];
            bool[] taken = new bool[cities.Length];

            for (int i = 0; i < organism.Length; i++)
            {
                taken[i] = false;
            }
            for (int i = 0; i < organism.Length - 1; i++)
            {
                int icandidate;
                do
                {
                    icandidate = (int)(ThreadSafeRandom.NextDouble() * organism.Length);
                } while (taken[icandidate]);
                organism[i] = icandidate;
                taken[icandidate] = true;
                if (i == organism.Length - 2)
                {
                    icandidate = 0;
                    while (taken[icandidate])
                    {
                        icandidate++;
                    }
                    organism[i + 1] = icandidate;
                }
            }

            this.pathChromosome = new Chromosome();
            this.Chromosomes.Add(this.pathChromosome);

            for (int i = 0; i < organism.Length; i++)
            {
                IntegerGene gene = new IntegerGene();
                gene.Value = organism[i];
                this.pathChromosome.Genes.Add(gene);
            }
            Organism = organism;

            Encode();

        }

        public override void Decode()
        {
            Chromosome chromosome = this.Chromosomes[0];
            int[] organism = new int[chromosome.Genes.Count];

            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                IntegerGene gene = (IntegerGene)chromosome.Genes[i];
                organism[i] = gene.Value;
            }

            Organism = organism;
        }

        public override void Encode()
        {
            Chromosome chromosome = this.Chromosomes[0];

            int[] organism = (int[])this.Organism;

            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                IntegerGene gene = (IntegerGene)chromosome.Genes[i];
                gene.Value = organism[i];
            }
        }
    }
}
