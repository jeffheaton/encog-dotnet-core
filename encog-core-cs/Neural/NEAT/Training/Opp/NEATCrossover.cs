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
using System.Collections.Generic;
using Encog.ML.EA.Opp;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;
using Encog.ML.EA.Genome;

namespace Encog.Neural.NEAT.Training.Opp
{
    /// <summary>
    /// Crossover is performed by mixing the link genes between the parents to
    /// produce an offspring. Only the link genes are considered for crossover. The
    /// neuron genes are chosen by virtue of which link genes were chosen. If a
    /// neuron gene is present in both parents, then we choose the neuron gene from
    /// the more fit of the two parents.
    /// 
    /// For NEAT, it does not really matter what parent we get the neuron gene from.
    /// However, because HyperNEAT also encodes a unique activation function into the
    /// neuron, the selection of a neuron gene between two parents is more important.
    /// 
    /// The crossover operator defines two broad classes of genes. Matching genes are
    /// those genes that are present in both parents. Non-matching genes are only
    /// present in one person. Non-matching genes are further divided into two more
    /// groups:
    /// 
    /// disjoint genes: Genes in the middle of a genome that do not match between the
    /// parents. excess genes: Genes at the edge of a genome that do not match
    /// between the parents.
    /// 
    /// Matching genes are inherited randomly, whereas disjoint genes (those that do
    /// not match in the middle) and excess genes (those that do not match in the
    /// end) are inherited from the more fit parent. In this case, equal fitnesses
    /// are assumed, so the disjoint and excess genes are also inherited randomly.
    /// The disabled genes may become enabled again in future generations: there is a
    /// preset chance that an inherited gene is disabled if it is disabled in either
    /// parent.
    /// 
    /// This is implemented in this class via the following algorithm. First, create
    /// a counter for each parent. At each step in the loop, perform the following.
    /// 
    /// If both parents have the same innovation number, then randomly choose which
    /// parent's gene to use. Increase the parent counter who contributed the gene.
    /// Else if one parent has a lower innovation number than the other, then include
    /// the lower innovation gene if its parent is the most fit. Increase the parent
    /// counter who contributed the gene.
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
    public class NEATCrossover : IEvolutionaryOperator
    {
        /// <summary>
        /// The owning object.
        /// </summary>
        private IEvolutionaryAlgorithm _owner;

        /// <summary>
        /// Add a neuron.
        /// </summary>
        /// <param name="nodeId">The neuron id.</param>
        /// <param name="vec">The id's used.</param>
        /// <param name="best">The best genome.</param>
        /// <param name="notBest">The non-best genome.</param>
        public void AddNeuronId(long nodeId, IList<NEATNeuronGene> vec,
                NEATGenome best, NEATGenome notBest)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i].Id == nodeId)
                {
                    return;
                }
            }

            vec.Add(FindBestNeuron(nodeId, best, notBest));

        }

        /// <summary>
        /// Choose a parent to favor.
        /// </summary>
        /// <param name="rnd">Random generator.</param>
        /// <param name="mom">The mother.</param>
        /// <param name="dad">The father</param>
        /// <returns></returns>
        private NEATGenome FavorParent(EncogRandom rnd, NEATGenome mom, NEATGenome dad)
        {

            // first determine who is more fit, the mother or the father?
            // see if mom and dad are the same fitness
            if (Math.Abs(mom.Score - dad.Score) < EncogFramework.DefaultDoubleEqual)
            {
                // are mom and dad the same fitness
                if (mom.NumGenes == dad.NumGenes)
                {
                    // if mom and dad are the same fitness and have the same number
                    // of genes,
                    // then randomly pick mom or dad as the most fit.
                    if (rnd.NextDouble() > 0.5)
                    {
                        return mom;
                    }
                    return dad;
                }
                    // mom and dad are the same fitness, but different number of genes
                // favor the parent with fewer genes
                if (mom.NumGenes < dad.NumGenes)
                {
                    return mom;
                }
                return dad;
            }
            // mom and dad have different scores, so choose the better score.
            // important to note, better score COULD BE the larger or smaller
            // score.
            if (_owner.SelectionComparer.Compare(mom, dad) < 0)
            {
                return mom;
            }

            return dad;
        }

        /// <summary>
        /// Find the best neuron, between two parents by the specified neuron id.
        /// </summary>
        /// <param name="nodeId">The neuron id.</param>
        /// <param name="best">The best genome.</param>
        /// <param name="notBest">The non-best (second best) genome. Also the worst, since this
        /// is the 2nd best of 2.</param>
        /// <returns>The best neuron genome by id.</returns>
        private NEATNeuronGene FindBestNeuron(long nodeId,
                NEATGenome best, NEATGenome notBest)
        {
            NEATNeuronGene result = best.FindNeuron(nodeId) ?? notBest.FindNeuron(nodeId);
            return result;
        }

        /// <summary>
        /// Init this operator. This allows the EA to be defined.
        /// </summary>
        /// <param name="theOwner">The owner.</param>
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            _owner = theOwner;
        }

        /// <inheritdoc/>
        public int OffspringProduced
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public int ParentsNeeded
        {
            get
            {
                return 2;
            }
        }

        /// <inheritdoc/>
        public void PerformOperation(EncogRandom rnd, IGenome[] parents,
                int parentIndex, IGenome[] offspring,
            int offspringIndex)
        {

            var mom = (NEATGenome)parents[parentIndex + 0];
            var dad = (NEATGenome)parents[parentIndex + 1];

            var best = FavorParent(rnd, mom, dad);
            var notBest = (best == mom) ? mom : dad;

            var selectedLinks = new List<NEATLinkGene>();
            var selectedNeurons = new List<NEATNeuronGene>();

            int curMom = 0; // current gene index from mom
            int curDad = 0; // current gene index from dad
            NEATLinkGene selectedGene = null;

            // add in the input and bias, they should always be here
            int alwaysCount = ((NEATGenome)parents[0]).InputCount
                    + ((NEATGenome)parents[0]).OutputCount + 1;
            for (int i = 0; i < alwaysCount; i++)
            {
                AddNeuronId(i, selectedNeurons, best, notBest);
            }

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {
                NEATLinkGene momGene = null; // the mom gene object
                NEATLinkGene dadGene = null; // the dad gene object
                long momInnovation = -1;
                long dadInnovation = -1;

                // grab the actual objects from mom and dad for the specified
                // indexes
                // if there are none, then null
                if (curMom < mom.NumGenes)
                {
                    momGene = mom.LinksChromosome[curMom];
                    momInnovation = momGene.InnovationId;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = dad.LinksChromosome[curDad];
                    dadInnovation = dadGene.InnovationId;
                }

                // now select a gene for mom or dad. This gene is for the baby
                if ((momGene == null) && (dadGene != null))
                {
                    if (best == dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if ((dadGene == null) && (momGene != null))
                {
                    if (best == mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (momInnovation < dadInnovation)
                {
                    if (best == mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (dadInnovation < momInnovation)
                {
                    if (best == dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if (dadInnovation == momInnovation)
                {
                    selectedGene = rnd.NextDouble() < 0.5f ? momGene : dadGene;
                    curMom++;
                    curDad++;
                }

                if (selectedGene != null)
                {
                    if (selectedLinks.Count == 0)
                    {
                        selectedLinks.Add(selectedGene);
                    }
                    else
                    {
                        if (selectedLinks[selectedLinks.Count - 1]
                                .InnovationId != selectedGene
                                .InnovationId)
                        {
                            selectedLinks.Add(selectedGene);
                        }
                    }

                    // Check if we already have the nodes referred to in
                    // SelectedGene.
                    // If not, they need to be added.
                    AddNeuronId(selectedGene.FromNeuronId, selectedNeurons,
                            best, notBest);
                    AddNeuronId(selectedGene.ToNeuronId, selectedNeurons,
                            best, notBest);
                }

            }

            // now create the required nodes. First sort them into order
            selectedNeurons.Sort();

            // finally, create the genome
            var factory = (INEATGenomeFactory)_owner
                    .Population.GenomeFactory;
            var babyGenome = factory.Factor(selectedNeurons,
                    selectedLinks, mom.InputCount, mom.OutputCount);
            babyGenome.BirthGeneration = _owner.IterationNumber;
            babyGenome.Population = _owner.Population;
            babyGenome.SortGenes();

            offspring[offspringIndex] = babyGenome;
        }
    }
}
