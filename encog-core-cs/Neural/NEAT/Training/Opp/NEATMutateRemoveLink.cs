using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp
{
    /// <summary>
    /// Mutate a genome by removing a random link. Do not remove links from genomes
    /// that have fewer than 5 links.
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
    public class NEATMutateRemoveLink : NEATMutation
    {
        /// <summary>
        /// Do not remove from genomes that have fewer than this number of links.
        /// </summary>
        public const int MIN_LINK = 5;

        /// <inheritdoc/>
        public override void PerformOperation(EncogRandom rnd, IGenome[] parents,
                int parentIndex, IGenome[] offspring,
                int offspringIndex)
        {

            NEATGenome target = ObtainGenome(parents, parentIndex, offspring,
                    offspringIndex);

            if (target.LinksChromosome.Count < NEATMutateRemoveLink.MIN_LINK)
            {
                // don't remove from small genomes
                return;
            }

            // determine the target and remove
            int index = RangeRandomizer.RandomInt(0, target
                    .LinksChromosome.Count - 1);
            NEATLinkGene targetGene = target.LinksChromosome[index];
            target.LinksChromosome.Remove(targetGene);

            // if this orphaned any nodes, then kill them too!
            if (!IsNeuronNeeded(target, targetGene.FromNeuronID))
            {
                RemoveNeuron(target, targetGene.FromNeuronID);
            }

            if (!IsNeuronNeeded(target, targetGene.ToNeuronID))
            {
                RemoveNeuron(target, targetGene.ToNeuronID);
            }
        }
    }
}
