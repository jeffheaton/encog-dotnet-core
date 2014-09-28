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
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;

namespace Encog.Neural.NEAT.Training.Species
{
    /// <summary>
    /// The original NEAT Speciation Strategy. This is currently the only speciation
    /// strategy implemented by Encog. There are other speciation strategies that
    /// have been proposed (and implemented) for NEAT. One example is k-means.
    /// 
    /// NEAT starts up by creating an initial population of genomes with randomly
    /// generated connections between input and output nodes. Not every input neuron
    /// is necessarily connected, this allows NEAT to determine which input neurons
    /// to use. Once the population has been generated it is speciated by iterating
    /// over this population of genomes. The first genome is placed in its own
    /// species.
    /// 
    /// The second genome is then compared to the first genome. If the compatibility
    /// is below the threshold then the genome is placed into the same species as the
    /// first. If not, the second genome founds a new species of its own. The
    /// remaining genomes follow this same process.
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
    public class OriginalNEATSpeciation : ThresholdSpeciation
    {
        /// <summary>
        /// The adjustment factor for disjoint genes.
        /// </summary>
        public double ConstDisjoint { get; set; }

        /// <summary>
        /// The adjustment factor for excess genes.
        /// </summary>
        public double ConstExcess { get; set; }

        /// <summary>
        /// The adjustment factor for matched genes.
        /// </summary>
        public double ConstMatched { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        public OriginalNEATSpeciation()
        {
            ConstDisjoint = 1;
            ConstExcess = 1;
            ConstMatched = 0.4;
        }

        /// <inheritdoc/>
        public override double GetCompatibilityScore(IGenome gen1,
                IGenome gen2)
        {
            double numDisjoint = 0;
            double numExcess = 0;
            double numMatched = 0;
            double weightDifference = 0;

            var genome1 = (NEATGenome)gen1;
            var genome2 = (NEATGenome)gen2;

            int genome1Size = genome1.LinksChromosome.Count;
            int genome2Size = genome2.LinksChromosome.Count;
            const int n = 1; // Math.max(genome1Size, genome2Size);

            int g1 = 0;
            int g2 = 0;

            while ((g1 < genome1Size) || (g2 < genome2Size))
            {

                if (g1 == genome1Size)
                {
                    g2++;
                    numExcess++;
                    continue;
                }

                if (g2 == genome2Size)
                {
                    g1++;
                    numExcess++;
                    continue;
                }

                // get innovation numbers for each gene at this point
                long id1 = genome1.LinksChromosome[g1].InnovationId;
                long id2 = genome2.LinksChromosome[g2].InnovationId;

                // innovation numbers are identical so increase the matched score
                if (id1 == id2)
                {

                    // get the weight difference between these two genes
                    weightDifference += Math.Abs(genome1.LinksChromosome[g1].Weight
                            - genome2.LinksChromosome[g2].Weight);
                    g1++;
                    g2++;
                    numMatched++;
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

            double score = ((ConstExcess * numExcess) / n)
                    + ((ConstDisjoint * numDisjoint) / n)
                    + (ConstMatched * (weightDifference / numMatched));

            return score;
        }

    }
}
