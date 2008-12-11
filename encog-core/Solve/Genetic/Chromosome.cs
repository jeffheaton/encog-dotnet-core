// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic;

namespace Encog.Neural.Genetic
{
    /// <summary>
    /// Chromosome: Implements a chromosome to genetic algorithm. This is an abstract
    /// class. Other classes are provided in this book that use this base class to
    /// train neural networks or provide an answer to the traveling salesman problem.
    /// 
    /// Lifeforms in this genetic algorithm consist of one single chromosome each.
    /// Therefore, this class represents a virtual lifeform. The chromosome is a
    /// string of objects that represent one solution. For a neural network, this
    /// string of objects usually represents the weight and threshold matrix.
    /// 
    /// Chromosomes are made up of genes. These are of the generic type GENE_TYPE.
    /// For a neural network this type would most likely be double values.
    /// </summary>
    /// <typeparam name="GENE_TYPE">The datatype for the genes.</typeparam>
    abstract public class Chromosome<GENE_TYPE> : IComparable<Chromosome<GENE_TYPE>>
    {
        /// <summary>
        /// The cost for this chromosome.
        /// </summary>
        public double Cost
        {
            get
            {
                return this.cost;
            }
            set
            {
                this.cost = value;
            }
        }

        /// <summary>
        /// The genes for this chromosome.
        /// </summary>
        public virtual GENE_TYPE[] Genes
        {
            get
            {
                return this.genes;
            }
            set
            {
                this.genes = value;
            }
        }

        /// <summary>
        /// Get the size of the gene array.
        /// </summary>
        public int Size
        {
            get
            {
                return this.genes.Length;
            }
        }

        /// <summary>
        /// The genetic algorithm that this chromosome belongs to.
        /// </summary>
        public GeneticAlgorithm<GENE_TYPE> GA
        {
            get
            {
                return this.geneticAlgorithm;
            }
            set
            {
                this.geneticAlgorithm = value;
            }
        }

        /// <summary>
        /// The cost for this chromosome.  The lower the better.
        /// </summary>
        private double cost;

        /// <summary>
        /// The individual elements of this chromosome.
        /// </summary>
        private GENE_TYPE[] genes;

        /// <summary>
        /// The genetic algorithm that this chromosome is associated with.
        /// </summary>
        private GeneticAlgorithm<GENE_TYPE> geneticAlgorithm;

        /// <summary>
        /// Called to calculate the cost for this chromosome.
        /// </summary>
        abstract public void CalculateCost();

        /// <summary>
        /// Used to compare two chromosomes. Used to sort by cost.
        /// </summary>
        /// <param name="other">The other chromosome to compare.</param>
        /// <returns>The value 0 if the argument is a chromosome that has an equal
        /// cost to this chromosome; a value less than 0 if the argument is a
        /// chromosome with a cost greater than this chromosome; and a value
        /// greater than 0 if the argument is a chromosome what a cost less
        /// than this chromosome.</returns>
        public int CompareTo(Chromosome<GENE_TYPE> other)
        {
            if (this == other)
                return 0;
            else if (this.Cost > other.Cost)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Get the specified gene.
        /// </summary>
        /// <param name="gene">The specified gene.</param>
        /// <returns>The gene specified.</returns>
        public GENE_TYPE GetGene(int gene)
        {
            return this.genes[gene];
        }


        /// <summary>
        /// Get a list of the genes that have not been taken before. This is useful
        /// if you do not wish the same gene to appear more than once in a
        /// chromosome.
        /// </summary>
        /// <param name="source">The pool of genes to select from.</param>
        /// <param name="taken">An array of the taken genes.</param>
        /// <returns>Those genes in source that are not taken.</returns>
        private GENE_TYPE GetNotTaken(Chromosome<GENE_TYPE> source,
                IList<GENE_TYPE> taken)
        {
            int geneLength = source.Size;

            for (int i = 0; i < geneLength; i++)
            {
                GENE_TYPE trial = source.GetGene(i);
                if (!taken.Contains(trial))
                {
                    taken.Add(trial);
                    return trial;
                }
            }

            return default(GENE_TYPE);
        }


        /// <summary>
        /// Assuming this chromosome is the "mother" mate with the passed in
        /// "father".
        /// </summary>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">Returns the first offspring.</param>
        /// <param name="offspring2">Returns the second offspring.</param>
        public void Mate(Chromosome<GENE_TYPE> father,
                 Chromosome<GENE_TYPE> offspring1,
                 Chromosome<GENE_TYPE> offspring2)
        {
            int geneLength = this.Genes.Length;


            Random rand = new Random();

            // the chromosome must be cut at two positions, determine them
            int cutpoint1 = (int)(rand.NextDouble() * (geneLength - this.GA.CutLength));
            int cutpoint2 = cutpoint1 + this.GA.CutLength;

            // keep track of which cities have been taken in each of the two
            // offspring, defaults to false.
            IList<GENE_TYPE> taken1 = new List<GENE_TYPE>();
            IList<GENE_TYPE> taken2 = new List<GENE_TYPE>();

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                }
                else
                {
                    offspring1.SetGene(i, father.GetGene(i));
                    offspring2.SetGene(i, this.GetGene(i));
                    taken1.Add(offspring1.GetGene(i));
                    taken2.Add(offspring2.GetGene(i));
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    if (this.GA.PreventRepeat)
                    {
                        offspring1.SetGene(i, GetNotTaken(this, taken1));
                        offspring2.SetGene(i, GetNotTaken(father, taken2));
                    }
                    else
                    {
                        offspring1.SetGene(i, this.GetGene(i));
                        offspring2.SetGene(i, father.GetGene(i));
                    }
                }
            }

            // copy results
            offspring1.CalculateCost();
            offspring2.CalculateCost();

            if (rand.NextDouble() < this.geneticAlgorithm.MutationPercent)
            {
                offspring1.Mutate();
            }
            if (rand.NextDouble() < this.geneticAlgorithm.MutationPercent)
            {
                offspring2.Mutate();
            }

        }

        /// <summary>
        /// Called to mutate this chromosome.
        /// </summary>
        abstract public void Mutate();




        /// <summary>
        /// Set the specified gene's value.
        /// </summary>
        /// <param name="gene">The specified gene.</param>
        /// <param name="value">The value to set the specified gene to.</param>
        public void SetGene(int gene, GENE_TYPE value)
        {
            this.genes[gene] = value;
        }


        /// <summary>
        /// Set the genes directly, not allowed to be overridden.
        /// </summary>
        /// <param name="genes">The genes to set.</param>
        public void SetGenesDirect(GENE_TYPE[] genes)
        {
            this.genes = genes;
        }


        /// <summary>
        /// Convert the chromosome to a string.
        /// </summary>
        /// <returns>The chromosome as a string.</returns>
        override public String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[Chromosome: cost=");
            builder.Append(this.Cost);
            return builder.ToString();
        }
    }
}
