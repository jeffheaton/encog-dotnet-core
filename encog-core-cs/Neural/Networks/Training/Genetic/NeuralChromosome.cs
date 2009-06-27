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
using Encog.Util.Randomize;
using Encog.Solve.Genetic;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a chromosome that allows a feedforward neural
    /// network to be trained using a genetic algorithm. The chromosome for a feed
    /// forward neural network is the weight and threshold matrix.
    /// 
    /// This class is abstract. If you wish to train the neural network using
    /// training sets, you should use the TrainingSetNeuralChromosome class. If you
    /// wish to use a cost function to train the neural network, then implement a
    /// subclass of this one that properly calculates the cost.
    /// 
    /// The generic type GA_TYPE specifies the GeneticAlgorithm derived class that
    /// implements the genetic algorithm that this class is to be used with.
    /// </summary>
    public abstract class NeuralChromosome
            : Chromosome<double>
    {

        /// <summary>
        /// The amount of distortion to perform a mutation.
        /// </summary>
        public const double DISTORT_FACTOR = 4.0;


        /// <summary>
        /// Mutation range.
        /// </summary>
        private IRandomizer mutateUtil = new Distort(DISTORT_FACTOR);

        /// <summary>
        /// The network to train.
        /// </summary>
        private BasicNetwork network;


        /// <summary>
        /// The network.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
            set
            {
                this.network = value;
            }
        }

        /// <summary>
        /// Init the genes array.
        /// </summary>
        /// <param name="length">The length to create.</param>
        public void InitGenes(int length)
        {
            double[] result = new double[length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0;
            }
            SetGenesDirect(result);
        }

        /// <summary>
        /// Mutate this chromosome randomly.
        /// </summary>
        public override void Mutate()
        {
            this.mutateUtil.Randomize(this.Genes);
        }

        /// <summary>
        /// Genes for this chromosome.
        /// </summary>
        public override double[] Genes
        {
            set
            {
                base.SetGenesDirect(value);
                CalculateCost();
            }
        }



        /// <summary>
        /// Copy the network to the genes.
        /// </summary>
        public void UpdateGenes()
        {
            this.Genes = NetworkCODEC.NetworkToArray(this.network);
        }

        /// <summary>
        /// Copy the genes to the network.
        /// </summary>
        public void UpdateNetwork()
        {
            NetworkCODEC.ArrayToNetwork(this.Genes, this.network);
        }
    }
}
