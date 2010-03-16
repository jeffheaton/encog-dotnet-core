using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genes;

namespace Encog.Solve.Genetic.Genome
{
    public class Chromosome
    {
        /**
	 * The individual elements of this chromosome.
	 */
	private IList<IGene> genes = new List<IGene>();

	/**
	 * Add a gene.
	 * @param gene The gene to add.
	 */
	public void add( IGene gene) {
		genes.Add(gene);

	}

	/**
	 * Get an individual gene.
	 * @param i The index of the gene.
	 * @return The gene.
	 */
	public IGene get( int i) {
		return genes[i];
	}

	/**
	 * Get the specified gene.
	 * 
	 * @param gene
	 *            The specified gene.
	 * @return The gene specified.
	 */
	public IGene getGene(int gene) {
		return genes[gene];
	}

	/**
	 * Used the get the entire gene list.
	 * 
	 * @return the genes
	 */
	public IList<IGene> getGenes() {
		return genes;
	}

	/**
	 * @return The number of genes in this chromosome.
	 */
	public int size() {
		return genes.Count;
	}
    }
}
