using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genes;

namespace Encog.Solve.Genetic.Genome
{
    /// <summary>
    /// Implements a chromosome to genetic algorithm. This is an abstract class.
    /// Other classes are provided in this book that use this base class to train
    /// neural networks or provide an answer to the traveling salesman problem. 
    /// Chromosomes are made up of genes. 
    /// 
    /// Genomes in this genetic algorithm consist of one or more chromosomes.
    /// </summary>
    public class Chromosome
    {
        /// <summary>
        /// The individual elements of this chromosome.
        /// </summary>
        private IList<IGene> genes = new List<IGene>();

        /// <summary>
        /// Used the get the entire gene list
        /// </summary>
        public IList<IGene> Genes
        {
            get
            {
                return genes;
            }
        }
    }
}
