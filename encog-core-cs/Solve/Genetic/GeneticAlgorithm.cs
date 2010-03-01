// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.MathUtil.Concurrency;
using Encog.MathUtil;
using System.Threading;

namespace Encog.Solve.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm. This is an abstract class.
    /// Other classes are provided in this book that use this base class to train
    /// neural networks or provide an answer to the traveling salesman problem.
    /// 
    /// The genetic algorithm is also capable of using a thread pool to speed
    /// execution.
    /// </summary>
    /// <typeparam name="GENE_TYPE">The datatype of the gene.</typeparam>
    public abstract class GeneticAlgorithm<GENE_TYPE>
    {
        /// <summary>
        /// Threadpool timeout.
        /// </summary>
        public const int TIMEOUT = 120;

        /// <summary>
        /// How many chromosomes should be created.
        /// </summary>
        private int populationSize;

        /// <summary>
        /// The percent that should mutate.
        /// </summary>
        private double mutationPercent;

        /// <summary>
        /// What percent should be chosen to mate. They will choose partners from the
        /// entire mating population.
        /// </summary>
        private double percentToMate;

        /// <summary>
        /// Percent of the population that the mating population chooses partners.
        /// from.
        /// </summary>
        private double matingPopulation;

        /// <summary>
        /// Should the same gene be prevented from repeating.
        /// </summary>
        private bool preventRepeat;

        /// <summary>
        /// How much genetic material should be cut when mating.
        /// </summary>
        private int cutLength;

        /// <summary>
        /// Should the score be minimized.
        /// </summary>
        public bool ShouldMinimize { get; set; }

        /// <summary>
        /// The population.
        /// </summary>
        private Chromosome<GENE_TYPE>[] chromosomes;


        /// <summary>
        /// Return the entire population.
        /// </summary>
        public Chromosome<GENE_TYPE>[] Chromosomes
        {
            get
            {
                return this.chromosomes;
            }
            set
            {
                this.chromosomes = value;
            }
        }

        /// <summary>
        /// The cut length.
        /// </summary>
        public int CutLength
        {
            get
            {
                return this.cutLength;
            }
            set
            {
                this.cutLength = value;
            }
        }

        /// <summary>
        /// The mating population percent.
        /// </summary>
        public double MatingPopulation
        {
            get
            {
                return this.matingPopulation;
            }
            set
            {
                this.matingPopulation = value;
            }

        }

        /// <summary>
        /// The mutation percent.
        /// </summary>
        public double MutationPercent
        {
            get
            {
                return this.mutationPercent;
            }
            set
            {
                this.mutationPercent = value;
            }
        }

        /// <summary>
        /// The percent to mate.
        /// </summary>
        public double PercentToMate
        {
            get
            {
                return this.percentToMate;
            }
            set
            {
                this.percentToMate = value;
            }
        }

        /// <summary>
        /// The population size.
        /// </summary>
        public int PopulationSize
        {
            get
            {
                return this.populationSize;
            }
            set
            {
                this.populationSize = value;
            }
        }

        /// <summary>
        /// Should repeating genes be prevented.
        /// </summary>
        public bool PreventRepeat
        {
            get
            {
                return this.preventRepeat;
            }
            set
            {
                this.preventRepeat = value;
            }
        }

        /// <summary>
        /// Construct object.
        /// </summary>
        public GeneticAlgorithm()
        {
            this.ShouldMinimize = true;
        }

        /// <summary>
        /// Modify the weight matrix and thresholds based on the last call to
        /// calcError.
        /// </summary>
        public void Iteration()
        {

            int countToMate = (int)(this.PopulationSize
                   * this.PercentToMate);
            int offspringCount = countToMate * 2;
            int offspringIndex = this.PopulationSize - offspringCount;
            int matingPopulationSize = (int)(this.PopulationSize
                   * this.MatingPopulation);

            // mate and form the next generation
            for (int i = 0; i < countToMate; i++)
            {
                Chromosome<GENE_TYPE> mother = this.chromosomes[i];
                int fatherInt = (int)(ThreadSafeRandom.NextDouble() * matingPopulationSize);
                Chromosome<GENE_TYPE> father = this.chromosomes[fatherInt];
                Chromosome<GENE_TYPE> child1 =
                   this.chromosomes[offspringIndex];
                Chromosome<GENE_TYPE> child2 =
                   this.chromosomes[offspringIndex + 1];

                MateWorker<GENE_TYPE> worker
                 = new MateWorker<GENE_TYPE>(
                       mother, father, child1, child2);

                EncogConcurrency.Instance.ProcessTask(worker);

                offspringIndex += 2;
            }

            EncogConcurrency.Instance.Shutdown(5);

            // sort the next generation
            SortChromosomes();
        }

        /// <summary>
        /// Sort the chromosomes.
        /// </summary>
        public void SortChromosomes()
        {
            Array.Sort(this.chromosomes);
        }

        /// <summary>
        /// Define the cut length to be 1/3 the length of a chromosome. This is a
	    /// good default value for it. If there are no chromosomes yet this call will
	    /// set the cut length to 0.
        /// </summary>
        public void DefineCutLength()
        {
            if (this.chromosomes.Length > 0)
            {
                int size = this.chromosomes[0].Genes.Length;
                CutLength = (size / 3);
            }
        }

    }

}
