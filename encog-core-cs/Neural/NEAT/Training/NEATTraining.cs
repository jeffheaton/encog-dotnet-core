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
        /**
	 * The average fit adjustment.
	 */

        /**
	 * The number of inputs.
	 */
        private readonly int inputCount;

        /**
	 * The number of output neurons.
	 */
        private readonly int outputCount;
        private double averageFitAdjustment;
        private NEATNetwork bestEverNetwork;
        private double bestEverScore;
        private int iteration;

        /**
	 * The activation mutation rate.
	 */
        private double paramActivationMutationRate = 0.1;

        /**
	 * The likelyhood of adding a link.
	 */
        private double paramChanceAddLink = 0.07;

        /**
	 * The likelyhood of adding a node.
	 */
        private double paramChanceAddNode = 0.04;

        /**
	 * THe likelyhood of adding a recurrent link.
	 */
        private double paramChanceAddRecurrentLink = 0.05;

        /**
	 * The compatibility threshold for a species.
	 */
        private double paramCompatibilityThreshold = 0.26;

        /**
	 * The crossover rate.
	 */
        private double paramCrossoverRate = 0.7;

        /**
	 * The max activation perturbation.
	 */
        private double paramMaxActivationPerturbation = 0.1;

        /**
	 * The maximum number of species.
	 */
        private int paramMaxNumberOfSpecies;

        /**
	 * The maximum number of neurons.
	 */
        private double paramMaxPermittedNeurons = 100;

        /**
	 * The maximum weight perturbation.
	 */
        private double paramMaxWeightPerturbation = 0.5;

        /**
	 * The mutation rate.
	 */
        private double paramMutationRate = 0.2;

        /**
	 * The number of link add attempts.
	 */
        private int paramNumAddLinkAttempts = 5;

        /**
	 * The number of generations allowed with no improvement.
	 */
        private int paramNumGensAllowedNoImprovement = 15;

        /**
	 * The number of tries to find a looped link.
	 */
        private int paramNumTrysToFindLoopedLink = 5;

        /**
	 * The number of tries to find an old link.
	 */
        private int paramNumTrysToFindOldLink = 5;

        /**
	 * The probability that the weight will be totally replaced.
	 */
        private double paramProbabilityWeightReplaced = 0.1;

        /**
	 * The total fit adjustment.
	 */

        /**
	 * Determines if we are using snapshot mode.
	 */
        private bool snapshot;
        private double totalFitAdjustment;

        /**
	 * The iteration number.
	 */

        /**
	 * Construct a neat trainer with a new population. The new population is
	 * created from the specified parameters.
	 * 
	 * @param calculateScore
	 *            The score calculation object.
	 * @param inputCount
	 *            The input neuron count.
	 * @param outputCount
	 *            The output neuron count.
	 * @param populationSize
	 *            The population size.
	 */

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

        /**
	 * Construct neat training with an existing population.
	 * 
	 * @param calculateScore
	 *            The score object to use.
	 * @param population
	 *            The population to use.
	 */

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

        /**
	 * Add a neuron.
	 * 
	 * @param nodeID
	 *            The neuron id.
	 * @param vec
	 *            THe list of id's used.
	 */

        /**
	 * @return The innovations.
	 */

        public NEATInnovationList Innovations
        {
            get { return (NEATInnovationList) Population.Innovations; }
        }

        /**
	 * @return The input count.
	 */

        public int InputCount
        {
            get { return inputCount; }
        }

        /**
	 * @return The number of output neurons.
	 */

        public int OutputCount
        {
            get { return outputCount; }
        }

        /**
	 * Returns an empty list, strategies are not supported.
	 * 
	 * @return The strategies in use(none).
	 */


        /**
	 * Set the activation mutation rate.
	 * 
	 * @param paramActivationMutationRate
	 *            The mutation rate.
	 */

        public double ParamActivationMutationRate
        {
            get { return paramActivationMutationRate; }
            set { paramActivationMutationRate = value; }
        }


        /**
	 * Set the chance to add a link.
	 * 
	 * @param paramChanceAddLink
	 *            The chance to add a link.
	 */

        public double ParamChanceAddLink
        {
            get { return paramChanceAddLink; }
            set { paramChanceAddLink = value; }
        }


        /**
	 * Set the chance to add a node.
	 * 
	 * @param paramChanceAddNode
	 *            The chance to add a node.
	 */

        public double ParamChanceAddNode
        {
            get { return paramChanceAddNode; }
            set { paramChanceAddNode = value; }
        }

        /**
	 * Set the chance to add a recurrent link.
	 * 
	 * @param paramChanceAddRecurrentLink
	 *            The chance to add a recurrent link.
	 */

        public double ParamChanceAddRecurrentLink
        {
            get { return paramChanceAddRecurrentLink; }
            set { paramChanceAddRecurrentLink = value; }
        }


        /**
	 * Set the compatibility threshold for species.
	 * 
	 * @param paramCompatibilityThreshold
	 *            The threshold.
	 */

        public double ParamCompatibilityThreshold
        {
            get { return paramCompatibilityThreshold; }
            set { paramCompatibilityThreshold = value; }
        }


        /**
	 * Set the cross over rate.
	 * 
	 * @param paramCrossoverRate
	 *            The crossover rate.
	 */

        public double ParamCrossoverRate
        {
            get { return paramCrossoverRate; }
            set { paramCrossoverRate = value; }
        }


        /**
	 * Set the max activation perturbation.
	 * 
	 * @param paramMaxActivationPerturbation
	 *            The max perturbation.
	 */

        public double ParamMaxActivationPerturbation
        {
            get { return paramMaxActivationPerturbation; }
            set { paramMaxActivationPerturbation = value; }
        }

        /**
	 * Set the maximum number of species.
	 * 
	 * @param paramMaxNumberOfSpecies
	 *            The max number of species.
	 */

        public int ParamMaxNumberOfSpecies
        {
            get { return paramMaxNumberOfSpecies; }
            set { paramMaxNumberOfSpecies = value; }
        }

        /**
	 * Set the max permitted neurons.
	 * 
	 * @param paramMaxPermittedNeurons
	 *            The max permitted neurons.
	 */

        public double ParamMaxPermittedNeurons
        {
            get { return paramMaxPermittedNeurons; }
            set { paramMaxPermittedNeurons = value; }
        }

        /**
	 * Set the max weight perturbation.
	 * 
	 * @param paramMaxWeightPerturbation
	 *            The max weight perturbation.
	 */

        public double ParamMaxWeightPerturbation
        {
            get { return paramMaxWeightPerturbation; }
            set { paramMaxWeightPerturbation = value; }
        }

        /**
	 * Set the mutation rate.
	 * 
	 * @param paramMutationRate
	 *            The mutation rate.
	 */

        public double ParamMutationRate
        {
            get { return paramMutationRate; }
            set { paramMutationRate = value; }
        }

        /**
	 * Set the number of attempts to add a link.
	 * 
	 * @param paramNumAddLinkAttempts
	 *            The number of attempts to add a link.
	 */

        public int ParamNumAddLinkAttempts
        {
            get { return paramNumAddLinkAttempts; }
            set { paramNumAddLinkAttempts = value; }
        }

        /**
	 * Set the number of no-improvement generations allowed.
	 * 
	 * @param paramNumGensAllowedNoImprovement
	 *            The number of generations.
	 */

        public int ParamNumGensAllowedNoImprovement
        {
            get { return paramNumGensAllowedNoImprovement; }
            set { paramNumGensAllowedNoImprovement = value; }
        }

        /**
	 * Set the number of tries to create a looped link.
	 * 
	 * @param paramNumTrysToFindLoopedLink
	 *            Number of tries.
	 */

        public int ParamNumTrysToFindLoopedLink
        {
            get { return paramNumTrysToFindLoopedLink; }
            set { paramNumTrysToFindLoopedLink = value; }
        }


        /**
	 * Set the number of tries to try an old link.
	 * 
	 * @param paramNumTrysToFindOldLink
	 *            Number of tries.
	 */

        public int ParamNumTrysToFindOldLink
        {
            get { return paramNumTrysToFindOldLink; }
            set { paramNumTrysToFindOldLink = value; }
        }


        /**
	 * Set the probability to replace a weight.
	 * 
	 * @param paramProbabilityWeightReplaced
	 *            The probability.
	 */

        public double ParamProbabilityWeightReplaced
        {
            get { return paramProbabilityWeightReplaced; }
            set { paramProbabilityWeightReplaced = value; }
        }

        /**
	 * Set if we are using snapshot mode.
	 * 
	 * @param snapshot
	 *            True if we are using snapshot mode.
	 */

        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        #region MLTrain Members

        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        public bool CanContinue
        {
            get { return false; }
        }

        public void FinishTraining()
        {
        }

        /**
	 * return The error for the best genome.
	 */

        public double Error
        {
            get { return bestEverScore; }
            set { bestEverScore = value; }
        }

        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        public int IterationNumber
        {
            get { return iteration; }
            set { iteration = value; }
        }

        /**
	 * @return A network created for the best genome.
	 */

        public MLMethod Method
        {
            get { return bestEverNetwork; }
        }

        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }

        /**
	 * Returns null, does not use a training set, rather uses a score function.
	 * 
	 * @return null, not used.
	 */

        public MLDataSet Training
        {
            get { return null; }
        }

        public bool TrainingDone
        {
            get { return false; }
        }

        /**
	 * Perform one training iteration.
	 */

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

        /**
	 * Perform the specified number of training iterations. This is a basic
	 * implementation that just calls iteration the specified number of times.
	 * However, some training methods, particularly with the GPU, benefit
	 * greatly by calling with higher numbers than 1.
	 * 
	 * @param count
	 *            The number of training iterations.
	 */

        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        public TrainingContinuation Pause()
        {
            return null;
        }

        public void Resume(TrainingContinuation state)
        {
        }

        #endregion

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

        /**
	 * Adjust each species score.
	 */

        public void adjustSpeciesScore()
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

        public NEATGenome Crossover(NEATGenome mom, NEATGenome dad)
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
            babyGenome.GeneticAlgorithm = this;
            babyGenome.Population = Population;

            return babyGenome;
        }

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
                neat.GeneticAlgorithm = this;
            }

            Population.Claim(this);

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

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

        /**
	 * Sort the genomes.
	 */

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

        /**
	 * Determine the species.
	 */

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

            adjustSpeciesScore();

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

        /**
	 * Select a gene using a tournament.
	 * 
	 * @param numComparisons
	 *            The number of compares to do.
	 * @return The chosen genome.
	 */

        public NEATGenome TournamentSelection(int numComparisons)
        {
            double bestScoreSoFar = 0;

            int chosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                var thisTry = (int) RangeRandomizer.Randomize(0,
                                                              Population.Size() - 1);

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