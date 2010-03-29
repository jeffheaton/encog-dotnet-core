using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Genes;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Synapse.NEAT;
using Encog.Neural.Networks.Layers;
using Encog.MathUtil;
using Encog.Neural.Activation;

namespace Encog.Neural.Networks.Training.NEAT
{
    /// <summary>
    /// Implements a NEAT genome.  This is a "blueprint" for creating a neural network.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public class NEATGenome : BasicGenome
    {
        /// <summary>
        /// The adjustment factor for disjoint genes.
        /// </summary>
        public static double TWEAK_DISJOINT = 1;

        /// <summary>
        /// The adjustment factor for excess genes.
        /// </summary>
        public const double TWEAK_EXCESS = 1;

        /// <summary>
        /// The adjustment factor for matched genes.
        /// </summary>
        public const double TWEAK_MATCHED = 0.4;

        /// <summary>
        /// The number of inputs.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The chromsome that holds the links.
        /// </summary>
        private Chromosome linksChromosome;

        /// <summary>
        /// The network depth.
        /// </summary>
        private int networkDepth;

        /// <summary>
        /// The chromosome that holds the neurons.
        /// </summary>
        private Chromosome neuronsChromosome;

        /// <summary>
        /// The number of outputs.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The species id.
        /// </summary>
        private long speciesID;

        /// <summary>
        /// The owner object.
        /// </summary>
        private NEATTraining training;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public NEATGenome():base(null)
        {
        }

        /// <summary>
        /// Construct a genome by copying another. 
        /// </summary>
        /// <param name="other">The other genome.</param>
        public NEATGenome(NEATGenome other)
            : base(other.training)
        {

            neuronsChromosome = new Chromosome();
            linksChromosome = new Chromosome();

            GenomeID = other.GenomeID;
            networkDepth = other.networkDepth;
            Score = other.Score;
            AdjustedScore = other.AdjustedScore;
            AmountToSpawn = other.AmountToSpawn;
            inputCount = other.inputCount;
            outputCount = other.outputCount;
            speciesID = other.speciesID;
            training = other.training;

            // copy neurons
            foreach (IGene gene in other.Neurons.Genes)
            {
                NEATNeuronGene oldGene = (NEATNeuronGene)gene;
                NEATNeuronGene newGene = new NEATNeuronGene(oldGene.NeuronType
                   , oldGene.Id, oldGene.SplitX,
                       oldGene.SplitY, oldGene.Recurrent, oldGene
                               .ActivationResponse);
                this.neuronsChromosome.Genes.Add(newGene);
            }

            // copy links
            foreach (IGene gene in other.Links.Genes)
            {
                NEATLinkGene oldGene = (NEATLinkGene)gene;
                NEATLinkGene newGene = new NEATLinkGene(oldGene
                        .FromNeuronID, oldGene.ToNeuronID, oldGene
                        .Enabled, oldGene.InnovationId, oldGene
                        .Weight, oldGene.IsRecurrent);
                Links.Genes.Add(newGene);
            }

        }

        /// <summary>
        /// Create a NEAT gnome. 
        /// </summary>
        /// <param name="training">The owner object.</param>
        /// <param name="genomeID">The genome id.</param>
        /// <param name="neurons">The neurons.</param>
        /// <param name="links">The links.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        public NEATGenome(NEATTraining training, long genomeID,
                Chromosome neurons, Chromosome links,
                int inputCount, int outputCount)
            : base(training)
        {
            GenomeID = genomeID;
            linksChromosome = links;
            neuronsChromosome = neurons;
            AmountToSpawn = 0;
            AdjustedScore = 0;
            this.inputCount = inputCount;
            this.outputCount = outputCount;
            this.training = training;
        }

    
        /// <summary>
        /// Construct a genome, do not provide links and neurons. 
        /// </summary>
        /// <param name="training">The owner object.</param>
        /// <param name="id">The genome id.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        public NEATGenome(NEATTraining training, long id,
                int inputCount, int outputCount)
            : base(training)
        {

            GenomeID = id;
            AdjustedScore = 0;
            this.inputCount = inputCount;
            this.outputCount = outputCount;
            AmountToSpawn = 0;
            speciesID = 0;
            this.training = training;

            double inputRowSlice = 0.8 / (inputCount);
            neuronsChromosome = new Chromosome();
            linksChromosome = new Chromosome();

            for (int i = 0; i < inputCount; i++)
            {
                neuronsChromosome.Genes.Add(new NEATNeuronGene(NEATNeuronType.Input, i,
                        0, 0.1 + i * inputRowSlice));
            }

            neuronsChromosome.Genes.Add(new NEATNeuronGene(NEATNeuronType.Bias,
                    inputCount, 0, 0.9));

            double outputRowSlice = 1 / (double)(outputCount + 1);

            for (int i = 0; i < outputCount; i++)
            {
                neuronsChromosome.Genes.Add(new NEATNeuronGene(NEATNeuronType.Output, i
                        + inputCount + 1, 1, (i + 1) * outputRowSlice));
            }

            for (int i = 0; i < inputCount + 1; i++)
            {
                for (int j = 0; j < outputCount; j++)
                {
                    linksChromosome.Genes.Add(new NEATLinkGene(
                            ((NEATNeuronGene)neuronsChromosome.Genes[i]).Id,
                            ((NEATNeuronGene)Neurons.Genes[inputCount + j + 1])
                                    .Id, true, inputCount + outputCount + 1
                                    + NumGenes, RangeRandomizer.Randomize(-1,
                                    1), false));
                }
            }

        }

        /// <summary>
        /// Mutate the genome by adding a link to this genome. 
        /// </summary>
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="chanceOfLooped">The chance of a self-connected neuron.</param>
        /// <param name="numTrysToFindLoop">The number of tries to find a loop.</param>
        /// <param name="numTrysToAddLink">The number of tries to add a link.</param>
        public void AddLink(double mutationRate, double chanceOfLooped,
                int numTrysToFindLoop, int numTrysToAddLink)
        {

            // should we even add the link
            if (ThreadSafeRandom.NextDouble() > mutationRate)
            {
                return;
            }

            // the link will be between these two neurons
            long neuron1ID = -1;
            long neuron2ID = -1;

            bool recurrent = false;

            // a self-connected loop?
            if (ThreadSafeRandom.NextDouble() < chanceOfLooped)
            {

                // try to find(randomly) a neuron to add a self-connected link to
                while ((numTrysToFindLoop--) > 0)
                {
                    NEATNeuronGene neuronGene = ChooseRandomNeuron(false);

                    // no self-links on input or bias neurons
                    if (!neuronGene.Recurrent
                            && (neuronGene.NeuronType != NEATNeuronType.Bias)
                            && (neuronGene.NeuronType != NEATNeuronType.Input))
                    {
                        neuron1ID = neuron2ID = neuronGene.Id;

                        neuronGene.Recurrent = true;
                        recurrent = true;

                        numTrysToFindLoop = 0;
                    }
                }
            }
            else
            {
                // try to add a regular link
                while ((numTrysToAddLink--) > 0)
                {
                    NEATNeuronGene neuron1 = ChooseRandomNeuron(true);
                    NEATNeuronGene neuron2 = ChooseRandomNeuron(false);

                    if (!IsDuplicateLink(neuron1ID, neuron2ID)
                            && (neuron1.Id != neuron2.Id)
                            && (neuron2.NeuronType != NEATNeuronType.Bias))
                    {

                        neuron1ID = -1;
                        neuron2ID = -1;
                        break;
                    }
                }
            }

            // did we fail to find a link
            if ((neuron1ID < 0) || (neuron2ID < 0))
            {
                return;
            }

            // check to see if this innovation has already been tried
            NEATInnovation innovation = training.Innovations
                    .CheckInnovation(neuron1ID, neuron1ID,
                            NEATInnovationType.NewLink);

            // see if this is a recurrent(backwards) link
            NEATNeuronGene neuronGene2 = (NEATNeuronGene)neuronsChromosome.Genes
                    [GetElementPos(neuron1ID)];
            if (neuronGene2.SplitY > neuronGene2.SplitY)
            {
                recurrent = true;
            }

            // is this a new innovation?
            if (innovation == null)
            {
                // new innovation
                training.Innovations.CreateNewInnovation(neuron1ID, neuron2ID,
                        NEATInnovationType.NewLink);

                long id2 = training.Population.AssignInnovationID();

                NEATLinkGene linkGene = new NEATLinkGene(neuron1ID,
                        neuron2ID, true, id2, RangeRandomizer.Randomize(-1, 1),
                        recurrent);
                linksChromosome.Genes.Add(linkGene);
            }
            else
            {
                // existing innovation
                NEATLinkGene linkGene = new NEATLinkGene(neuron1ID,
                        neuron2ID, true, innovation.InnovationID,
                        RangeRandomizer.Randomize(-1, 1), recurrent);
                linksChromosome.Genes.Add(linkGene);
            }
        }

        /// <summary>
        /// Mutate the genome by adding a neuron. 
        /// </summary>
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="numTrysToFindOldLink">The number of tries to find a link to split.</param>
        public void AddNeuron(double mutationRate, int numTrysToFindOldLink)
        {

            // should we add a neuron?
            if (ThreadSafeRandom.NextDouble() > mutationRate)
            {
                return;
            }

            // the link to split
            NEATLinkGene splitLink = null;

            int sizeThreshold = inputCount + outputCount + 10;

            // if there are not at least
            int upperLimit;
            if (linksChromosome.Genes.Count < sizeThreshold)
            {
                upperLimit = NumGenes - 1 - (int)Math.Sqrt(NumGenes);
            }
            else
            {
                upperLimit = NumGenes - 1;
            }

            while ((numTrysToFindOldLink--) > 0)
            {
                // choose a link, use the square root to prefer the older links
                int i = RangeRandomizer.RandomInt(0, upperLimit);
                NEATLinkGene link = (NEATLinkGene)linksChromosome.Genes[i];

                // get the from neuron
                long fromNeuron = link.FromNeuronID;

                if ((link.Enabled)
                        && (!link.IsRecurrent)
                        && (((NEATNeuronGene)Neurons.Genes[
                                GetElementPos(fromNeuron)]).NeuronType != NEATNeuronType.Bias))
                {
                    splitLink = link;
                    break;
                }
            }

            if (splitLink == null)
            {
                return;
            }

            splitLink.Enabled = false;

            double originalWeight = splitLink.Weight;

            long from = splitLink.FromNeuronID;
            long to = splitLink.ToNeuronID;

            NEATNeuronGene fromGene = (NEATNeuronGene)Neurons.Genes[
                    GetElementPos(from)];
            NEATNeuronGene toGene = (NEATNeuronGene)Neurons.Genes[
                    GetElementPos(to)];

            double newDepth = (fromGene.SplitY + toGene.SplitY) / 2;
            double newWidth = (fromGene.SplitX + toGene.SplitX) / 2;

            // has this innovation already been tried?
            NEATInnovation innovation = training.Innovations.CheckInnovation(
                    from, to, NEATInnovationType.NewNeuron);

            // prevent chaining
            if (innovation != null)
            {
                long neuronID = innovation.NeuronID;

                if (AlreadyHaveThisNeuronID(neuronID))
                {
                    innovation = null;
                }
            }

            if (innovation == null)
            {
                // this innovation has not been tried, create it
                long newNeuronID = training.Innovations
                        .CreateNewInnovation(from, to,
                                NEATInnovationType.NewNeuron,
                                NEATNeuronType.Hidden, newWidth, newDepth);

                neuronsChromosome.Genes.Add(new NEATNeuronGene(NEATNeuronType.Hidden,
                        newNeuronID, newDepth, newWidth));

                // add the first link
                long link1ID = training.Population.AssignInnovationID();

                training.Innovations.CreateNewInnovation(from, newNeuronID,
                        NEATInnovationType.NewLink);

                NEATLinkGene link1 = new NEATLinkGene(from, newNeuronID,
                        true, link1ID, 1.0, false);

                linksChromosome.Genes.Add(link1);

                // add the second link
                long link2ID = training.Population.AssignInnovationID();

                training.Innovations.CreateNewInnovation(newNeuronID, to,
                        NEATInnovationType.NewLink);

                NEATLinkGene link2 = new NEATLinkGene(newNeuronID, to, true,
                        link2ID, originalWeight, false);

                linksChromosome.Genes.Add(link2);
            }

            else
            {
                // existing innovation
                long newNeuronID = innovation.NeuronID;

                NEATInnovation innovationLink1 = training.Innovations
                        .CheckInnovation(from, newNeuronID,
                                NEATInnovationType.NewLink);
                NEATInnovation innovationLink2 = training.Innovations
                        .CheckInnovation(newNeuronID, to,
                                NEATInnovationType.NewLink);

                if ((innovationLink1 == null) || (innovationLink2 == null))
                {
                    throw new NeuralNetworkError("NEAT Error");
                }

                NEATLinkGene link1 = new NEATLinkGene(from, newNeuronID,
                        true, innovationLink1.InnovationID, 1.0, false);
                NEATLinkGene link2 = new NEATLinkGene(newNeuronID, to, true,
                        innovationLink2.InnovationID, originalWeight, false);

                linksChromosome.Genes.Add(link1);
                linksChromosome.Genes.Add(link2);

                NEATNeuronGene newNeuron = new NEATNeuronGene(
                        NEATNeuronType.Hidden, newNeuronID, newDepth, newWidth);

                neuronsChromosome.Genes.Add(newNeuron);
            }

            return;
        }

        /// <summary>
        /// Do we already have this neuron id? 
        /// </summary>
        /// <param name="id">The id to check for.</param>
        /// <returns>True if we already have this neuron id.</returns>
        public bool AlreadyHaveThisNeuronID(long id)
        {
            foreach (IGene gene in neuronsChromosome.Genes)
            {

                NEATNeuronGene neuronGene = (NEATNeuronGene)gene;

                if (neuronGene.Id == id)
                {
                    return true;
                }
            }

            return false;
        }

        
        /// <summary>
        /// Choose a random neuron. 
        /// </summary>
        /// <param name="includeInput">Should the input neurons be included.</param>
        /// <returns>The random neuron.</returns>
        private NEATNeuronGene ChooseRandomNeuron(bool includeInput)
        {
            int start;

            if (includeInput)
            {
                start = 0;
            }
            else
            {
                start = inputCount + 1;
            }

            int neuronPos = RangeRandomizer.RandomInt(start, Neurons
                    .Genes.Count - 1);
            NEATNeuronGene neuronGene = (NEATNeuronGene)neuronsChromosome.Genes[neuronPos];
            return neuronGene;

        }

        /// <summary>
        /// Convert the genes to an actual network.
        /// </summary>
        public override void Decode()
        {
            IList<NEATNeuron> neurons = new List<NEATNeuron>();

            foreach (IGene gene in Neurons.Genes)
            {
                NEATNeuronGene neuronGene = (NEATNeuronGene)gene;
                NEATNeuron neuron = new NEATNeuron(
                        neuronGene.NeuronType, neuronGene.Id, neuronGene
                                .SplitY, neuronGene.SplitX, neuronGene
                                .ActivationResponse);

                neurons.Add(neuron);
            }

            // now to create the links.
            foreach (IGene gene in Links.Genes)
            {
                NEATLinkGene linkGene = (NEATLinkGene)gene;
                if (linkGene.Enabled)
                {
                    int element = GetElementPos(linkGene.FromNeuronID);
                    NEATNeuron fromNeuron = neurons[element];

                    element = GetElementPos(linkGene.ToNeuronID);
                    NEATNeuron toNeuron = neurons[element];

                    NEATLink link = new NEATLink(linkGene.Weight,
                            fromNeuron, toNeuron, linkGene.IsRecurrent);

                    fromNeuron.OutputboundLinks.Add(link);
                    toNeuron.InboundLinks.Add(link);

                }
            }

            BasicLayer inputLayer = new BasicLayer(new ActivationLinear(),
                    false, inputCount);
            BasicLayer outputLayer = new BasicLayer(training
                    .OutputActivationFunction, false, outputCount);
            NEATSynapse synapse = new NEATSynapse(inputLayer, outputLayer,
                    neurons, training.NeatActivationFunction, networkDepth);
            synapse.Snapshot = this.training.Snapshot;
            inputLayer.AddSynapse(synapse);
            BasicNetwork network = new BasicNetwork();
            network.TagLayer(BasicNetwork.TAG_INPUT, inputLayer);
            network.TagLayer(BasicNetwork.TAG_OUTPUT, outputLayer);
            network.Structure.FinalizeStructure();
            Organism = network;

        }

        /// <summary>
        /// Convert the network to genes.  Not currently supported.
        /// </summary>
        public override void Encode()
        {

        }


        /// <summary>
        /// Get the compatibility score with another genome.  Used to determine species. 
        /// </summary>
        /// <param name="genome">The other genome.</param>
        /// <returns>The score.</returns>
        public double GetCompatibilityScore(NEATGenome genome)
        {
            double numDisjoint = 0;
            double numExcess = 0;
            double numMatched = 0;
            double weightDifference = 0;

            int g1 = 0;
            int g2 = 0;

            while ((g1 < linksChromosome.Genes.Count - 1)
                    || (g2 < linksChromosome.Genes.Count - 1))
            {

                if (g1 == linksChromosome.Genes.Count - 1)
                {
                    g2++;
                    numExcess++;

                    continue;
                }

                if (g2 == genome.Links.Genes.Count - 1)
                {
                    g1++;
                    numExcess++;

                    continue;
                }

                // get innovation numbers for each gene at this point
                long id1 = ((NEATLinkGene)linksChromosome.Genes[g1])
                        .InnovationId;
                long id2 = ((NEATLinkGene)genome.Links.Genes[g2])
                        .InnovationId;

                // innovation numbers are identical so increase the matched score
                if (id1 == id2)
                {
                    g1++;
                    g2++;
                    numMatched++;

                    // get the weight difference between these two genes
                    weightDifference += Math.Abs(((NEATLinkGene)linksChromosome.Genes[g1]).Weight
                            - ((NEATLinkGene)genome.Links.Genes[g2])
                                    .Weight);
                }

                // innovation numbers are different so increment the disjoint score
                if (id1 < id2)
                {
                    numDisjoint++;
                    g1++;
                }

                if (id1 > id2)
                {
                    ++numDisjoint;
                    ++g2;
                }

            }

            int longest = genome.NumGenes;

            if (NumGenes > longest)
            {
                longest = NumGenes;
            }

            double score = (NEATGenome.TWEAK_EXCESS * numExcess / longest)
                    + (NEATGenome.TWEAK_DISJOINT * numDisjoint / longest)
                    + (NEATGenome.TWEAK_MATCHED * weightDifference / numMatched);

            return score;
        }

       
        /// <summary>
        /// Get the specified neuron's index. 
        /// </summary>
        /// <param name="neuronID">The neuron id to check for.</param>
        /// <returns>The index.</returns>
        private int GetElementPos(long neuronID)
        {

            for (int i = 0; i < Neurons.Genes.Count; i++)
            {
                NEATNeuronGene neuronGene = (NEATNeuronGene)neuronsChromosome.Genes[i];
                if (neuronGene.Id == neuronID)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputCount
        {
            get
            {
            return inputCount;
            }
        }

        /// <summary>
        /// THe links chromosome.
        /// </summary>
        public Chromosome Links
        {
            get
            {
            return linksChromosome;
            }
        }

        /**
         * @return The network depth.
         */
        public int NetworkDepth
        {
            get
            {
            return networkDepth;
            }
            set
            {
                this.networkDepth = value;
            }
        }

        /// <summary>
        /// The neurons chromosome.
        /// </summary>
        public Chromosome Neurons
        {
            get
            {
            return neuronsChromosome;
            }
        }

        /// <summary>
        /// The number of genes in the links chromosome.
        /// </summary>
        public int NumGenes
        {
            get
            {
                return linksChromosome.Genes.Count;
            }
        }

        /// <summary>
        /// The output count.
        /// </summary>
        public int OutputCount
        {
            get
            {
                return outputCount;
            }
        }

        /// <summary>
        /// The species ID.
        /// </summary>
        public long SpeciesID
        {
            get
            {
                return speciesID;
            }
            set
            {
                this.speciesID = value;
            }
        }

      
        /// <summary>
        /// Get the specified split y. 
        /// </summary>
        /// <param name="nd">The neuron.</param>
        /// <returns>The split y</returns>
        public double GetSplitY(int nd)
        {
            return ((NEATNeuronGene)neuronsChromosome.Genes[nd]).SplitY;
        }

       
        /// <summary>
        /// Determine if this is a duplicate link. 
        /// </summary>
        /// <param name="fromNeuronID">The from neuron id.</param>
        /// <param name="toNeuronID">The to neuron id.</param>
        /// <returns>True if this is a duplicate link.</returns>
        public bool IsDuplicateLink(long fromNeuronID, long toNeuronID)
        {
            foreach (IGene gene in this.Links.Genes)
            {
                NEATLinkGene linkGene = (NEATLinkGene)gene;
                if ((linkGene.FromNeuronID == fromNeuronID)
                        && (linkGene.ToNeuronID == toNeuronID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Mutate the activation response. 
        /// </summary>
        /// <param name="mutateRate">The mutation rate.</param>
        /// <param name="maxPertubation">The maximum to perturb it by.</param>
        public void MutateActivationResponse(double mutateRate,
                double maxPertubation)
        {
            foreach (IGene gene in neuronsChromosome.Genes)
            {
                if (ThreadSafeRandom.NextDouble() < mutateRate)
                {
                    NEATNeuronGene neuronGene = (NEATNeuronGene)gene;
                    neuronGene.ActivationResponse = neuronGene
                            .ActivationResponse
                            + RangeRandomizer.Randomize(-1, 1) * maxPertubation;
                }
            }
        }

        /// <summary>
        /// Mutate the weights. 
        /// </summary>
        /// <param name="mutateRate">The mutation rate.</param>
        /// <param name="probNewMutate">The probability of a whole new weight.</param>
        /// <param name="maxPertubation">The max perturbation.</param>
        public void MutateWeights(double mutateRate, double probNewMutate,
                double maxPertubation)
        {
            foreach (IGene gene in linksChromosome.Genes)
            {
                NEATLinkGene linkGene = (NEATLinkGene)gene;
                if (ThreadSafeRandom.NextDouble() < mutateRate)
                {
                    if (ThreadSafeRandom.NextDouble() < probNewMutate)
                    {
                        linkGene.Weight = RangeRandomizer.Randomize(-1, 1);
                    }
                    else
                    {
                        linkGene.Weight += RangeRandomizer.Randomize(-1, 1)
                                        * maxPertubation;
                    }
                }
            }
        }

        /// <summary>
        /// Sort the genes.
        /// </summary>
        public void SortGenes()
        {
            linksChromosome.Genes.Sort();
        }

        /// <summary>
        /// This genome as a string.
        /// </summary>
        /// <returns>This genome as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NEATGenome:");
            result.Append(GenomeID);
            result.Append(",fitness=");
            result.Append(Score);
            result.Append(")");
            return result.ToString();
        }
    }
}
