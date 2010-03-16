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
    public interface IGenome
    {
        /**
	 * @return The number of genes in this genome.
	 */
        int CalculateGeneCount();

        /**
         * Use the genes to update the organism.
         */
        void Decode();

        /**
         * Use the organism to update the genes.
         */
        void Encode();

        /**
         * Get the adjusted score, this considers old-age penalties and youth
         * bonuses. If there are no such bonuses or penalties, this is the same as
         * the score.
         * @return The adjusted score.
         */
        double getAdjustedScore();

        /**
         * @return The amount of offspring this genome will have.
         */
        double getAmountToSpawn();

        /**
         * @return The chromosomes that make up this genome.
         */
        IList<Chromosome> getChromosomes();

        /**
         * @return The genome ID.
         */
        long getGenomeID();

        /**
         * @return The organism produced by this genome.
         */
        Object getOrganism();

        /**
         * @return The score for this genome.
         */
        double getScore();

        /**
         * Mate with another genome and produce two children.
         * @param father The father genome.
         * @param child1 The first child.
         * @param child2 The second child.
         */
        void mate(IGenome father, IGenome child1, IGenome child2);

        /**
         * Set the adjusted score.
         * @param adjustedScore The adjusted score.
         */
        void setAdjustedScore(double adjustedScore);

        /**
         * Set the amount to spawn.
         * @param amountToSpawn The amount to spawn.
         */
        void setAmountToSpawn(double amountToSpawn);

        /**
         * Set the genome ID.
         * @param genomeID The genome id.
         */
        void setGenomeID(long genomeID);

        /** 
         * Set the score.
         * @param score The new score.
         */
        void setScore(double score);
    }
}
