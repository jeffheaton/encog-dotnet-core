// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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

using Encog.Neural.Genetic;
using Encog.Matrix;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// This class loads financial data from Yahoo.
    /// </summary>
    abstract public class NeuralChromosome : Chromosome<double>
    {
        /// <summary>
        /// The neural network associated with this chromosome.
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
        /// The genes that make up this chromosome.
        /// </summary>
        public override double[] Genes
        {
            set
            {
                // copy the new genes
                base.Genes = value;

                CalculateCost();
            }
            get
            {
                return base.Genes;
            }
        }


        /// <summary>
        /// The range to mutate over.  How large of mutations are allowed.
        /// </summary>
        private const double RANGE = 20.0;

        /// <summary>
        /// The current best neural network.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Initialize the genes to zero.
        /// </summary>
        /// <param name="length">How many genes should be initialized.</param>
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
        override public void Mutate()
        {
            Random rand = new Random();

            int length = this.Genes.Length;
            for (int i = 0; i < length; i++)
            {
                double d = GetGene(i);
                double ratio = (int)((RANGE * rand.NextDouble()) - RANGE);
                d *= ratio;
                SetGene(i, d);
            }
        }

        /// <summary>
        /// Update the gene values from the network values.
        /// </summary>
        public void UpdateGenes()
        {
            this.Genes = MatrixCODEC.NetworkToArray(this.network);
        }

        /// <summary>
        /// Update the neural network from the gene values.
        /// </summary>
        public void UpdateNetwork()
        {
            MatrixCODEC.ArrayToNetwork(this.Genes, this.network);
        }
    }
}

