using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Score;
using Encog.ML.EA.Sort;
using Encog.ML.EA.Population;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Opp;
using Encog.ML.EA.Codec;
using Encog.ML.EA.Species;
using Encog.ML.EA.Rules;
using System.Threading.Tasks;
using Encog.MathUtil.Randomize.Factory;

namespace Encog.ML.EA.Train
{
    /// <summary>
    /// Provides a basic implementation of a multi-threaded Evolutionary Algorithm.
    /// The EA works from a score function.
    /// </summary>
    [Serializable]
    public class BasicEA : IEvolutionaryAlgorithm, IMultiThreadable
    {
        /// <summary>
        /// Calculate the score adjustment, based on adjusters.
        /// </summary>
        /// <param name="genome">The genome to adjust.</param>
        /// <param name="adjusters">The score adjusters.</param>
        public static void CalculateScoreAdjustment(IGenome genome,
                IList<IAdjustScore> adjusters)
        {
            double score = genome.Score;
            double delta = 0;

            foreach (IAdjustScore a in adjusters)
            {
                delta += a.CalculateAdjustment(genome);
            }

            genome.AdjustedScore = (score + delta);
        }

        /// <summary>
        /// Should exceptions be ignored.
        /// </summary>
        public bool IgnoreExceptions { get; set; }

        /// <summary>
        /// The genome comparator.
        /// </summary>
        public IGenomeComparer BestComparator { get; set; }

        /// <summary>
        /// The genome comparator.
        /// </summary>
        public IGenomeComparer SelectionComparator { get; set; }

        /// <summary>
        /// The population.
        /// </summary>
        public IPopulation population { get; set; }

        /// <summary>
        /// The score calculation function.
        /// </summary>
        public ICalculateScore ScoreFunction { get; set; }

        /// <summary>
        /// The selection operator.
        /// </summary>
        public ISelectionOperator Selection { get; set; }

        /// <summary>
        /// The score adjusters.
        /// </summary>
        private IList<IAdjustScore> adjusters = new List<IAdjustScore>();

        /// <summary>
        /// The operators to use.
        /// </summary>
        private OperationList operators = new OperationList();

        /// <summary>
        /// The CODEC to use to convert between genome and phenome.
        /// </summary>
        public IGeneticCODEC CODEC { get; set; }

        /// <summary>
        /// The current iteration.
        /// </summary>
        public int CurrentIteration { get; set; }

        /// <summary>
        /// Random number factory.
        /// </summary>
        public IRandomFactory RandomNumberFactory { get; set; }

        /// <summary>
        /// The validation mode.
        /// </summary>
        public bool ValidationMode { get; set; }

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The desired thread count.
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// The speciation method.
        /// </summary>
        private ISpeciation speciation = new SingleSpeciation();

        /// <summary>
        /// The best genome from the last iteration.
        /// </summary>
        private IGenome oldBestGenome;

        /// <summary>
        /// The population for the next iteration.
        /// </summary>
        private IList<IGenome> newPopulation = new List<IGenome>();

        /// <summary>
        /// The mutation to be used on the top genome. We want to only modify its
        /// weights.
        /// </summary>
        public IEvolutionaryOperator ChampMutation { get; set; }

        /// <summary>
        /// The percentage of a species that is "elite" and is passed on directly.
        /// </summary>
        public double EliteRate { get; set; }

        /// <summary>
        /// The number of times to try certian operations, so an endless loop does
        /// not occur.
        /// </summary>
        public int MaxTries { get; set; }

        /// <summary>
        /// The best ever genome.
        /// </summary>
        public IGenome BestGenome { get; set; }

        /// <summary>
        /// Holds rewrite and constraint rules.
        /// </summary>
        public IRuleHolder Rules { get; set; }

        /// <summary>
        /// The maximum number of errors to tolerate for the operators before stopping.
        /// Because this is a stocastic process some operators will generated errors sometimes.
        /// </summary>
        public int MaxOperationErrors { get; set; }

        /// <summary>
        /// Has the first iteration occured.
        /// </summary>
        private bool initialized;


        /// <summary>
        /// Construct an EA.
        /// </summary>
        /// <param name="thePopulation">The population.</param>
        /// <param name="theScoreFunction">The score function.</param>
        public BasicEA(IPopulation thePopulation,
                ICalculateScore theScoreFunction)
        {

            RandomNumberFactory = EncogFramework.Instance.RandomFactory.FactorFactory();
            EliteRate = 0.3;
            MaxTries = 5;
            MaxOperationErrors = 500;
            CODEC = new GenomeAsPhenomeCODEC();


            this.population = thePopulation;
            ScoreFunction = theScoreFunction;
            Selection = new TournamentSelection(this, 4);
            Rules = new BasicRuleHolder();

            // set the score compare method
            if (theScoreFunction.ShouldMinimize)
            {
                SelectionComparator = new MinimizeAdjustedScoreComp();
                BestComparator = new MinimizeScoreComp();
            }
            else
            {
                SelectionComparator = new MaximizeAdjustedScoreComp();
                BestComparator = new MaximizeScoreComp();
            }

            // set the iteration
            foreach (ISpecies species in thePopulation.Species)
            {
                foreach (IGenome genome in species.Members)
                {
                    CurrentIteration = Math.Max(CurrentIteration,
                            genome.BirthGeneration);
                }
            }
        }

        /// <summary>
        /// Add a child to the next iteration.
        /// </summary>
        /// <param name="genome">The child.</param>
        /// <returns>True, if the child was added successfully.</returns>
        public bool AddChild(IGenome genome)
        {
            lock (this.newPopulation)
            {
                if (this.newPopulation.Count < this.population.PopulationSize)
                {
                    // don't readd the old best genome, it was already added
                    if (genome != this.oldBestGenome)
                    {

                        if (ValidationMode)
                        {
                            if (this.newPopulation.Contains(genome))
                            {
                                throw new EncogError(
                                        "Genome already added to population: "
                                                + genome.ToString());
                            }
                        }

                        this.newPopulation.Add(genome);
                    }

                    if (!Double.IsInfinity(genome.Score)
                            && !Double.IsNaN(genome.Score)
                            && BestComparator.IsBetterThan(genome,
                                    BestGenome))
                    {
                        BestGenome = genome;
                        this.population.BestGenome = BestGenome;
                    }
                    return true;
                }
                else
                {
                    if (ValidationMode)
                    {
                        throw new EncogError("Population overflow");
                    }
                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public void AddOperation(double probability,
                IEvolutionaryOperator opp)
        {
            Operators.Add(probability, opp);
            opp.Init(this);
        }

        /// <inheritdoc/>
        public void AddScoreAdjuster(IAdjustScore scoreAdjust)
        {
            this.adjusters.Add(scoreAdjust);
        }

        /// <inheritdoc/>
        public void CalculateScore(IGenome g)
        {

            // try rewrite
            Rules.Rewrite(g);

            // decode
            IMLMethod phenotype = CODEC.Decode(g);
            double score;

            // deal with invalid decode
            if (phenotype == null)
            {
                if (BestComparator.ShouldMinimize)
                {
                    score = Double.PositiveInfinity;
                }
                else
                {
                    score = Double.NegativeInfinity;
                }
            }
            else
            {
                if (phenotype is IMLContext)
                {
                    ((IMLContext)phenotype).ClearContext();
                }
                score = ScoreFunction.CalculateScore(phenotype);
            }

            // now set the scores
            g.Score = score;
            g.AdjustedScore = score;
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {

        }

        /// <inheritdoc/>
        public double Error
        {
            get
            {
                if (BestGenome != null)
                {
                    return BestGenome.Score;
                }
                else
                {
                    if (ScoreFunction.ShouldMinimize)
                    {
                        return double.PositiveInfinity;
                    }
                    else
                    {
                        return double.NegativeInfinity;
                    }
                }
            }
        }


        /// <summary>
        /// The old best genome.
        /// </summary>
        public IGenome OldBestGenome
        {
            get
            {
                return this.oldBestGenome;
            }
        }

        /// <inheritdoc/>
        public void Iteration()
        {
            if (!this.initialized)
            {
                PreIteration();
            }

            if (Population.Species.Count == 0)
            {
                throw new EncogError("Population is empty, there are no species.");
            }

            CurrentIteration++;

            // Clear new population to just best genome.
            this.newPopulation.Clear();
            this.newPopulation.Add(BestGenome);
            this.oldBestGenome = BestGenome;


            // execute species in parallel
            IList<EAWorker> threadList = new List<EAWorker>();
            foreach (ISpecies species in Population.Species)
            {
                int numToSpawn = species.OffspringCount;

                // Add elite genomes directly
                if (species.Members.Count > 5)
                {
                    int idealEliteCount = (int)(species.Members.Count * EliteRate);
                    int eliteCount = Math.Min(numToSpawn, idealEliteCount);
                    for (int i = 0; i < eliteCount; i++)
                    {
                        IGenome eliteGenome = species.Members[i];
                        if (this.oldBestGenome != eliteGenome)
                        {
                            numToSpawn--;
                            if (!AddChild(eliteGenome))
                            {
                                break;
                            }
                        }
                    }
                }

                // now add one task for each offspring that each species is allowed
                while (numToSpawn-- > 0)
                {
                    EAWorker worker = new EAWorker(this, species);
                    threadList.Add(worker);
                }
            }

            // run all threads and wait for them to finish
            Parallel.ForEach(threadList, currentTask =>
            {
                currentTask.PerformTask();
            });

            // validate, if requested
            if (ValidationMode)
            {
                int currentPopSize = this.newPopulation.Count;
                int targetPopSize = Population.PopulationSize;
                if (currentPopSize != targetPopSize)
                {
                    throw new EncogError("Population size of " + currentPopSize
                            + " is outside of the target size of " + targetPopSize);
                }

                if (this.oldBestGenome != null
                        && !this.newPopulation.Contains(this.oldBestGenome))
                {
                    throw new EncogError(
                            "The top genome died, this should never happen!!");
                }

                if (BestGenome != null
                        && this.oldBestGenome != null
                        && BestComparator.IsBetterThan(this.oldBestGenome,
                                BestGenome))
                {
                    throw new EncogError(
                            "The best genome's score got worse, this should never happen!! Went from "
                                    + this.oldBestGenome.Score + " to "
                                    + this.BestGenome.Score);
                }
            }

            this.speciation.PerformSpeciation(this.newPopulation);
        }

        /// <summary>
        /// Called before the first iteration. Determine the number of threads to
        /// use.
        /// </summary>
        private void PreIteration()
        {
            this.initialized = true;
            this.speciation.Init(this);

            // just pick the first genome with a valid score as best, it will be
            // updated later.
            // also most populations are sorted this way after training finishes
            // (for reload)
            // if there is an empty population, the constructor would have blow
            IList<IGenome> list = Population.Flatten();

            int idx = 0;
            do
            {
                BestGenome = list[idx++];
            } while (idx < list.Count
                    && (Double.IsInfinity(BestGenome.Score) || Double
                            .IsNaN(this.BestGenome.Score)));

            Population.BestGenome = BestGenome;

            // speciate
            IList<IGenome> genomes = Population.Flatten();
            this.speciation.PerformSpeciation(genomes);

        }

        /// <summary>
        /// The population.
        /// </summary>
        public IPopulation Population
        {
            get
            {
                return this.population;
            }
        }

        /// <summary>
        /// The operators.
        /// </summary>
        public OperationList Operators
        {
            get
            {
                return this.operators;
            }
        }



        public IGenomeComparer BestComparer { get; set; }


        public int MaxIndividualSize
        {
            get { throw new NotImplementedException(); }
        }

        IPopulation IEvolutionaryAlgorithm.Population
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IList<IAdjustScore> ScoreAdjusters
        {
            get { return this.adjusters; }
        }

        public IGenomeComparer SelectionComparer { get; set; }

        public bool ShouldIgnoreExceptions { get; set; }

        public ISpeciation Speciation
        {
            get
            {
                return this.speciation;
            }
            set
            {
                this.speciation = value;
            }
        }
    }
}
