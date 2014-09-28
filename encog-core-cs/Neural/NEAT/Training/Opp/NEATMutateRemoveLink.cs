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
        public const int MinLink = 5;

        /// <inheritdoc/>
        public override void PerformOperation(EncogRandom rnd, IGenome[] parents,
                int parentIndex, IGenome[] offspring,
                int offspringIndex)
        {

            var target = ObtainGenome(parents, parentIndex, offspring,
                    offspringIndex);

            if (target.LinksChromosome.Count < MinLink)
            {
                // don't remove from small genomes
                return;
            }

            // determine the target and remove
            var index = RangeRandomizer.RandomInt(0, target
                    .LinksChromosome.Count - 1);
            NEATLinkGene targetGene = target.LinksChromosome[index];
            target.LinksChromosome.Remove(targetGene);

            // if this orphaned any nodes, then kill them too!
            if (!IsNeuronNeeded(target, targetGene.FromNeuronId))
            {
                RemoveNeuron(target, targetGene.FromNeuronId);
            }

            if (!IsNeuronNeeded(target, targetGene.ToNeuronId))
            {
                RemoveNeuron(target, targetGene.ToNeuronId);
            }
        }
    }
}
