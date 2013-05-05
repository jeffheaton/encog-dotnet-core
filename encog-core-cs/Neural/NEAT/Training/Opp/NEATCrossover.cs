using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Opp.Selection;
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
        private IEvolutionaryAlgorithm owner;

        /// <summary>
        /// Add a neuron.
        /// </summary>
        /// <param name="nodeID">The neuron id.</param>
        /// <param name="vec">The id's used.</param>
        /// <param name="best">The best genome.</param>
        /// <param name="notBest">The non-best genome.</param>
        public void AddNeuronID(long nodeID, IList<NEATNeuronGene> vec,
                NEATGenome best, NEATGenome notBest)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i].Id == nodeID)
                {
                    return;
                }
            }

            vec.Add(FindBestNeuron(nodeID, best, notBest));

            return;
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
            if (mom.Score == dad.Score)
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
                    else
                    {
                        return dad;
                    }
                }
                // mom and dad are the same fitness, but different number of genes
                // favor the parent with fewer genes
                else
                {
                    if (mom.NumGenes < dad.NumGenes)
                    {
                        return mom;
                    }
                    else
                    {
                        return dad;
                    }
                }
            }
            else
            {
                // mom and dad have different scores, so choose the better score.
                // important to note, better score COULD BE the larger or smaller
                // score.
                if (this.owner.SelectionComparer.Compare(mom, dad) < 0)
                {
                    return mom;
                }

                else
                {
                    return dad;
                }
            }

        }

        /// <summary>
        /// Find the best neuron, between two parents by the specified neuron id.
        /// </summary>
        /// <param name="nodeID">The neuron id.</param>
        /// <param name="best">The best genome.</param>
        /// <param name="notBest">The non-best (second best) genome. Also the worst, since this
        /// is the 2nd best of 2.</param>
        /// <returns>The best neuron genome by id.</returns>
        private NEATNeuronGene FindBestNeuron(long nodeID,
                NEATGenome best, NEATGenome notBest)
        {
            NEATNeuronGene result = best.FindNeuron(nodeID);
            if (result == null)
            {
                result = notBest.FindNeuron(nodeID);
            }
            return result;
        }

        /// <summary>
        /// Init this operator. This allows the EA to be defined.
        /// </summary>
        /// <param name="theOwner">The owner.</param>
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            this.owner = theOwner;
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

            NEATGenome mom = (NEATGenome)parents[parentIndex + 0];
            NEATGenome dad = (NEATGenome)parents[parentIndex + 1];

            NEATGenome best = FavorParent(rnd, mom, dad);
            NEATGenome notBest = (best == mom) ? mom : dad;

            List<NEATLinkGene> selectedLinks = new List<NEATLinkGene>();
            List<NEATNeuronGene> selectedNeurons = new List<NEATNeuronGene>();

            int curMom = 0; // current gene index from mom
            int curDad = 0; // current gene index from dad
            NEATLinkGene selectedGene = null;

            // add in the input and bias, they should always be here
            int alwaysCount = ((NEATGenome)parents[0]).InputCount
                    + ((NEATGenome)parents[0]).OutputCount + 1;
            for (int i = 0; i < alwaysCount; i++)
            {
                AddNeuronID(i, selectedNeurons, best, notBest);
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
                    if (rnd.NextDouble() < 0.5f)
                    {
                        selectedGene = momGene;
                    }

                    else
                    {
                        selectedGene = dadGene;
                    }
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
                    AddNeuronID(selectedGene.FromNeuronID, selectedNeurons,
                            best, notBest);
                    AddNeuronID(selectedGene.ToNeuronID, selectedNeurons,
                            best, notBest);
                }

            }

            // now create the required nodes. First sort them into order
            selectedNeurons.Sort();

            // finally, create the genome
            INEATGenomeFactory factory = (INEATGenomeFactory)this.owner
                    .Population.GenomeFactory;
            NEATGenome babyGenome = factory.Factor(selectedNeurons,
                    selectedLinks, mom.InputCount, mom.OutputCount);
            babyGenome.BirthGeneration = this.owner.IterationNumber;
            babyGenome.Population = this.owner.Population;
            babyGenome.SortGenes();

            offspring[offspringIndex] = babyGenome;
        }
    }
}
