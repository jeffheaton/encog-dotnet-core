//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Genetic;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Population;
using Encog.ML.Genetic.Species;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Genetic;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements NEAT genetic training.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    ///
    public class NEATTraining : GeneticAlgorithm, MLTrain
    {        
        /// <summary>
        /// The number of inputs.
        /// </summary>
        private readonly int inputCount;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private readonly int outputCount;

        /// <summary>
        /// The average fit adjustment.
        /// </summary>
        private double averageFitAdjustment;

        /// <summary>
        /// The best ever network.
        /// </summary>
        private NEATNetwork bestEverNetwork;

        /// <summary>
        /// The best ever score.
        /// </summary>
        private double bestEverScore;

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The activation mutation rate.
        /// </summary>
        private double paramActivationMutationRate = 0.1;

        /// <summary>
        /// The likelyhood of adding a link.
        /// </summary>
        private double paramChanceAddLink = 0.07;

        /// <summary>
        /// The likelyhood of adding a node.
        /// </summary>
        private double paramChanceAddNode = 0.04;

        /// <summary>
        /// The likelyhood of adding a recurrent link.
        /// </summary>
        private double paramChanceAddRecurrentLink = 0.05;

        /// <summary>
        /// The compatibility threshold for a species.
        /// </summary>
        private double paramCompatibilityThreshold = 0.26;

        /// <summary>
        /// The crossover rate.
        /// </summary>
        private double paramCrossoverRate = 0.7;

        /// <summary>
        /// The max activation perturbation.
        /// </summary>
        private double paramMaxActivationPerturbation = 0.1;

        /// <summary>
        /// The maximum number of species.
        /// </summary>
        private int paramMaxNumberOfSpecies;

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        private double paramMaxPermittedNeurons = 100;

        /// <summary>
        /// The maximum weight perturbation.
        /// </summary>
        private double paramMaxWeightPerturbation = 0.5;

        /// <summary>
        /// The mutation rate.
        /// </summary>
        private double paramMutationRate = 0.2;

        /// <summary>
        /// The number of link add attempts.
        /// </summary>
        private int paramNumAddLinkAttempts = 5;

        /// <summary>
        /// The number of generations allowed with no improvement.
        /// </summary>
        private int paramNumGensAllowedNoImprovement = 15;

        /// <summary>
        /// The number of tries to find a looped link.
        /// </summary>
        private int paramNumTrysToFindLoopedLink = 5;

        /// <summary>
        /// The number of tries to find an old link.
        /// </summary>
        private int paramNumTrysToFindOldLink = 5;

        /// <summary>
        /// The probability that the weight will be totally replaced.
        /// </summary>
        private double paramProbabilityWeightReplaced = 0.1;

        /// <summary>
        /// Determines if we are using snapshot mode.
        /// </summary>
        private bool snapshot;

        /// <summary>
        /// The total fit adjustment.
        /// </summary>
        private double totalFitAdjustment;

        /// <summary>
        /// Construct a neat trainer with a new population. The new population is
        /// created from the specified parameters.
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
            Population = new NEATPopulation(inputCount, outputCount,
                                            populationSize);

            Init();
        }

        /// <summary>
        /// Construct neat training with an existing population.
        /// </summary>
        /// <param name="calculateScore">The score object to use.</param>
        /// <param name="population">The population to use.</param>
        public NEATTraining(ICalculateScore calculateScore,
                            IPopulation population)
        {
            if (population.Size() < 1)
            {
                throw new TrainingError("Population can not be empty.");
            }

            var genome = (NEATGenome) population.Genomes[0];
            CalculateScore = new GeneticScoreAdapter(calculateScore);
            Comparator = new GenomeComparator(CalculateScore);
            Population = (population);
            inputCount = genome.InputCount;
            outputCount = genome.OutputCount;

            Init();
        }

        /// <summary>
        /// The innovations.
        /// </summary>
        public NEATInnovationList Innovations
        {
            get { return (NEATInnovationList) Population.Innovations; }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
        }

        /// <summary>
        /// Set the activation mutation rate.
        /// </summary>
        public double ParamActivationMutationRate
        {
            get { return paramActivationMutationRate; }
            set { paramActivationMutationRate = value; }
        }


        /// <summary>
        /// Set the chance to add a link.
        /// </summary>
        public double ParamChanceAddLink
        {
            get { return paramChanceAddLink; }
            set { paramChanceAddLink = value; }
        }


        /// <summary>
        /// Set the chance to add a node.
        /// </summary>
        public double ParamChanceAddNode
        {
            get { return paramChanceAddNode; }
            set { paramChanceAddNode = value; }
        }

        /// <summary>
        /// Set the chance to add a recurrent link.
        /// </summary>
        public double ParamChanceAddRecurrentLink
        {
            get { return paramChanceAddRecurrentLink; }
            set { paramChanceAddRecurrentLink = value; }
        }


        /// <summary>
        /// Set the compatibility threshold for species.
        /// </summary>
        public double ParamCompatibilityThreshold
        {
            get { return paramCompatibilityThreshold; }
            set { paramCompatibilityThreshold = value; }
        }


        /// <summary>
        /// Set the cross over rate.
        /// </summary>
        public double ParamCrossoverRate
        {
            get { return paramCrossoverRate; }
            set { paramCrossoverRate = value; }
        }


        /// <summary>
        /// Set the max activation perturbation.
        /// </summary>
        public double ParamMaxActivationPerturbation
        {
            get { return paramMaxActivationPerturbation; }
            set { paramMaxActivationPerturbation = value; }
        }

        /// <summary>
        /// Set the maximum number of species.
        /// </summary>
        public int ParamMaxNumberOfSpecies
        {
            get { return paramMaxNumberOfSpecies; }
            set { paramMaxNumberOfSpecies = value; }
        }

        /// <summary>
        /// Set the max permitted neurons.
        /// </summary>
        public double ParamMaxPermittedNeurons
        {
            get { return paramMaxPermittedNeurons; }
            set { paramMaxPermittedNeurons = value; }
        }

       /// <summary>
        /// Set the max weight perturbation.
       /// </summary>
        public double ParamMaxWeightPerturbation
        {
            get { return paramMaxWeightPerturbation; }
            set { paramMaxWeightPerturbation = value; }
        }

        /// <summary>
        /// Set the mutation rate.
        /// </summary>
        public double ParamMutationRate
        {
            get { return paramMutationRate; }
            set { paramMutationRate = value; }
        }

        /// <summary>
        /// Set the number of attempts to add a link.
        /// </summary>
        public int ParamNumAddLinkAttempts
        {
            get { return paramNumAddLinkAttempts; }
            set { paramNumAddLinkAttempts = value; }
        }

        /// <summary>
        /// Set the number of no-improvement generations allowed.
        /// </summary>
        public int ParamNumGensAllowedNoImprovement
        {
            get { return paramNumGensAllowedNoImprovement; }
            set { paramNumGensAllowedNoImprovement = value; }
        }

        /// <summary>
        /// Set the number of tries to create a looped link.
        /// </summary>
        public int ParamNumTrysToFindLoopedLink
        {
            get { return paramNumTrysToFindLoopedLink; }
            set { paramNumTrysToFindLoopedLink = value; }
        }


        /// <summary>
        /// Set the number of tries to try an old link.
        /// </summary>
        public int ParamNumTrysToFindOldLink
        {
            get { return paramNumTrysToFindOldLink; }
            set { paramNumTrysToFindOldLink = value; }
        }


        /// <summary>
        /// Set the probability to replace a weight.
        /// </summary>
        public double ParamProbabilityWeightReplaced
        {
            get { return paramProbabilityWeightReplaced; }
            set { paramProbabilityWeightReplaced = value; }
        }

        /// <summary>
        /// Set if we are using snapshot mode.
        /// </summary>
        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        #region MLTrain Members

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {
        }

        /// <summary>
        /// The error for the best genome.
        /// </summary>
        public double Error
        {
            get { return bestEverScore; }
            set { bestEverScore = value; }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        /// <inheritdoc/>
        public int IterationNumber
        {
            get { return iteration; }
            set { iteration = value; }
        }

        /// <summary>
        /// A network created for the best genome.
        /// </summary>
        public IMLMethod Method
        {
            get { return bestEverNetwork; }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            iteration++;
            IList<NEATGenome> newPop = new List<NEATGenome>();

            int numSpawnedSoFar = 0;

            foreach (ISpecies s in Population.Species)
            {
                if (numSpawnedSoFar < Population.Size())
                {
                    var numToSpawn = (int) Math.Round(s.NumToSpawn);

                    bool bChosenBestYet = false;

                    while ((numToSpawn--) > 0)
                    {
                        NEATGenome baby = null;

                        if (!bChosenBestYet)
                        {
                            baby = (NEATGenome) s.Leader;

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
                                baby = new NEATGenome((NEATGenome) s.ChooseParent());
                            }
                            else
                            {
                                var g1 = (NEATGenome) s.ChooseParent();

                                if (ThreadSafeRandom.NextDouble() < paramCrossoverRate)
                                {
                                    var g2 = (NEATGenome) s.ChooseParent();

                                    int numAttempts = 5;

                                    while ((g1.GenomeID == g2.GenomeID)
                                           && ((numAttempts--) > 0))
                                    {
                                        g2 = (NEATGenome) s.ChooseParent();
                                    }

                                    if (g1.GenomeID != g2.GenomeID)
                                    {
                                        baby = Crossover(g1, g2);
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

                                if (baby.Neurons.Size() < paramMaxPermittedNeurons)
                                {
                                    baby.AddNeuron(paramChanceAddNode,
                                                   paramNumTrysToFindOldLink);
                                }

                                // now there's the chance a link may be added
                                baby.AddLink(paramChanceAddLink,
                                             paramChanceAddRecurrentLink,
                                             paramNumTrysToFindLoopedLink,
                                             paramNumAddLinkAttempts);

                                // mutate the weights
                                baby.MutateWeights(paramMutationRate,
                                                   paramProbabilityWeightReplaced,
                                                   paramMaxWeightPerturbation);

                                baby.MutateActivationResponse(
                                    paramActivationMutationRate,
                                    paramMaxActivationPerturbation);
                            }
                        }

                        if (baby != null)
                        {
                            // sort the baby's genes by their innovation numbers
                            baby.SortGenes();

                            // add to new pop
                            // if (newPop.contains(baby)) {
                            // throw new EncogError("readd");
                            // }
                            newPop.Add(baby);

                            ++numSpawnedSoFar;

                            if (numSpawnedSoFar == Population.Size())
                            {
                                numToSpawn = 0;
                            }
                        }
                    }
                }
            }

            while (newPop.Count < Population.Size())
            {
                newPop.Add(TournamentSelection(Population.Size()/5));
            }

            Population.Clear();
            foreach (NEATGenome genome in newPop)
            {
                Population.Add(genome);
            }

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {
        }

        #endregion

        /// <summary>
        /// Add the specified neuron id.
        /// </summary>
        /// <param name="nodeID">The neuron to add.</param>
        /// <param name="vec">The list to add to.</param>
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
        /// Adjust the compatibility threshold.
        /// </summary>
        public void AdjustCompatibilityThreshold()
        {
            // has this been disabled (unlimited species)
            if (paramMaxNumberOfSpecies < 1)
            {
                return;
            }

            double thresholdIncrement = 0.01;

            if (Population.Species.Count > paramMaxNumberOfSpecies)
            {
                paramCompatibilityThreshold += thresholdIncrement;
            }

            else if (Population.Species.Count < 2)
            {
                paramCompatibilityThreshold -= thresholdIncrement;
            }
        }

        /// <summary>
        /// Adjust each species score.
        /// </summary>
        public void AdjustSpeciesScore()
        {
            foreach (ISpecies s in Population.Species)
            {
                // loop over all genomes and adjust scores as needed
                foreach (IGenome member in s.Members)
                {
                    double score = member.Score;

                    // apply a youth bonus
                    if (s.Age < Population.YoungBonusAgeThreshold)
                    {
                        score = Comparator.ApplyBonus(score,
                                                      Population.YoungScoreBonus);
                    }

                    // apply an old age penalty
                    if (s.Age > Population.OldAgeThreshold)
                    {
                        score = Comparator.ApplyPenalty(score,
                                                        Population.OldAgePenalty);
                    }

                    double adjustedScore = score/s.Members.Count;

                    member.AdjustedScore = adjustedScore;
                }
            }
        }

        /// <summary>
        /// Perform a cross over.  
        /// </summary>
        /// <param name="mom">The mother genome.</param>
        /// <param name="dad">The father genome.</param>
        /// <returns></returns>
        public new NEATGenome Crossover(NEATGenome mom, NEATGenome dad)
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

            var babyNeurons = new Chromosome();
            var babyGenes = new Chromosome();

            var vecNeurons = new List<long>();

            int curMom = 0;
            int curDad = 0;

            NEATLinkGene momGene;
            NEATLinkGene dadGene;

            NEATLinkGene selectedGene = null;

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {
                if (curMom < mom.NumGenes)
                {
                    momGene = (NEATLinkGene) mom.Links.Get(curMom);
                }
                else
                {
                    momGene = null;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = (NEATLinkGene) dad.Links.Get(curDad);
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

                if (babyGenes.Size() == 0)
                {
                    babyGenes.Add(selectedGene);
                }

                else
                {
                    if (((NEATLinkGene) babyGenes.Get(babyGenes.Size() - 1))
                            .InnovationId != selectedGene.InnovationId)
                    {
                        babyGenes.Add(selectedGene);
                    }
                }

                // Check if we already have the nodes referred to in SelectedGene.
                // If not, they need to be added.
                AddNeuronID(selectedGene.FromNeuronID, vecNeurons);
                AddNeuronID(selectedGene.ToNeuronID, vecNeurons);
            } // end while

            // now create the required nodes. First sort them into order
            vecNeurons.Sort();

            for (int i = 0; i < vecNeurons.Count; i++)
            {
                babyNeurons.Add(Innovations.CreateNeuronFromID(
                    vecNeurons[i]));
            }

            // finally, create the genome
            var babyGenome = new NEATGenome(Population
                                                .AssignGenomeID(), babyNeurons, babyGenes, mom.InputCount,
                                            mom.OutputCount);
            babyGenome.GA = this;
            babyGenome.Population = Population;

            return babyGenome;
        }

        /// <summary>
        /// Init the training.
        /// </summary>
        private void Init()
        {
            if (CalculateScore.ShouldMinimize)
            {
                bestEverScore = Double.MaxValue;
            }
            else
            {
                bestEverScore = Double.MinValue;
            }

            // check the population
            foreach (IGenome obj in Population.Genomes)
            {
                if (!(obj is NEATGenome))
                {
                    throw new TrainingError(
                        "Population can only contain objects of NEATGenome.");
                }

                var neat = (NEATGenome) obj;

                if ((neat.InputCount != inputCount)
                    || (neat.OutputCount != outputCount))
                {
                    throw new TrainingError(
                        "All NEATGenome's must have the same input and output sizes as the base network.");
                }
                neat.GA = this;
            }

            Population.Claim(this);

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <summary>
        /// Reset counts and kill genomes with worse scores.
        /// </summary>
        public void ResetAndKill()
        {
            totalFitAdjustment = 0;
            averageFitAdjustment = 0;

            var speciesArray = new ISpecies[Population.Species.Count];

            for (int i = 0; i < Population.Species.Count; i++)
            {
                speciesArray[i] = Population.Species[i];
            }

            foreach (Object element in speciesArray)
            {
                var s = (ISpecies) element;
                s.Purge();

                if ((s.GensNoImprovement > paramNumGensAllowedNoImprovement)
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
            foreach (IGenome g in Population.Genomes)
            {
                g.Decode();
                PerformCalculateScore(g);
            }

            Population.Sort();

            IGenome genome = Population.Best;
            double currentBest = genome.Score;

            if (Comparator.IsBetterThan(currentBest, bestEverScore))
            {
                bestEverScore = currentBest;
                bestEverNetwork = ((NEATNetwork) genome.Organism);
            }

            bestEverScore = Comparator.BestScore(Error,
                                                 bestEverScore);
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
                var genome = (NEATGenome) g;
                bool added = false;

                foreach (ISpecies s in Population.Species)
                {
                    double compatibility = genome.GetCompatibilityScore((NEATGenome) s.Leader);

                    if (compatibility <= paramCompatibilityThreshold)
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
                        new BasicSpecies(Population, genome,
                                         Population.AssignSpeciesID()));
                }
            }

            AdjustSpeciesScore();

            foreach (IGenome g in Population.Genomes)
            {
                var genome = (NEATGenome) g;
                totalFitAdjustment += genome.AdjustedScore;
            }

            averageFitAdjustment = totalFitAdjustment
                                   /Population.Size();

            foreach (IGenome g in Population.Genomes)
            {
                var genome = (NEATGenome) g;
                double toSpawn = genome.AdjustedScore
                                 /averageFitAdjustment;
                genome.AmountToSpawn = toSpawn;
            }

            foreach (ISpecies species in Population.Species)
            {
                species.CalculateSpawnAmount();
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

            int chosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                var thisTry = (int) RangeRandomizer.Randomize(0,Population.Size() - 1);

                if (Population.Get(thisTry).Score > bestScoreSoFar)
                {
                    chosenOne = thisTry;

                    bestScoreSoFar = Population.Get(thisTry).Score;
                }
            }

            return (NEATGenome) Population.Get(chosenOne);
        }
    }
}
