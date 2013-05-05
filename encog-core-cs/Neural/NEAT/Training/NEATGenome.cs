using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.Util;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT genome. This is a "blueprint" for creating a neural
    /// network.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/ Encog's NEAT implementation was drawn from
    /// the following three Journal Articles. For more complete BibTeX sources, see
    /// NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    /// </summary>
    [Serializable]
    public class NEATGenome : BasicGenome, ICloneable
    {
        /// <summary>
        /// The number of inputs.
        /// </summary>
        public int InputCount { get; set; }

        /// <summary>
        /// The list that holds the links.
        /// </summary>
        private List<NEATLinkGene> linksList = new List<NEATLinkGene>();

        /// <summary>
        /// The network depth.
        /// </summary>
        public int NetworkDepth { get; set; }

        /// <summary>
        /// The list that holds the neurons.
        /// </summary>
        private IList<NEATNeuronGene> neuronsList = new List<NEATNeuronGene>();

        /// <summary>
        /// The number of outputs.
        /// </summary>
        public int OutputCount { get; set; }

        /// <summary>
        ///  Construct a genome by copying another. 
        /// </summary>
        /// <param name="other">The other genome.</param>
        public NEATGenome(NEATGenome other)
        {
            NetworkDepth = other.NetworkDepth;
            Population = other.Population;
            Score = other.Score;
            AdjustedScore = other.AdjustedScore;
            InputCount = other.InputCount;
            OutputCount = other.OutputCount;
            this.Species = other.Species;

            // copy neurons
            foreach (NEATNeuronGene oldGene in other.NeuronsChromosome)
            {
                NEATNeuronGene newGene = new NEATNeuronGene(oldGene);
                this.neuronsList.Add(newGene);
            }

            // copy links
            foreach (NEATLinkGene oldGene in other.LinksChromosome)
            {
                NEATLinkGene newGene = new NEATLinkGene(
                        oldGene.FromNeuronID, oldGene.ToNeuronID,
                        oldGene.Enabled, oldGene.InnovationId,
                        oldGene.Weight);
                this.linksList.Add(newGene);
            }

        }

        /// <summary>
        /// Create a NEAT gnome. Neuron genes will be added by reference, links will
        /// be copied.
        /// </summary>
        /// <param name="neurons">The neurons to create.</param>
        /// <param name="links">The links to create.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        public NEATGenome(IList<NEATNeuronGene> neurons,
                IList<NEATLinkGene> links, int inputCount,
                int outputCount)
        {
            AdjustedScore = 0;
            InputCount = inputCount;
            OutputCount = outputCount;

            foreach (NEATLinkGene gene in links)
            {
                this.linksList.Add(new NEATLinkGene(gene));
            }

            this.neuronsList = this.neuronsList.Union(neurons).ToList();
        }

        /// <summary>
        /// Create a new genome with the specified connection density. This
        /// constructor is typically used to create the initial population.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="pop">The population.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount"><The output count./param>
        /// <param name="connectionDensity">The connection density.</param>
        public NEATGenome(EncogRandom rnd, NEATPopulation pop,
                int inputCount, int outputCount,
                double connectionDensity)
        {
            AdjustedScore = 0;
            InputCount = inputCount;
            OutputCount = outputCount;

            // get the activation function
            IActivationFunction af = pop.ActivationFunctions.PickFirst();

            // first bias
            int innovationID = 0;
            NEATNeuronGene biasGene = new NEATNeuronGene(NEATNeuronType.Bias, af,
                    inputCount, innovationID++);
            this.neuronsList.Add(biasGene);

            // then inputs

            for (int i = 0; i < inputCount; i++)
            {
                NEATNeuronGene gene = new NEATNeuronGene(NEATNeuronType.Input, af,
                        i, innovationID++);
                this.neuronsList.Add(gene);
            }

            // then outputs

            for (int i = 0; i < outputCount; i++)
            {
                NEATNeuronGene gene = new NEATNeuronGene(NEATNeuronType.Output, af,
                        i + inputCount + 1, innovationID++);
                this.neuronsList.Add(gene);
            }

            // and now links
            for (int i = 0; i < inputCount + 1; i++)
            {
                for (int j = 0; j < outputCount; j++)
                {
                    // make sure we have at least one connection
                    if (this.linksList.Count < 1
                            || rnd.NextDouble() < connectionDensity)
                    {
                        long fromID = this.neuronsList[i].Id;
                        long toID = this.neuronsList[inputCount + j + 1].Id;
                        double w = RangeRandomizer.Randomize(rnd, -pop.WeightRange, pop.WeightRange);
                        NEATLinkGene gene = new NEATLinkGene(fromID, toID, true,
                                innovationID++, w);
                        this.linksList.Add(gene);
                    }
                }
            }

        }

        /// <summary>
        /// Empty constructor for persistence.
        /// </summary>
        public NEATGenome()
        {

        }

        /// <summary>
        /// The number of genes in the links chromosome.
        /// </summary>
        public int NumGenes
        {
            get
            {
                return this.linksList.Count;
            }
        }

        /// <summary>
        /// Sort the genes.
        /// </summary>
        public void SortGenes()
        {
            this.linksList.Sort();
        }

        /// <summary>
        /// The link neurons.
        /// </summary>
        public IList<NEATLinkGene> LinksChromosome
        {
            get
            {
                return this.linksList;
            }
        }

        /// <summary>
        /// The neurons
        /// </summary>
        public IList<NEATNeuronGene> NeuronsChromosome
        {
            get
            {
                return this.neuronsList;
            }
        }

        /// <summary>
        /// Validate the structure of this genome.
        /// </summary>
        public void Validate()
        {

            // make sure that the bias neuron is where it should be
            NEATNeuronGene g = this.neuronsList[0];
            if (g.NeuronType != NEATNeuronType.Bias)
            {
                throw new EncogError("NEAT Neuron Gene 0 should be a bias gene.");
            }

            // make sure all input neurons are at the beginning
            for (int i = 1; i <= InputCount; i++)
            {
                NEATNeuronGene gene = this.neuronsList[i];
                if (gene.NeuronType != NEATNeuronType.Input)
                {
                    throw new EncogError("NEAT Neuron Gene " + i
                            + " should be an input gene.");
                }
            }

            // make sure that there are no double links
            IDictionary<String, NEATLinkGene> map = new Dictionary<String, NEATLinkGene>();
            foreach (NEATLinkGene nlg in this.linksList)
            {
                String key = nlg.FromNeuronID + "->" + nlg.ToNeuronID;
                if (map.ContainsKey(key))
                {
                    throw new EncogError("Double link found: " + key);
                }
                map[key] = nlg;
            }
        }

        /// <inheritdoc/>
        public override void Copy(IGenome source)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Find the neuron with the specified nodeID.
        /// </summary>
        /// <param name="nodeID">The nodeID to look for.</param>
        /// <returns>The neuron, if found, otherwise null.</returns>
        public NEATNeuronGene FindNeuron(long nodeID)
        {
            foreach (NEATNeuronGene gene in this.neuronsList)
            {
                if (gene.Id == nodeID)
                    return gene;
            }
            return null;
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(this.GetType().Name);
            result.Append(",score=");
            result.Append(Format.FormatDouble(Score, 2));
            result.Append(",adjusted score=");
            result.Append(Format.FormatDouble(AdjustedScore, 2));
            result.Append(",birth generation=");
            result.Append(this.BirthGeneration);
            result.Append(",neurons=");
            result.Append(this.neuronsList.Count);
            result.Append(",links=");
            result.Append(this.linksList.Count);
            result.Append("]");
            return result.ToString();
        }

        /// <inheritdoc/>
        public object Clone()
        {
            NEATGenome result = new NEATGenome();
            result.Copy(this);
            return result;
        }
    }
}
