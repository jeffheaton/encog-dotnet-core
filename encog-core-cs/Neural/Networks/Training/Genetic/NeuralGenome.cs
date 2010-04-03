using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.Solve.Genetic.Genes;
using Encog.Neural.Networks.Structure;
using Encog.Solve.Genetic.Genome;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genome that allows a feedforward neural
    /// network to be trained using a genetic algorithm. The chromosome for a feed
    /// forward neural network is the weight and threshold matrix.
    /// </summary>
    public class NeuralGenome : BasicGenome
    {
        /// <summary>
        /// The network chromosome.
        /// </summary>
        private Chromosome networkChromosome;

        /// <summary>
        /// Construct a neural network genome.
        /// </summary>
        /// <param name="nga">The neural genetic algorithm.</param>
        /// <param name="network">The network.</param>
        public NeuralGenome(NeuralGeneticAlgorithm nga, BasicNetwork network)
            : base(nga.Helper)
        {
            this.Organism = network;
            this.networkChromosome = new Chromosome();

            // create an array of "double genes"
            int size = network.Structure.CalculateSize();
            for (int i = 0; i < size; i++)
            {
                IGene gene = new DoubleGene();
                this.networkChromosome.Genes.Add(gene);
            }

            this.Chromosomes.Add(this.networkChromosome);

            Encode();
        }

        /// <summary>
        /// Decode the genome to a network.
        /// </summary>
        public override void Decode()
        {
            double[] net = new double[networkChromosome.Genes.Count];
            for (int i = 0; i < net.Length; i++)
            {
                DoubleGene gene = (DoubleGene)networkChromosome.Genes[i];
                net[i] = gene.Value;

            }
            NetworkCODEC.ArrayToNetwork(net, (BasicNetwork)Organism);

        }

        /// <summary>
        /// Encode the network to a genome.
        /// </summary>
        public override void Encode()
        {
            double[] net = NetworkCODEC.NetworkToArray((BasicNetwork)Organism);

            for (int i = 0; i < net.Length; i++)
            {
                ((DoubleGene)networkChromosome.Genes[i]).Value = net[i];
            }
        }
    }
}
