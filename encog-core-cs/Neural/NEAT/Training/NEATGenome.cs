using System;
using System.Collections.Generic;
using Encog.MathUtil.Randomize;
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;
using Encog.Neural.Neat.Training;
using Encog.MathUtil;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT genome. This is a "blueprint" for creating a neural
    /// network.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    ///
    [Serializable]
    public class NEATGenome : BasicGenome, ICloneable
    {
        /// <summary>
        /// The neurons property.
        /// </summary>
        public const String PROPERTY_NEURONS = "neurons";

        /// <summary>
        /// The links property.
        /// </summary>
        public const String PROPERTY_LINKS = "links";

        /// <summary>
        /// The adjustment factor for disjoint genes.
        /// </summary>
        ///
        public const double TWEAK_DISJOINT = 1;

        /// <summary>
        /// The adjustment factor for excess genes.
        /// </summary>
        ///
        public const double TWEAK_EXCESS = 1;

        /// <summary>
        /// The adjustment factor for matched genes.
        /// </summary>
        ///
        public const double TWEAK_MATCHED = 0.4d;

        /// <summary>
        /// The number of inputs.
        /// </summary>
        ///
        private int inputCount;

        /// <summary>
        /// The chromsome that holds the links.
        /// </summary>
        ///
        private Chromosome linksChromosome;

        /// <summary>
        /// THe network depth.
        /// </summary>
        ///
        private int networkDepth;

        /// <summary>
        /// The chromosome that holds the neurons.
        /// </summary>
        ///
        private Chromosome neuronsChromosome;

        /// <summary>
        /// The number of outputs.
        /// </summary>
        ///
        private int outputCount;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long speciesID;

        /// <summary>
        /// Construct a genome by copying another.
        /// </summary>
        ///
        /// <param name="other">The other genome.</param>
        public NEATGenome(NEATGenome other)
        {
            neuronsChromosome = new Chromosome();
            linksChromosome = new Chromosome();
            GA = other.GA;

            Chromosomes.Add(neuronsChromosome);
            Chromosomes.Add(linksChromosome);

            GenomeID = other.GenomeID;
            networkDepth = other.networkDepth;
            Population = other.Population;
            Score = other.Score;
            AdjustedScore = other.AdjustedScore;
            AmountToSpawn = other.AmountToSpawn;
            inputCount = other.inputCount;
            outputCount = other.outputCount;
            speciesID = other.speciesID;


            // copy neurons
            foreach (IGene gene  in  other.Neurons.Genes)
            {
                var oldGene = (NEATNeuronGene) gene;
                var newGene = new NEATNeuronGene(
                    oldGene.NeuronType, oldGene.Id,
                    oldGene.SplitY, oldGene.SplitX,
                    oldGene.Recurrent, oldGene.ActivationResponse);
                Neurons.Add(newGene);
            }


            // copy links
            foreach (IGene gene_0  in  other.Links.Genes)
            {
                var oldGene_1 = (NEATLinkGene) gene_0;
                var newGene_2 = new NEATLinkGene(
                    oldGene_1.FromNeuronID, oldGene_1.ToNeuronID,
                    oldGene_1.Enabled, oldGene_1.InnovationId,
                    oldGene_1.Weight, oldGene_1.Recurrent);
                Links.Add(newGene_2);
            }
        }

        /// <summary>
        /// Create a NEAT gnome.
        /// </summary>
        ///
        /// <param name="genomeID">The genome id.</param>
        /// <param name="neurons">The neurons.</param>
        /// <param name="links">The links.</param>
        /// <param name="inputCount_0">The input count.</param>
        /// <param name="outputCount_1">The output count.</param>
        public NEATGenome(long genomeID, Chromosome neurons,
                          Chromosome links, int inputCount_0, int outputCount_1)
        {
            GenomeID = genomeID;
            linksChromosome = links;
            neuronsChromosome = neurons;
            AmountToSpawn = 0;
            AdjustedScore = 0;
            inputCount = inputCount_0;
            outputCount = outputCount_1;

            Chromosomes.Add(neuronsChromosome);
            Chromosomes.Add(linksChromosome);
        }

        /// <summary>
        /// Construct a genome, do not provide links and neurons.
        /// </summary>
        ///
        /// <param name="id">The genome id.</param>
        /// <param name="inputCount_0">The input count.</param>
        /// <param name="outputCount_1">The output count.</param>
        public NEATGenome(long id, int inputCount_0, int outputCount_1)
        {
            GenomeID = id;
            AdjustedScore = 0;
            inputCount = inputCount_0;
            outputCount = outputCount_1;
            AmountToSpawn = 0;
            speciesID = 0;

            double inputRowSlice = 0.8d/(inputCount_0);
            neuronsChromosome = new Chromosome();
            linksChromosome = new Chromosome();

            Chromosomes.Add(neuronsChromosome);
            Chromosomes.Add(linksChromosome);

            for (int i = 0; i < inputCount_0; i++)
            {
                neuronsChromosome.Add(new NEATNeuronGene(NEATNeuronType.Input,
                                                         i, 0, 0.1d + i*inputRowSlice));
            }

            neuronsChromosome.Add(new NEATNeuronGene(NEATNeuronType.Bias,
                                                     inputCount_0, 0, 0.9d));

            double outputRowSlice = 1/(double) (outputCount_1 + 1);

            for (int i_2 = 0; i_2 < outputCount_1; i_2++)
            {
                neuronsChromosome.Add(new NEATNeuronGene(
                                          NEATNeuronType.Output, i_2 + inputCount_0 + 1, 1, (i_2 + 1)
                                                                                            *outputRowSlice));
            }

            for (int i_3 = 0; i_3 < inputCount_0 + 1; i_3++)
            {
                for (int j = 0; j < outputCount_1; j++)
                {
                    linksChromosome.Add(new NEATLinkGene(
                                            ((NEATNeuronGene) neuronsChromosome.Get(i_3)).Id,
                                            ((NEATNeuronGene) Neurons.Get(
                                                inputCount_0 + j + 1)).Id, true, inputCount_0
                                                                                 + outputCount_1 + 1 + NumGenes,
                                            RangeRandomizer.Randomize(-1, 1), false));
                }
            }
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        public NEATGenome()
        {
        }

        /// <value>the inputCount to set</value>
        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }


        /// <value>THe links chromosome.</value>
        public Chromosome Links
        {
            get { return linksChromosome; }
        }


        /// <value>the networkDepth to set</value>
        public int NetworkDepth
        {
            get { return networkDepth; }
            set { networkDepth = value; }
        }


        /// <value>The neurons chromosome.</value>
        public Chromosome Neurons
        {
            get { return neuronsChromosome; }
        }


        /// <value>The number of genes in the links chromosome.</value>
        public int NumGenes
        {
            get { return linksChromosome.Size(); }
        }


        /// <value>the outputCount to set</value>
        public int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }


        /// <summary>
        /// Set the species id.
        /// </summary>
        ///
        /// <value>The species id.</value>
        public long SpeciesID
        {
            get { return speciesID; }
            set { speciesID = value; }
        }

        /// <value>the linksChromosome to set</value>
        public Chromosome LinksChromosome
        {
            get { return linksChromosome; }
            set { linksChromosome = value; }
        }


        /// <value>the neuronsChromosome to set</value>
        public Chromosome NeuronsChromosome
        {
            get { return neuronsChromosome; }
            set { neuronsChromosome = value; }
        }

        #region ICloneable Members

        /// <summary>
        /// Clone the object. Not currently supported.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Mutate the genome by adding a link to this genome.
        /// </summary>
        ///
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="chanceOfLooped">The chance of a self-connected neuron.</param>
        /// <param name="numTrysToFindLoop">The number of tries to find a loop.</param>
        /// <param name="numTrysToAddLink">The number of tries to add a link.</param>
        internal void AddLink(double mutationRate, double chanceOfLooped,
                              int numTrysToFindLoop, int numTrysToAddLink)
        {
            // should we even add the link
            if (ThreadSafeRandom.NextDouble() > mutationRate)
            {
                return;
            }

            int countTrysToFindLoop = numTrysToFindLoop;
            int countTrysToAddLink = numTrysToFindLoop;

            // the link will be between these two neurons
            long neuron1ID = -1;
            long neuron2ID = -1;

            bool recurrent = false;

            // a self-connected loop?
            if (ThreadSafeRandom.NextDouble() < chanceOfLooped)
            {
                // try to find(randomly) a neuron to add a self-connected link to
                while ((countTrysToFindLoop--) > 0)
                {
                    NEATNeuronGene neuronGene = ChooseRandomNeuron(false);

                    // no self-links on input or bias neurons
                    if (!neuronGene.Recurrent
                        && (neuronGene.NeuronType != NEATNeuronType.Bias)
                        && (neuronGene.NeuronType != NEATNeuronType.Input))
                    {
                        neuron1ID = neuronGene.Id;
                        neuron2ID = neuronGene.Id;

                        neuronGene.Recurrent = true;
                        recurrent = true;

                        countTrysToFindLoop = 0;
                    }
                }
            }
            else
            {
                // try to add a regular link
                while ((countTrysToAddLink--) > 0)
                {
                    NEATNeuronGene neuron1 = ChooseRandomNeuron(true);
                    NEATNeuronGene neuron2 = ChooseRandomNeuron(false);

                    if (!IsDuplicateLink(neuron1ID, neuron2ID)
                        && (neuron1.Id != neuron2.Id)
                        && (neuron2.NeuronType != NEATNeuronType.Bias))
                    {
                        neuron1ID = neuron1.Id;
                        neuron2ID = neuron2.Id;
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
            NEATInnovation innovation = ((NEATTraining) GA).Innovations.CheckInnovation(neuron1ID,
                                                                                                      neuron1ID,
                                                                                                      NEATInnovationType
                                                                                                          .NewLink);

            // see if this is a recurrent(backwards) link
            var neuronGene_0 = (NEATNeuronGene) neuronsChromosome
                                                    .Get(GetElementPos(neuron1ID));
            if (neuronGene_0.SplitY > neuronGene_0.SplitY)
            {
                recurrent = true;
            }

            // is this a new innovation?
            if (innovation == null)
            {
                // new innovation
                ((NEATTraining) GA).Innovations
                    .CreateNewInnovation(neuron1ID, neuron2ID,
                                         NEATInnovationType.NewLink);

                long id2 = GA.Population.AssignInnovationID();

                var linkGene = new NEATLinkGene(neuron1ID,
                                                neuron2ID, true, id2, RangeRandomizer.Randomize(-1, 1),
                                                recurrent);
                linksChromosome.Add(linkGene);
            }
            else
            {
                // existing innovation
                var linkGene_1 = new NEATLinkGene(neuron1ID,
                                                  neuron2ID, true, innovation.InnovationID,
                                                  RangeRandomizer.Randomize(-1, 1), recurrent);
                linksChromosome.Add(linkGene_1);
            }
        }

        /// <summary>
        /// Mutate the genome by adding a neuron.
        /// </summary>
        ///
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="numTrysToFindOldLink">The number of tries to find a link to split.</param>
        internal void AddNeuron(double mutationRate, int numTrysToFindOldLink)
        {
            // should we add a neuron?
            if (ThreadSafeRandom.NextDouble() > mutationRate)
            {
                return;
            }

            int countTrysToFindOldLink = numTrysToFindOldLink;

            // the link to split
            NEATLinkGene splitLink = null;

            int sizeBias = inputCount + outputCount + 10;

            // if there are not at least
            int upperLimit;
            if (linksChromosome.Size() < sizeBias)
            {
                upperLimit = NumGenes - 1 - (int) Math.Sqrt(NumGenes);
            }
            else
            {
                upperLimit = NumGenes - 1;
            }

            while ((countTrysToFindOldLink--) > 0)
            {
                // choose a link, use the square root to prefer the older links
                int i = RangeRandomizer.RandomInt(0, upperLimit);
                var link = (NEATLinkGene) linksChromosome
                                              .Get(i);

                // get the from neuron
                long fromNeuron = link.FromNeuronID;

                if ((link.Enabled)
                    && (!link.Recurrent)
                    && (((NEATNeuronGene) Neurons.Get(
                        GetElementPos(fromNeuron))).NeuronType != NEATNeuronType.Bias))
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

            var fromGene = (NEATNeuronGene) Neurons.Get(
                GetElementPos(from));
            var toGene = (NEATNeuronGene) Neurons.Get(
                GetElementPos(to));

            double newDepth = (fromGene.SplitY + toGene.SplitY)/2;
            double newWidth = (fromGene.SplitX + toGene.SplitX)/2;

            // has this innovation already been tried?
            NEATInnovation innovation = ((NEATTraining) GA).Innovations.CheckInnovation(from, to,
                                                                                                      NEATInnovationType
                                                                                                          .NewNeuron);

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
                long newNeuronID = ((NEATTraining) GA).Innovations.CreateNewInnovation(from, to,
                                                                                                     NEATInnovationType.
                                                                                                         NewNeuron,
                                                                                                     NEATNeuronType.
                                                                                                         Hidden,
                                                                                                     newWidth, newDepth);

                neuronsChromosome.Add(new NEATNeuronGene(
                                          NEATNeuronType.Hidden, newNeuronID, newDepth, newWidth));

                // add the first link
                long link1ID = (GA).Population.AssignInnovationID();

                ((NEATTraining) GA).Innovations
                    .CreateNewInnovation(from, newNeuronID,
                                         NEATInnovationType.NewLink);

                var link1 = new NEATLinkGene(from, newNeuronID,
                                             true, link1ID, 1.0d, false);

                linksChromosome.Add(link1);

                // add the second link
                long link2ID = (GA).Population.AssignInnovationID();

                ((NEATTraining) GA).Innovations
                    .CreateNewInnovation(newNeuronID, to,
                                         NEATInnovationType.NewLink);

                var link2 = new NEATLinkGene(newNeuronID, to, true,
                                             link2ID, originalWeight, false);

                linksChromosome.Add(link2);
            }

            else
            {
                // existing innovation
                long newNeuronID_0 = innovation.NeuronID;

                NEATInnovation innovationLink1 = ((NEATTraining) GA).Innovations.CheckInnovation(from,
                                                                                                               newNeuronID_0,
                                                                                                               NEATInnovationType
                                                                                                                   .
                                                                                                                   NewLink);
                NEATInnovation innovationLink2 =
                    ((NEATTraining) GA).Innovations.CheckInnovation(newNeuronID_0, to,
                                                                                  NEATInnovationType.NewLink);

                if ((innovationLink1 == null) || (innovationLink2 == null))
                {
                    throw new NeuralNetworkError("NEAT Error");
                }

                var link1_1 = new NEATLinkGene(from, newNeuronID_0,
                                               true, innovationLink1.InnovationID, 1.0d, false);
                var link2_2 = new NEATLinkGene(newNeuronID_0, to, true,
                                               innovationLink2.InnovationID, originalWeight, false);

                linksChromosome.Add(link1_1);
                linksChromosome.Add(link2_2);

                var newNeuron = new NEATNeuronGene(
                    NEATNeuronType.Hidden, newNeuronID_0, newDepth, newWidth);

                neuronsChromosome.Add(newNeuron);
            }

            return;
        }

        /// <summary>
        /// Do we already have this neuron id?
        /// </summary>
        ///
        /// <param name="id">The id to check for.</param>
        /// <returns>True if we already have this neuron id.</returns>
        public bool AlreadyHaveThisNeuronID(long id)
        {
            foreach (IGene gene  in  neuronsChromosome.Genes)
            {
                var neuronGene = (NEATNeuronGene) gene;

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
        ///
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
                                                                 .Size() - 1);
            var neuronGene = (NEATNeuronGene) neuronsChromosome
                                                  .Get(neuronPos);
            return neuronGene;
        }

        /// <summary>
        /// Convert the genes to an actual network.
        /// </summary>
        ///
        public override void Decode()
        {
            var pop = (NEATPopulation) Population;

            IList<NEATNeuron> neurons = new List<NEATNeuron>();


            foreach (IGene gene  in  Neurons.Genes)
            {
                var neuronGene = (NEATNeuronGene) gene;
                var neuron = new NEATNeuron(
                    neuronGene.NeuronType, neuronGene.Id,
                    neuronGene.SplitY, neuronGene.SplitX,
                    neuronGene.ActivationResponse);

                neurons.Add(neuron);
            }


            // now to create the links.
            foreach (IGene gene_0  in  Links.Genes)
            {
                var linkGene = (NEATLinkGene) gene_0;
                if (linkGene.Enabled)
                {
                    int element = GetElementPos(linkGene.FromNeuronID);
                    NEATNeuron fromNeuron = neurons[element];

                    element = GetElementPos(linkGene.ToNeuronID);
                    if (element == -1)
                    {
                        Console.Out.WriteLine("test");
                    }
                    NEATNeuron toNeuron = neurons[element];

                    var link = new NEATLink(linkGene.Weight,
                                            fromNeuron, toNeuron, linkGene.Recurrent);

                    fromNeuron.OutputboundLinks.Add(link);
                    toNeuron.InboundLinks.Add(link);
                }
            }

            var network = new NEATNetwork(inputCount, outputCount, neurons,
                                          pop.NeatActivationFunction,
                                          pop.OutputActivationFunction, 0);

            network.Snapshot = pop.Snapshot;
            Organism = network;
        }

        /// <summary>
        /// Convert the network to genes. Not currently supported.
        /// </summary>
        ///
        public override void Encode()
        {
        }

        /// <summary>
        /// Get the compatibility score with another genome. Used to determine
        /// species.
        /// </summary>
        ///
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

            while ((g1 < linksChromosome.Size() - 1)
                   || (g2 < linksChromosome.Size() - 1))
            {
                if (g1 == linksChromosome.Size() - 1)
                {
                    g2++;
                    numExcess++;

                    continue;
                }

                if (g2 == genome.Links.Size() - 1)
                {
                    g1++;
                    numExcess++;

                    continue;
                }

                // get innovation numbers for each gene at this point
                long id1 = ((NEATLinkGene) linksChromosome.Get(g1)).InnovationId;
                long id2 = ((NEATLinkGene) genome.Links.Get(g2)).InnovationId;

                // innovation numbers are identical so increase the matched score
                if (id1 == id2)
                {
                    g1++;
                    g2++;
                    numMatched++;

                    // get the weight difference between these two genes
                    weightDifference += Math.Abs(((NEATLinkGene) linksChromosome.Get(g1)).Weight
                                                 - ((NEATLinkGene) genome.Links.Get(g2)).Weight);
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

            double score = (TWEAK_EXCESS*numExcess/longest)
                           + (TWEAK_DISJOINT*numDisjoint/longest)
                           + (TWEAK_MATCHED*weightDifference/numMatched);

            return score;
        }

        /// <summary>
        /// Get the specified neuron's index.
        /// </summary>
        ///
        /// <param name="neuronID">The neuron id to check for.</param>
        /// <returns>The index.</returns>
        private int GetElementPos(long neuronID)
        {
            for (int i = 0; i < Neurons.Size(); i++)
            {
                var neuronGene = (NEATNeuronGene) neuronsChromosome
                                                      .GetGene(i);
                if (neuronGene.Id == neuronID)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Get the specified split y.
        /// </summary>
        ///
        /// <param name="nd">The neuron.</param>
        /// <returns>The split y.</returns>
        public double GetSplitY(int nd)
        {
            return ((NEATNeuronGene) neuronsChromosome.Get(nd)).SplitY;
        }

        /// <summary>
        /// Determine if this is a duplicate link.
        /// </summary>
        ///
        /// <param name="fromNeuronID">The from neuron id.</param>
        /// <param name="toNeuronID">The to neuron id.</param>
        /// <returns>True if this is a duplicate link.</returns>
        public bool IsDuplicateLink(long fromNeuronID,
                                    long toNeuronID)
        {
            foreach (IGene gene  in  Links.Genes)
            {
                var linkGene = (NEATLinkGene) gene;
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
        ///
        /// <param name="mutateRate">The mutation rate.</param>
        /// <param name="maxPertubation">The maximum to perturb it by.</param>
        public void MutateActivationResponse(double mutateRate,
                                             double maxPertubation)
        {
            foreach (IGene gene  in  neuronsChromosome.Genes)
            {
                if (ThreadSafeRandom.NextDouble() < mutateRate)
                {
                    var neuronGene = (NEATNeuronGene) gene;
                    neuronGene.ActivationResponse = neuronGene.ActivationResponse
                                                    + RangeRandomizer.Randomize(-1, 1)*maxPertubation;
                }
            }
        }

        /// <summary>
        /// Mutate the weights.
        /// </summary>
        ///
        /// <param name="mutateRate">The mutation rate.</param>
        /// <param name="probNewMutate">The probability of a whole new weight.</param>
        /// <param name="maxPertubation">The max perturbation.</param>
        public void MutateWeights(double mutateRate,
                                  double probNewMutate, double maxPertubation)
        {
            foreach (IGene gene  in  linksChromosome.Genes)
            {
                var linkGene = (NEATLinkGene) gene;
                if (ThreadSafeRandom.NextDouble() < mutateRate)
                {
                    if (ThreadSafeRandom.NextDouble() < probNewMutate)
                    {
                        linkGene.Weight = RangeRandomizer.Randomize(-1, 1);
                    }
                    else
                    {
                        linkGene.Weight = linkGene.Weight
                                          + RangeRandomizer.Randomize(-1, 1)*maxPertubation;
                    }
                }
            }
        }

        /// <summary>
        /// Sort the genes.
        /// </summary>
        ///
        public void SortGenes()
        {
            linksChromosome.Genes.Sort();
        }
    }
}