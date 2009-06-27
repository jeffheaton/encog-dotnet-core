// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
