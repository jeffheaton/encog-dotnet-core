// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Species;
using Encog.Solve.Genetic.Population;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Genetic;
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Layers;
using Encog.Cloud;
using Encog.Engine.Network.Activation;

namespace Encog.Neural.Networks.Training.NEAT
{
    /// <summary>
    /// Simple enum to hold parents.
    /// </summary>
    enum NEATParent
    {
        Mom,
        Dad
    }

    /// <summary>
    /// Train a NEAT neural network, using a Genetic Algorithm.
    /// </summary>
    public class NEATTraining : GeneticAlgorithm, ITrain
    {
        /// <summary>
        /// The average fit adjustment.
        /// </summary>
        private double averageFitAdjustment;

        /// <summary>
        /// The best ever score.
        /// </summary>
        private double bestEverScore;

        /// <summary>
        /// The best ever network.
        /// </summary>
        private BasicNetwork bestEverNetwork;

        /// <summary>
        /// The number of inputs.
        /// </summary>
        private int inputCount;

        /// <summary>
        /// The activation function for neat to use.
        /// </summary>
        public IActivationFunction NeatActivationFunction { get; set; }

        /// <summary>
        /// The activation function to use on the output layer of Encog.
        /// </summary>
        public IActivationFunction OutputActivationFunction { get; set; }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private int outputCount;

        /// <summary>
        /// The activation mutation rate.
        /// </summary>
        public double ParamActivationMutationRate { get; set; }

        /// <summary>
        /// The likelyhood of adding a link.
        /// </summary>
        public double ParamChanceAddLink { get; set; }

        /// <summary>
        /// The likelyhood of adding a node.
        /// </summary>
        public double ParamChanceAddNode { get; set; }

        /// <summary>
        /// The likelyhood of adding a recurrent link.
        /// </summary>
        public double ParamChanceAddRecurrentLink { get; set; }

        /// <summary>
        /// The compatibility threshold for a species.
        /// </summary>
        public double ParamCompatibilityThreshold { get; set; }

        /// <summary>
        /// The crossover rate.
        /// </summary>
        public double ParamCrossoverRate { get; set; }

        /// <summary>
        /// The max activation perturbation.
        /// </summary>
        public double ParamMaxActivationPerturbation { get; set; }

        /// <summary>
        /// The maximum number of species.
        /// </summary>
        public int ParamMaxNumberOfSpecies { get; set; }

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        public double ParamMaxPermittedNeurons { get; set; }

        /// <summary>
        /// The maximum weight perturbation.
        /// </summary>
        public double ParamMaxWeightPerturbation { get; set; }

        /// <summary>
        /// The mutation rate.
        /// </summary>
        public double ParamMutationRate { get; set; }

        /// <summary>
        /// The number of link add attempts.
        /// </summary>
        public int ParamNumAddLinkAttempts { get; set; }

        /// <summary>
        /// The number of generations allowed with no improvement.
        /// </summary>
        public int ParamNumGensAllowedNoImprovement { get; set; }

        /// <summary>
        /// The number of tries to find a looped link.
        /// </summary>
        public int ParamNumTrysToFindLoopedLink { get; set; }

        /// <summary>
        /// The number of tries to find an old link.
        /// </summary>
        public int ParamNumTrysToFindOldLink { get; set; }

        /// <summary>
        /// The probability that the weight will be totally replaced.
        /// </summary>
        public double ParamProbabilityWeightReplaced { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// The cloud.
        /// </summary>
        public EncogCloud Cloud { get; set; }
#endif

        /// <summary>
        /// The splits.
        /// </summary>
        private List<SplitDepth> splits;

        /// <summary>
        /// The total fit adjustment.
        /// </summary>
        private double totalFitAdjustment;

        /// <summary>
        /// Use snapshot?
        /// </summary>
        public bool Snapshot { get; set; }

        /// <summary>
        /// Construct neat training with a predefined population. 
        /// </summary>
        /// <param name="calculateScore">The score object to use.</param>
        /// <param name="population">The population to use.</param>
        public NEATTraining(ICalculateScore calculateScore,
                IPopulation population)
        {
            if (population.Genomes.Count < 1)
            {
                throw new TrainingError("Population can not be empty.");
            }

            NEATGenome genome = (NEATGenome)population.Genomes[0];
            this.CalculateScore = new GeneticScoreAdapter(calculateScore);
            this.Population = population;
            this.inputCount = genome.InputCount;
            this.outputCount = genome.OutputCount;

            foreach (IGenome obj in population.Genomes)
            {
                NEATGenome neat = (NEATGenome)obj;
                neat.GA = this;
            }

            Init();
        }

       
        /// <summary>
        /// Construct a neat trainer with a new population. 
        /// </summary>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATTraining(ICalculateScore calculateScore,
                int inputCount, int outputCount,
                int populationSize)
        {

            this.inputCount = inputCount;
            this.outputCount = outputCount;

            CalculateScore = new GeneticScoreAdapter(calculateScore);
            Comparator = new GenomeComparator(CalculateScore);
            Population = new BasicPopulation(populationSize);

            // create the initial population
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(
                        new NEATGenome(this, Population.AssignGenomeID(),
                                inputCount, outputCount));
            }

           Init();
        }

        /// <summary>
        /// Construct a NEAT training class.
        /// </summary>
        /// <param name="score">How to score the networks.</param>
        /// <param name="network">The network to base this on.</param>
        /// <param name="population">The population to use.</param>
        public NEATTraining(ICalculateScore score, BasicNetwork network,
        IPopulation population)
        {
            ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);
            this.CalculateScore = new GeneticScoreAdapter(score);
            this.Comparator = new GenomeComparator(CalculateScore);
            this.inputCount = inputLayer.NeuronCount;
            this.outputCount = outputLayer.NeuronCount;
            this.Population = population;

            foreach (IGenome obj in population.Genomes)
            {
                NEATGenome neat = (NEATGenome)obj;
                neat.GA = this;
            }

            Init();
        }

        /// <summary>
        /// Setup for training.
        /// </summary>
        private void Init()
        {
            // default values
            ParamActivationMutationRate = 0.1;
            ParamChanceAddLink = 0.07;
            ParamChanceAddNode = 0.04;
            ParamChanceAddRecurrentLink = 0.05;
            ParamCompatibilityThreshold = 0.26;
            ParamCrossoverRate = 0.7;
            ParamMaxActivationPerturbation = 0.1;
            ParamMaxNumberOfSpecies = 0;
            ParamMaxPermittedNeurons = 100;
            ParamMaxWeightPerturbation = 0.5;
            ParamMutationRate = 0.2;
            ParamNumAddLinkAttempts = 5;
            ParamNumGensAllowedNoImprovement = 15;
            ParamNumTrysToFindLoopedLink = 5;
            ParamNumTrysToFindOldLink = 5;
            ParamProbabilityWeightReplaced = 0.1;

            NeatActivationFunction = new ActivationSigmoid();
            OutputActivationFunction = new ActivationLinear();


            //
            NEATGenome genome = (NEATGenome)Population.Genomes[0];

            Population.Innovations =
                    new NEATInnovationList(Population, genome.Links,
                            genome.Neurons);

            splits = Split(null, 0, 1, 0);

            if (CalculateScore.ShouldMinimize)
            {
                bestEverScore = double.MaxValue;
            }
            else
            {
                bestEverScore = double.MinValue;
            }

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }


        /// <summary>
        /// Add a neuron. 
        /// </summary>
        /// <param name="nodeID">The neuron id.</param>
        /// <param name="vec">The list of id's used.</param>
        public void AddNeuronID(long nodeID, IList<long> vec)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i] == nodeID)
                {
                    return;
                }
            }

            vec.Add(nodeID);

            return;
        }

        /// <summary>
        /// Not supported, will throw an error. 
        /// </summary>
        /// <param name="strategy">Not used.</param>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                    "Strategies are not supported by this training method.");
        }

        /// <summary>
        /// Adjust the species compatibility threshold. This prevents us from having
        /// too many species.
        /// </summary>
        public void AdjustCompatibilityThreshold()
        {

            // has this been disabled (unlimited species)
            if (ParamMaxNumberOfSpecies < 1)
            {
                return;
            }

            double thresholdIncrement = 0.01;

            if (Population.Species.Count > ParamMaxNumberOfSpecies)
            {
                ParamCompatibilityThreshold += thresholdIncrement;
            }

            else if (Population.Species.Count < 2)
            {
                ParamCompatibilityThreshold -= thresholdIncrement;
            }

        }

        /// <summary>
        /// Adjust each species score.
        /// </summary>
        public void AdjustSpeciesScore()
        {
            foreach (ISpecies s in this.Population.Species)
            {
                foreach (IGenome member in s.Members )
                {
                    double score = member.Score;

                    // apply a youth bonus
                    if (s.Age < this.Population.YoungBonusAgeThreshold)
                    {
                        score = this.Comparator.ApplyBonus(score,
                                this.Population.YoungScoreBonus);
                    }
                    // apply an old age penalty
                    if (s.Age > this.Population.OldAgeThreshold)
                    {
                        score = this.Comparator.ApplyPenalty(score,
                                this.Population.OldAgePenalty);
                    }

                    double adjustedScore = score / s.Members.Count;

                    member.AdjustedScore = adjustedScore;

                }
            }
        }

        /// <summary>
        /// Calculate the network depth for the specified genome. 
        /// </summary>
        /// <param name="genome">The genome to calculate.</param>
        private void CalculateNetDepth(NEATGenome genome)
        {
            int maxSoFar = 0;

            for (int nd = 0; nd < genome.Neurons.Genes.Count; ++nd)
            {
                foreach (SplitDepth split in splits)
                {

                    if ((genome.GetSplitY(nd) == split.Value)
                            && (split.Depth > maxSoFar))
                    {
                        maxSoFar = split.Depth;
                    }
                }
            }

            genome.NetworkDepth = maxSoFar + 2;
        }

        
        /// <summary>
        /// Perform the crossover. 
        /// </summary>
        /// <param name="mom">The mother.</param>
        /// <param name="dad">The father.</param>
        /// <returns>The child.</returns>
        public NEATGenome PerformCrossover(NEATGenome mom, NEATGenome dad)
        {
            NEATParent best;

            // first determine who is more fit, the mother or the father?
            if (mom.Score == dad.Score)
            {
                if (mom.NumGenes == dad.NumGenes)
                {
                    if (ThreadSafeRandom.NextDouble() > 0)
                    {
                        best = NEATParent.Mom;
                    }
                    else
                    {
                        best = NEATParent.Dad;
                    }
                }

                else
                {
                    if (mom.NumGenes < dad.NumGenes)
                    {
                        best = NEATParent.Mom;
                    }

                    else
                    {
                        best = NEATParent.Dad;
                    }
                }
            }

            else
            {
                if (Comparator.IsBetterThan(mom.Score, dad.Score))
                {
                    best = NEATParent.Mom;
                }

                else
                {
                    best = NEATParent.Dad;
                }
            }

            Chromosome babyNeurons = new Chromosome();
            Chromosome babyGenes = new Chromosome();

            List<long> vecNeurons = new List<long>();

            int curMom = 0;
            int curDad = 0;

            NEATLinkGene momGene;
            NEATLinkGene dadGene;

            NEATLinkGene selectedGene = null;

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {

                if (curMom < mom.NumGenes)
                {
                    momGene = (NEATLinkGene)mom.Links.Genes[curMom];
                }
                else
                {
                    momGene = null;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = (NEATLinkGene)dad.Links.Genes[curDad];
                }
                else
                {
                    dadGene = null;
                }

                if ((momGene == null) && (dadGene != null))
                {
                    if (best == NEATParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if ((dadGene == null) && (momGene != null))
                {
                    if (best == NEATParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (momGene.InnovationId < dadGene.InnovationId)
                {
                    if (best == NEATParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (dadGene.InnovationId < momGene.InnovationId)
                {
                    if (best == NEATParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if (dadGene.InnovationId == momGene.InnovationId)
                {
                    if (ThreadSafeRandom.NextDouble() < 0.5f)
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

                if (babyGenes.Genes.Count == 0)
                {
                    babyGenes.Genes.Add(selectedGene);
                }

                else
                {
                    if (babyGenes.Genes[babyGenes.Genes.Count - 1]
                            .InnovationId != selectedGene.InnovationId)
                    {
                        babyGenes.Genes.Add(selectedGene);
                    }
                }

                // Check if we already have the nodes referred to in SelectedGene.
                // If not, they need to be added.
                AddNeuronID(selectedGene.FromNeuronID, vecNeurons);
                AddNeuronID(selectedGene.ToNeuronID, vecNeurons);

            }

            // now create the required nodes. First sort them into order
            vecNeurons.Sort();

            for (int i = 0; i < vecNeurons.Count; i++)
            {
                babyNeurons.Genes.Add(this.Innovations.CreateNeuronFromID(
                        vecNeurons[i]));
            }

            // finally, create the genome
            NEATGenome babyGenome = new NEATGenome(this, Population
                    .AssignGenomeID(), babyNeurons, babyGenes, mom.InputCount,
                    mom.OutputCount);

            return babyGenome;
        }

        /// <summary>
        /// Called when training is done.
        /// </summary>
        public void FinishTraining()
        {

        }

        /// <summary>
        /// The error for the best genome.
        /// </summary>
        public double Error
        {
            get
            {
                return this.bestEverScore;
            }
            set
            {
            }
        }

        /// <summary>
        /// The innovations.
        /// </summary>
        public NEATInnovationList Innovations
        {
            get
            {
                return (NEATInnovationList)Population.Innovations;
            }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get
            {
                return inputCount;
            }
        }

        /// <summary>
        /// A network created for the best genome.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.bestEverNetwork;
            }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get
            {
                return outputCount;
            }
        }

        /// <summary>
        /// Returns an empty list, strategies are not supported. 
        /// </summary>
        public IList<IStrategy> Strategies
        {
            get
            {
                return new List<IStrategy>();
            }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public INeuralDataSet Training
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {

            IList<NEATGenome> newPop = new List<NEATGenome>();

            int numSpawnedSoFar = 0;

            foreach (ISpecies s in Population.Species)
            {
                if (numSpawnedSoFar < Population.Genomes.Count)
                {
                    int numToSpawn = (int)Math.Round(s.NumToSpawn);

                    bool bChosenBestYet = false;

                    while ((numToSpawn--) > 0)
                    {
                        NEATGenome baby = null;

                        if (!bChosenBestYet)
                        {
                            baby = (NEATGenome)s.Leader;

                            bChosenBestYet = true;
                        }

                        else
                        {
                            // if the number of individuals in this species is only
                            // one
                            // then we can only perform mutation
                            if (s.Members.Count == 1)
                            {
                                // spawn a child
                                baby = new NEATGenome((NEATGenome)s.ChooseParent());
                            }
                            else
                            {
                                NEATGenome g1 = (NEATGenome)s.ChooseParent();

                                if (ThreadSafeRandom.NextDouble() < ParamCrossoverRate)
                                {
                                    NEATGenome g2 = (NEATGenome)s.ChooseParent();

                                    int NumAttempts = 5;

                                    while ((g1.GenomeID == g2.GenomeID)
                                            && ((NumAttempts--) > 0))
                                    {
                                        g2 = (NEATGenome)s.ChooseParent();
                                    }

                                    if (g1.GenomeID != g2.GenomeID)
                                    {
                                        baby = PerformCrossover(g1, g2);
                                    }
                                }

                                else
                                {
                                    baby = new NEATGenome(g1);
                                }
                            }

                            if (baby != null)
                            {
                                baby.GenomeID = Population.AssignGenomeID();

                                if (baby.Neurons.Genes.Count < ParamMaxPermittedNeurons)
                                {
                                    baby.AddNeuron(ParamChanceAddNode,
                                            ParamNumTrysToFindOldLink);
                                }

                                // now there's the chance a link may be added
                                baby.AddLink(ParamChanceAddLink,
                                        ParamChanceAddRecurrentLink,
                                        ParamNumTrysToFindLoopedLink,
                                        ParamNumAddLinkAttempts);

                                // mutate the weights
                                baby.MutateWeights(ParamMutationRate,
                                        ParamProbabilityWeightReplaced,
                                        ParamMaxWeightPerturbation);

                                baby.MutateActivationResponse(
                                        ParamActivationMutationRate,
                                        ParamMaxActivationPerturbation);

                            }
                        }

                        if (baby != null)
                        {
                            // sort the baby's genes by their innovation numbers
                            baby.SortGenes();

                            if (newPop.Contains(baby))
                                throw new EncogError("add");

                            newPop.Add(baby);

                            ++numSpawnedSoFar;

                            if (numSpawnedSoFar == Population.Genomes.Count)
                            {
                                numToSpawn = 0;
                            }
                        }
                    }
                }
            }

            while (newPop.Count < Population.Genomes.Count)
            {
                NEATGenome newOne = TournamentSelection(Population.Genomes.Count / 5);
                newPop.Add(newOne);
            }

            Population.Clear();
            for (int i = 0; i < newPop.Count; i++)
            {
                Population.Add(newPop[i]);
            }

            ResetAndKill();
            SortAndRecord();

            IGenome genome = Population.GetBest();
            double currentBest = genome.Score;
            if (this.Comparator.IsBetterThan(currentBest, bestEverScore))
            {
                bestEverScore = currentBest;
                this.bestEverNetwork = ((BasicNetwork)genome.Organism);
            }

            SpeciateAndCalculateSpawnLevels();
        }


        /// <summary>
        /// Reset for an iteration.
        /// </summary>
        public void ResetAndKill()
        {
            totalFitAdjustment = 0;
            averageFitAdjustment = 0;

            Object[] speciesArray = Population.Species.ToArray();

            for (int i = 0; i < speciesArray.Length; i++)
            {
                ISpecies s = (ISpecies)speciesArray[i];
                s.Purge();

                if ((s.GensNoImprovement > ParamNumGensAllowedNoImprovement)
                        && Comparator.IsBetterThan(bestEverScore,
                                s.BestScore))
                {
                    Population.Species.Remove(s);
                }
            }
        }

        /// <summary>
        /// Sort the genomes.
        /// </summary>
        public void SortAndRecord()
        {
            foreach (IGenome genome in this.Population.Genomes)
            {
                genome.Decode();
                PerformScoreCalculation(genome);
            }

            Population.Sort();

            bestEverScore = Comparator.BestScore(Error, bestEverScore);
        }

        /// <summary>
        /// Determine the species.
        /// </summary>
        public void SpeciateAndCalculateSpawnLevels()
        {

            // calculate compatibility between genomes and species
            AdjustCompatibilityThreshold();

            // assign genomes to species (if any exist)
            foreach (IGenome g in Population.Genomes)
            {
                NEATGenome genome = (NEATGenome)g;
                bool added = false;

                foreach (ISpecies s in Population.Species)
                {
                    double compatibility = genome
                            .GetCompatibilityScore((NEATGenome)s.Leader);

                    if (compatibility <= ParamCompatibilityThreshold)
                    {
                        AddSpeciesMember(s, genome);
                        genome.SpeciesID = s.SpeciesID;
                        added = true;
                        break;
                    }
                }

                // if this genome did not fall into any existing species, create a
                // new species
                if (!added)
                {
                    Population.Species.Add(
                            new BasicSpecies(this.Population, genome, Population
                                    .AssignSpeciesID()));
                }
            }

            AdjustSpeciesScore();

            foreach (IGenome g in Population.Genomes)
            {
                NEATGenome genome = (NEATGenome)g;
                totalFitAdjustment += genome.AdjustedScore;
            }

            averageFitAdjustment = totalFitAdjustment / Population.Genomes.Count;

            foreach (IGenome g in Population.Genomes)
            {
                NEATGenome genome = (NEATGenome)g;
                double toSpawn = genome.AdjustedScore
                        / averageFitAdjustment;
                genome.AmountToSpawn = toSpawn;
            }

            foreach (ISpecies species in Population.Species)
            {
                species.CalculateSpawnAmount();
            }
        }

        
        /// <summary>
        /// Calculate splits. 
        /// </summary>
        /// <param name="result">The resulting list, used for recursive calls.</param>
        /// <param name="low">The high to check.</param>
        /// <param name="high">The low to check.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>A list of split depths.</returns>
        private List<SplitDepth> Split(List<SplitDepth> result, double low,
                double high, int depth)
        {
            if (result == null)
            {
                result = new List<SplitDepth>();
            }

            double span = high - low;

            result.Add(new SplitDepth(low + span / 2, depth + 1));

            if (depth > 6)
            {
                return result;
            }

            else
            {
                Split(result, low, low + span / 2, depth + 1);
                Split(result, low + span / 2, high, depth + 1);
                return result;
            }
        }

     
        /// <summary>
        /// Select a gene using a tournament. 
        /// </summary>
        /// <param name="numComparisons">The number of compares to do.</param>
        /// <returns>The chosen genome.</returns>
        public NEATGenome TournamentSelection(int numComparisons)
        {
            double bestScoreSoFar = 0;

            int ChosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                int ThisTry = (int)RangeRandomizer.Randomize(0,
                        Population.Genomes.Count - 1);

                if (Population.Genomes[ThisTry].Score > bestScoreSoFar)
                {
                    ChosenOne = ThisTry;

                    bestScoreSoFar = Population.Genomes[ThisTry].Score;
                }
            }

            return (NEATGenome)Population.Genomes[ChosenOne];
        }
    }
}
