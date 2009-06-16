using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a chromosome that allows a
    /// feedforward neural network to be trained using a genetic algorithm. The
    /// network is trained using training sets.
    /// 
    /// The chromosome for a feed forward neural network is the weight and threshold
    /// matrix.
    /// </summary>
    public class TrainingSetNeuralChromosome : NeuralChromosome
    {
        /// <summary>
        /// The constructor, takes a list of cities to set the initial "genes" to.
        /// </summary>
        /// <param name="genetic">The genetic algorithm used with this chromosome.</param>
        /// <param name="network">The neural network to train.</param>
        public TrainingSetNeuralChromosome(
                 TrainingSetNeuralGeneticAlgorithm genetic,
                 BasicNetwork network)
        {
            this.GeneticAlgorithm = genetic.Genetic;
            this.genetic = genetic;
            this.Network = network;

            InitGenes(network.WeightMatrixSize);
            UpdateGenes();
        }

        /// <summary>
        /// The genetic algorithm being used.
        /// </summary>
        private TrainingSetNeuralGeneticAlgorithm genetic;

        /// <summary>
        /// Calculate the cost for this chromosome.
        /// </summary>
        public override void CalculateCost()
        {
            // update the network with the new gene values
            UpdateNetwork();

            // update the cost with the new genes
            this.Cost = this.Network.CalculateError(this.genetic.Training);

        }

        /// <summary>
        /// Get all genes.
        /// </summary>
        public override double[] Genes
        {
            set
            {
                // copy the new genes
                base.Genes = value;

                CalculateCost();
            }

        }
    }

}
