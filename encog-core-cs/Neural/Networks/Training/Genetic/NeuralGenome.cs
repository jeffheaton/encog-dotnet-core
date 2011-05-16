using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;
using Encog.Neural.Networks.Structure;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genome that allows a feedforward neural network to be trained
    /// using a genetic algorithm. The chromosome for a feed forward neural network
    /// is the weight and bias matrix.
    /// </summary>
    ///
    public class NeuralGenome : BasicGenome
    {
        /// <summary>
        /// Serial id.
        /// </summary>
        ///
        private const long serialVersionUID = 1L;

        /// <summary>
        /// The chromosome.
        /// </summary>
        ///
        private readonly Chromosome networkChromosome;

        /// <summary>
        /// Construct a neural genome.
        /// </summary>
        ///
        /// <param name="network">The network to use.</param>
        public NeuralGenome(BasicNetwork network)
        {
            Organism = network;

            networkChromosome = new Chromosome();

            // create an array of "double genes"
            int size = network.Structure.CalculateSize();
            for (int i = 0; i < size; i++)
            {
                IGene gene = new DoubleGene();
                networkChromosome.Genes.Add(gene);
            }

            Chromosomes.Add(networkChromosome);

            Encode();
        }

        /// <summary>
        /// Decode the genomes into a neural network.
        /// </summary>
        ///
        public override sealed void Decode()
        {
            var net = new double[networkChromosome.Genes.Count];
            for (int i = 0; i < net.Length; i++)
            {
                var gene = (DoubleGene) networkChromosome.Genes[i];
                net[i] = gene.Value;
            }
            NetworkCODEC.ArrayToNetwork(net, (BasicNetwork) Organism);
        }

        /// <summary>
        /// Encode the neural network into genes.
        /// </summary>
        ///
        public override sealed void Encode()
        {
            double[] net = NetworkCODEC
                .NetworkToArray((BasicNetwork) Organism);

            for (int i = 0; i < net.Length; i++)
            {
                ((DoubleGene) networkChromosome.GetGene(i)).Value = net[i];
            }
        }
    }
}