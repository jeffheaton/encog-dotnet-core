using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Randomize;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Genetic
{

    /// <summary>
    /// Implements a genetic algorithm that allows a neural network to be trained
    /// using a genetic algorithm. This algorithm is for a neural network. The neural
    /// network is trained using training sets.
    /// </summary>
    public class TrainingSetNeuralGeneticAlgorithm : NeuralGeneticAlgorithm
    {
        /// <summary>
        /// Construct a training object.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="randomizer">The randomizer to use to create new networks.</param>
        /// <param name="training">The training set.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="mutationPercent">The mutation percent.</param>
        /// <param name="percentToMate">The percent to mate.</param>
        public TrainingSetNeuralGeneticAlgorithm(BasicNetwork network,
                 IRandomizer randomizer, INeuralDataSet training,
                 int populationSize, double mutationPercent,
                 double percentToMate)
            : base()
        {

            this.Genetic.MutationPercent = mutationPercent;
            this.Genetic.MatingPopulation = percentToMate * 2;
            this.Genetic.PopulationSize = populationSize;
            this.Genetic.PercentToMate = percentToMate;

            this.Training = training;

            this.Genetic.Chromosomes =
                    new TrainingSetNeuralChromosome[this.Genetic.PopulationSize];
            for (int i = 0; i < this.Genetic.Chromosomes.Length; i++)
            {
                BasicNetwork chromosomeNetwork = (BasicNetwork)network
                       .Clone();
                randomizer.Randomize(chromosomeNetwork);

                TrainingSetNeuralChromosome c =
                   new TrainingSetNeuralChromosome(
                       this, chromosomeNetwork);
                c.UpdateGenes();
                this.Genetic.Chromosomes[i] = c;
            }
            this.Genetic.SortChromosomes();
        }

    }

}
