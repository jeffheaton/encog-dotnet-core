using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Innovation;
using Encog.Solve.Genetic.Species;

namespace Encog.Solve.Genetic.Population
{
    public interface IPopulation
    {
        
	/**
	 * Add a genome to the population.
	 * @param genome The genome to add.
	 */
	void add(IGenome genome);

	/**
	 * Add all of the specified members to this population.
	 * @param newPop A list of new genomes to add.
	 */
	void addAll(IList<IGenome> newPop);

	/**
	 * @return Assign a gene id.
	 */
	long assignGeneID();

	/**
	 * @return Assign a genome id.
	 */
	long assignGenomeID();

	/**
	 * @return Assign an innovation id.
	 */
	long assignInnovationID();

	/**
	 * @return Assign a species id.
	 */
	long assignSpeciesID();

	/**
	 * Clear all genomes from this population.
	 */
	void clear();

	
	/**
	 * Get a genome by index.  Index 0 is the best genome.
	 * @param i The genome to get.
	 */
	IGenome get(int i);
		
	/**
	 * @return The best genome in the population.
	 */
	IGenome getBest();

	/**
	 * @return The genomes in the population.
	 */
	IList<IGenome> getGenomes();

	/**
	 * @return A list of innovations in this population.
	 */
	IInnovationList getInnovations();

	/**
	 * @return The percent to decrease "old" genom's score by.
	 */
	double getOldAgePenalty();

	/**
	 * @return The age at which to consider a genome "old".
	 */
	int getOldAgeThreshold();

	/**
	 * @return The max population size.
	 */
	int getPopulationSize();

	/**
	 * @return A list of species.
	 */
	IList<ISpecies> getSpecies();

	/**
	 * @return The survival rate.
	 */
	double getSurvivalRate();

	/**
	 * @return The age, below which, a genome is considered "young".
	 */
	int getYoungBonusAgeThreshold();

	/**
	 * @return The bonus given to "young" genomes.
	 */
	double getYoungScoreBonus();

	/**
	 * Set the innovations collection.
	 * @param innovations The innovations collection.
	 */
	void setInnovations(IInnovationList innovations);

	/**
	 * Set the old age penalty.
	 * @param oldAgePenalty The old age penalty.
	 */
	void setOldAgePenalty(double oldAgePenalty);

	/**
	 * Set the age at which a genome is considered "old".
	 * @param oldAgeThreshold
	 */
	void setOldAgeThreshold(int oldAgeThreshold);

	/**
	 * Set the max population size.
	 * @param populationSize The max population size.
	 */
	void setPopulationSize( int populationSize);

	/**
	 * Set the survival rate.
	 * @param survivalRate The survival rate.
	 */
	void setSurvivalRate(double survivalRate);

	/**
	 * Set the age at which genoms are considered young.
	 * @param youngBonusAgeThreshhold The age.
	 */
	void setYoungBonusAgeThreshhold(int youngBonusAgeThreshhold);

	/**
	 * Set the youth score bonus.
	 * @param youngScoreBonus The bonus.
	 */
	void setYoungScoreBonus(double youngScoreBonus);

	/**
	 * @return The size of the population.
	 */
	int size();

	/**
	 * Sort the population by best score.
	 */
	void sort();

    }
}
