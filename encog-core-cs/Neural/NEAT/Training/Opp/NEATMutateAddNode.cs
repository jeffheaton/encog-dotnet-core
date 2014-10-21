//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
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
            var target = ObtainGenome(parents, parentIndex, offspring,
                    offspringIndex);
            var countTrysToFindOldLink = Owner.MaxTries;

            var pop = ((NEATPopulation)target.Population);

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
                long fromNeuron = link.FromNeuronId;

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

            long from = splitLink.FromNeuronId;
            long to = splitLink.ToNeuronId;

            NEATInnovation innovation = ((NEATPopulation)Owner.Population).Innovations
                    .FindInnovationSplit(from, to);

            // add the splitting neuron
            IActivationFunction af = ((NEATPopulation)Owner.Population).ActivationFunctions.Pick(new Random());

            target.NeuronsChromosome.Add(
                    new NEATNeuronGene(NEATNeuronType.Hidden, af, innovation
                            .NeuronId, innovation.InnovationId));

            // add the other two sides of the link
            CreateLink(target, from, innovation.NeuronId,
                    splitLink.Weight);
            CreateLink(target, innovation.NeuronId, to, pop.WeightRange);

            target.SortGenes();
        }
    }
}
