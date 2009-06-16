using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic;
using log4net;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm that allows a feedforward neural network to be
    /// trained using a genetic algorithm. This algorithm is for a feed forward
    /// neural network.
    /// 
    /// This class is somewhat undefined. If you wish to train the neural network 
    /// using training sets, you should use the TrainingSetNeuralGeneticAlgorithm 
    /// class. If you wish to use a cost function to train the neural network, 
    /// then implement a subclass of this one that properly calculates the cost.
    /// </summary>
    public class NeuralGeneticAlgorithm : BasicTraining
    {
        /// <summary>
        /// Very simple class that implements a genetic algorithm.
        /// </summary>
        public class NeuralGeneticAlgorithmHelper : GeneticAlgorithm<double>
        {
            /// <summary>
            /// The error from the last iteration.
            /// </summary>
            public double Error
            {
                get
                {
                    return this.Chromosomes[0].Cost;
                }
            }

            /// <summary>
            /// Get the current best neural network.
            /// </summary>
            public BasicNetwork Network
            {
                get
                {
                    NeuralChromosome c = (NeuralChromosome)this.Chromosomes[0];
                    c.UpdateNetwork();
                    return c.Network;
                }
            }
        }

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(NeuralGeneticAlgorithm));

        /// <summary>
        /// Simple helper class that implements the required methods to 
        /// implement a genetic algorithm.
        /// </summary>
        private NeuralGeneticAlgorithmHelper genetic;

        /// <summary>
        /// Construct the training class.
        /// </summary>
        public NeuralGeneticAlgorithm()
        {
            this.genetic = new NeuralGeneticAlgorithmHelper();
        }



        /// <summary>
        /// The network that is being trained.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.genetic.Network;
            }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {

            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("Performing Genetic iteration.");
            }
            PreIteration();
            this.Genetic.Iteration();
            this.Error = Genetic.Error;
            PostIteration();
        }

        /// <summary>
        /// 
        /// </summary>
        public NeuralGeneticAlgorithmHelper Genetic
        {
            get
            {
                return this.genetic;
            }
            set
            {
                this.genetic = value;
            }
        }
    }

}
