using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.ML.EA.Genome;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.NEAT.Training.Opp
{
    /// <summary>
    /// Mutate a genome by adding a new node. To do this a random link is chosen. The
    /// a neuron is created to split this link. This removes one link and adds two
    /// new links. The weights on the new link are created to minimize changes to the
    /// values produced by the neuron.
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
    public class NEATMutateAddNode : NEATMutation
    {

        /// <inheritdoc/>
        public override void PerformOperation(EncogRandom rnd, IGenome[] parents,
                int parentIndex, IGenome[] offspring,
                int offspringIndex)
        {
            NEATGenome target = ObtainGenome(parents, parentIndex, offspring,
                    offspringIndex);
            int countTrysToFindOldLink = Owner.MaxTries;

            NEATPopulation pop = ((NEATPopulation)target.Population);

            // the link to split
            NEATLinkGene splitLink = null;

            int sizeBias = ((NEATGenome)parents[0]).InputCount
                    + ((NEATGenome)parents[0]).OutputCount + 10;

            // if there are not at least
            int upperLimit;
            if (target.LinksChromosome.Count < sizeBias)
            {
                upperLimit = target.NumGenes - 1
                        - (int)Math.Sqrt(target.NumGenes);
            }
            else
            {
                upperLimit = target.NumGenes - 1;
            }

            while ((countTrysToFindOldLink--) > 0)
            {
                // choose a link, use the square root to prefer the older links
                int i = RangeRandomizer.RandomInt(0, upperLimit);
                NEATLinkGene link = target.LinksChromosome[i];

                // get the from neuron
                long fromNeuron = link.FromNeuronID;

                if ((link.Enabled)
                        && (target.NeuronsChromosome
                                [GetElementPos(target, fromNeuron)]
                                .NeuronType != NEATNeuronType.Bias))
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

            long from = splitLink.FromNeuronID;
            long to = splitLink.ToNeuronID;

            NEATInnovation innovation = ((NEATPopulation)Owner.Population).Innovations
                    .FindInnovationSplit(from, to);

            // add the splitting neuron
            IActivationFunction af = ((NEATPopulation)Owner.Population).ActivationFunctions.Pick(new Random());

            target.NeuronsChromosome.Add(
                    new NEATNeuronGene(NEATNeuronType.Hidden, af, innovation
                            .NeuronID, innovation.InnovationID));

            // add the other two sides of the link
            CreateLink(target, from, innovation.NeuronID,
                    splitLink.Weight);
            CreateLink(target, innovation.NeuronID, to, pop.WeightRange);

            target.SortGenes();
        }
    }
}
