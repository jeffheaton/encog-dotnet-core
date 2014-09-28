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
using Encog.MathUtil.Randomize;
using Encog.ML.EA.Genome;

namespace Encog.Neural.NEAT.Training.Opp
{
    /// <summary>
    /// Mutates a NEAT genome by adding a link. To add a link, two random neurons are
    /// chosen and a new random link is created between them. There are severals
    /// rules. Bias and input neurons can never be the target of a link. We also do
    /// not create double links. Two neurons cannot have more than one link going in
    /// the same direction. A neuron can have a single link to itself.
    /// 
    /// If the network is only one cycle, then output neurons cannot be a target.
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
    public class NEATMutateAddLink : NEATMutation
    {
        /// <inheritdoc/>
        public override void PerformOperation(EncogRandom rnd, IGenome[] parents, int parentIndex,
                IGenome[] offspring, int offspringIndex)
        {
            int countTrysToAddLink = Owner.MaxTries;

            NEATGenome target = ObtainGenome(parents, parentIndex, offspring,
                    offspringIndex);

            // the link will be between these two neurons
            long neuron1Id = -1;
            long neuron2Id = -1;

            // try to add a link
            while ((countTrysToAddLink--) > 0)
            {
                NEATNeuronGene neuron1 = ChooseRandomNeuron(target, true);
                NEATNeuronGene neuron2 = ChooseRandomNeuron(target, false);

                if (neuron1 == null || neuron2 == null)
                {
                    return;
                }

                // do not duplicate
                // do not go to a bias neuron
                // do not go from an output neuron
                // do not go to an input neuron
                if (!IsDuplicateLink(target, neuron1.Id, neuron2.Id)
                        && (neuron2.NeuronType != NEATNeuronType.Bias)
                        && (neuron2.NeuronType != NEATNeuronType.Input))
                {

                    if (((NEATPopulation)Owner.Population).ActivationCycles != 1
                            || neuron1.NeuronType != NEATNeuronType.Output)
                    {
                        neuron1Id = neuron1.Id;
                        neuron2Id = neuron2.Id;
                        break;
                    }
                }
            }

            // did we fail to find a link
            if ((neuron1Id < 0) || (neuron2Id < 0))
            {
                return;
            }

            double r = ((NEATPopulation)target.Population).WeightRange;
            CreateLink(target, neuron1Id, neuron2Id,
                    RangeRandomizer.Randomize(rnd, -r, r));
            target.SortGenes();
        }
    }
}
