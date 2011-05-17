using System;
using System.Collections.Generic;
using Encog.ML.Genetic.Genes;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// Implements a chromosome to genetic algorithm. This is an abstract class.
    /// Other classes are provided in this book that use this base class to train
    /// neural networks or provide an answer to the traveling salesman problem. 
    /// Chromosomes are made up of genes. 
    /// Genomes in this genetic algorithm consist of one or more chromosomes. 
    /// </summary>
    ///
    [Serializable]
    public class Chromosome
    {
        /// <summary>
        /// The individual elements of this chromosome.
        /// </summary>
        ///
        private readonly List<IGene> genes;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public Chromosome()
        {
            genes = new List<IGene>();
        }

        /// <summary>
        /// Used the get the entire gene list.
        /// </summary>
        ///
        /// <value>the genes</value>
        public List<IGene> Genes
        {
            /// <summary>
            /// Used the get the entire gene list.
            /// </summary>
            ///
            /// <returns>the genes</returns>
            get { return genes; }
        }

        /// <summary>
        /// Add a gene.
        /// </summary>
        ///
        /// <param name="gene">The gene to add.</param>
        public void Add(IGene gene)
        {
            genes.Add(gene);
        }

        /// <summary>
        /// Get an individual gene.
        /// </summary>
        ///
        /// <param name="i">The index of the gene.</param>
        /// <returns>The gene.</returns>
        public IGene Get(int i)
        {
            return genes[i];
        }

        /// <summary>
        /// Get the specified gene.
        /// </summary>
        ///
        /// <param name="gene">The specified gene.</param>
        /// <returns>The gene specified.</returns>
        public IGene GetGene(int gene)
        {
            return genes[gene];
        }


        /// <returns>The number of genes in this chromosome.</returns>
        public int Size()
        {
            return genes.Count;
        }
    }
}