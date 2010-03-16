using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic.Species
{
    public interface ISpecies
    {
        /**
	 * Add a genome to this species.
	 * 
	 * @param genome
	 *            The genome to add.
	 */
        void addMember(IGenome genome);

        /**
         * Adjust the score of this species. This is where old age and youth
         * bonus/penalties happen.
         */
        void adjustScore();

        /**
         * Calculate the amount that a species will spawn.
         */
        void calculateSpawnAmount();

        /**
         * Choose a worthy parent for mating.
         * 
         * @return The parent genome.
         */
        IGenome chooseParent();

        /**
         * @return The age of this species.
         */
        int getAge();

        /**
         * @return The best score for this species.
         */
        double getBestScore();

        /**
         * @return How many generations with no improvement.
         */
        int getGensNoImprovement();

        /**
         * @return Get the leader for this species. The leader is the genome with
         *         the best score.
         */
        IGenome getLeader();

        /**
         * @return The numbers of this species.
         */
        IList<IGenome> getMembers();

        /**
         * @return The number of genomes this species will try to spawn into the
         *         next generation.
         */
        double getNumToSpawn();

        /**
         * @return The number of spawns this species requires.
         */
        double getSpawnsRequired();

        /**
         * @return The species ID.
         */
        long getSpeciesID();

        /**
         * Purge old unsuccessful genomes.
         */
        void purge();

        /**
         * Set the age of this species.
         * @param age The age.
         */
        void setAge(int age);

        /**
         * Set the best score.
         * @param bestScore The best score.
         */
        void setBestScore(double bestScore);

        /**
         * Set the number of generations with no improvement.
         * @param gensNoImprovement The number of generations with
         * no improvement.
         */
        void setGensNoImprovement(int gensNoImprovement);

        /**
         * Set the leader of this species.
         * @param leader The leader of this species.
         */
        void setLeader(IGenome leader);

        /**
         * Set the number of spawns required.
         * @param spawnsRequired The number of spawns required.
         */
        void setSpawnsRequired(double spawnsRequired);
    }
}
