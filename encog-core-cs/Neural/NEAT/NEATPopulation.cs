using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Population;
using Encog.ML;
using Encog.Util.Identity;
using Encog.Neural.NEAT.Training;
using Encog.ML.EA.Genome;
using Encog.Util.Obj;
using Encog.ML.EA.Codec;
using Encog.MathUtil.Randomize.Factory;
using Encog.ML.Data;
using Encog.Neural.HyperNEAT.Substrate;
using Encog.Engine.Network.Activation;
using Encog.Neural.HyperNEAT;
using Encog.ML.EA.Species;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// A population for a NEAT or HyperNEAT system. This population holds the
    /// genomes, substrate and other values for a NEAT or HyperNEAT network.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/
    /// Encog's NEAT implementation was drawn from the following three Journal
    /// Articles. For more complete BibTeX sources, see NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    [Serializable]
    public class NEATPopulation : BasicPopulation, IMLError, IMLRegression
    {
        /// <summary>
        /// The default survival rate.
        /// </summary>
        public const double DEFAULT_SURVIVAL_RATE = 0.2;

        /// <summary>
        /// The activation function to use.
        /// </summary>
        public const String PROPERTY_NEAT_ACTIVATION = "neatAct";

        /// <summary>
        /// Property tag for the population size.
        /// </summary>
        public const String PROPERTY_POPULATION_SIZE = "populationSize";

        /// <summary>
        /// Property tag for the survival rate.
        /// </summary>
        public const String PROPERTY_SURVIVAL_RATE = "survivalRate";

        /// <summary>
        /// Default number of activation cycles.
        /// </summary>
        public const int DEFAULT_CYCLES = 4;

        /// <summary>
        /// Property to hold the number of cycles.
        /// </summary>
        public const String PROPERTY_CYCLES = "cycles";

        /// <summary>
        /// Change the weight, do not allow the weight to go out of the weight range.
        /// </summary>
        /// <param name="w">The amount to change the weight by.</param>
        /// <param name="weightRange">Specify the weight range. The range is from -weightRange to
        /// +weightRange.</param>
        /// <returns>The new weight value.</returns>
        public static double ClampWeight(double w, double weightRange)
        {
            if (w < -weightRange)
            {
                return -weightRange;
            }
            else if (w > weightRange)
            {
                return weightRange;
            }
            else
            {
                return w;
            }
        }

        /// <summary>
        /// The number of activation cycles that the networks produced by this
        /// population will use.
        /// </summary>
        private int activationCycles = NEATPopulation.DEFAULT_CYCLES;

        /// <summary>
        /// Generate gene id's.
        /// </summary>
        private IGenerateID geneIDGenerate = new BasicGenerateID();

        /// <summary>
        /// Generate innovation id's.
        /// </summary>
        private IGenerateID innovationIDGenerate = new BasicGenerateID();

        /// <summary>
        /// A list of innovations, or null if this feature is not being used.
        /// </summary>
        public NEATInnovationList Innovations { get; set; }

        /// <summary>
        /// The weight range. Weights will be between -weight and +weight.
        /// </summary>
        public double WeightRange { get; set; }

        /// <summary>
        /// The best genome that we've currently decoded into the bestNetwork
        /// property. If this value changes to point to a new genome reference then
        /// the phenome will need to be recalculated.
        /// </summary>
        private IGenome cachedBestGenome;

        /// <summary>
        /// The best network. If the population is used as an MLMethod, then this
        /// network will represent.
        /// </summary>
        private NEATNetwork bestNetwork;

        /// <summary>
        /// The number of input units. All members of the population must agree with
        /// this number.
        /// </summary>
        public int InputCount { get; set; }

        /// <summary>
        /// The number of output units. All members of the population must agree with
        /// this number.
        /// </summary>
        public int OutputCount { get; set; }

        /// <summary>
        /// The survival rate.
        /// </summary>
        public double SurvivalRate { get; set; }

        /// <summary>
        /// The substrate, if this is a hyperneat network.
        /// </summary>
        public Substrate CurrentSubstrate { get; set; }

        /// <summary>
        /// The activation functions that we can choose from.
        /// </summary>
        private ChooseObject<IActivationFunction> activationFunctions = new ChooseObject<IActivationFunction>();

        /// <summary>
        /// The CODEC used to decode the NEAT genomes into networks. Different
        /// CODEC's are used for NEAT vs HyperNEAT.
        /// </summary>
        public IGeneticCODEC CODEC { get; set; }

        /// <summary>
        /// The initial connection density for the initial random population of
        /// genomes.
        /// </summary>
        public double InitialConnectionDensity { get; set; }

        /// <summary>
        /// A factory to create random number generators.
        /// </summary>
        private IRandomFactory RandomNumberFactory { get; set; }

        /// <summary>
        /// An empty constructor for serialization.
        /// </summary>
        public NEATPopulation()
        {
            SurvivalRate = NEATPopulation.DEFAULT_SURVIVAL_RATE;
            WeightRange = 5;
            InitialConnectionDensity = 0.1;
            RandomNumberFactory = EncogFramework.Instance
                .RandomFactory.FactorFactory();
        }

        /// <summary>
        /// Construct a starting NEAT population. This does not generate the initial
        /// random population of genomes.
        /// </summary>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATPopulation(int inputCount, int outputCount,
                int populationSize)
            : base(populationSize, null)
        {

            SurvivalRate = NEATPopulation.DEFAULT_SURVIVAL_RATE;
            WeightRange = 5;
            InitialConnectionDensity = 0.1;
            RandomNumberFactory = EncogFramework.Instance
                .RandomFactory.FactorFactory();

            InputCount = inputCount;
            OutputCount = outputCount;

            NEATActivationFunction = new ActivationSteepenedSigmoid();

            if (populationSize == 0)
            {
                throw new NeuralNetworkError(
                        "Population must have more than zero genomes.");
            }

        }

        /// <summary>
        /// Construct a starting HyperNEAT population. This does not generate the
        /// initial random population of genomes.
        /// </summary>
        /// <param name="theSubstrate">The substrate ID.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATPopulation(Substrate theSubstrate, int populationSize)
            : base(populationSize, new FactorHyperNEATGenome())
        {
            SurvivalRate = NEATPopulation.DEFAULT_SURVIVAL_RATE;
            WeightRange = 5;
            InitialConnectionDensity = 0.1;
            RandomNumberFactory = EncogFramework.Instance
                .RandomFactory.FactorFactory();

            CurrentSubstrate = theSubstrate;
            InputCount = 6;
            OutputCount = 2;
            HyperNEATGenome.BuildCPPNActivationFunctions(this.activationFunctions);
        }

        /// <summary>
        /// A newly generated gene id.
        /// </summary>
        /// <returns>A newly generated gene id.</returns>
        public long AssignGeneID()
        {
            return this.geneIDGenerate.Generate();
        }

        /// <summary>
        /// Assign an innovation id.
        /// </summary>
        /// <returns>A newly generated innovation id.</returns>
        public long AssignInnovationID()
        {
            return this.innovationIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {
            UpdateBestNetwork();
            return this.bestNetwork.CalculateError(data);
        }

        /// <inheritdoc/>
        public IMLData Compute(IMLData input)
        {
            UpdateBestNetwork();
            return this.bestNetwork.Compute(input);
        }

        /// <summary>
        /// The activation cycles.
        /// </summary>
        public int ActivationCycles
        {
            get
            {
                return this.activationCycles;
            }
            set
            {
                this.activationCycles = value;
            }
        }

        /// <summary>
        /// The activation functions.
        /// </summary>
        public ChooseObject<IActivationFunction> ActivationFunctions
        {
            get
            {
                return this.activationFunctions;
            }
        }


        /// <summary>
        /// Generate a gene id.
        /// </summary>
        /// <returns>The gene id.</returns>
        public IGenerateID GeneIDGenerate()
        {
            return this.geneIDGenerate;
        }

        /// <inheritdoc/>
        public INEATGenomeFactory GenomeFactory
        {
            get
            {
                return (INEATGenomeFactory)base.GenomeFactory;
            }
            set
            {
                base.GenomeFactory = value;
            }
        }


        /// <summary>
        /// Innovation id generator.
        /// </summary>
        public IGenerateID InnovationIDGenerate
        {
            get
            {
                return this.innovationIDGenerate;
            }
        }

        /// <summary>
        /// Returns true if this is a hyperneat population.
        /// </summary>
        public bool IsHyperNEAT
        {
            get
            {
                return CurrentSubstrate != null;
            }
        }

        /// <summary>
        /// Create an initial random population.
        /// </summary>
        public void Reset()
        {
            // create the genome factory
            if (IsHyperNEAT)
            {
                CODEC = new HyperNEATCODEC();
                GenomeFactory = new FactorHyperNEATGenome();
            }
            else
            {
                CODEC = new NEATCODEC();
                GenomeFactory = new FactorNEATGenome();
            }

            // create the new genomes
            Species.Clear();

            // reset counters
            GeneIDGenerate().CurrentID = 1;
            InnovationIDGenerate.CurrentID = 1;

            EncogRandom rnd = RandomNumberFactory.Factor();

            // create one default species
            BasicSpecies defaultSpecies = new BasicSpecies();
            defaultSpecies.Population = this;

            // create the initial population
            for (int i = 0; i < PopulationSize; i++)
            {
                NEATGenome genome = GenomeFactory.Factor(rnd, this,
                        InputCount, OutputCount,
                        InitialConnectionDensity);
                defaultSpecies.Add(genome);
            }
            defaultSpecies.Leader = defaultSpecies.Members[0];
            Species.Add(defaultSpecies);

            // create initial innovations
            Innovations = new NEATInnovationList(this);
        }

        /// <summary>
        /// Specify to use a single activation function. This is typically the case
        /// for NEAT, but not for HyperNEAT.
        /// </summary>
        public IActivationFunction NEATActivationFunction
        {
            set
            {
                this.activationFunctions.Clear();
                this.activationFunctions.Add(1.0, value);
                this.activationFunctions.FinalizeStructure();
            }
        }

        /// <summary>
        /// See if the best genome has changed, and decode a new best network, if
        /// needed.
        /// </summary>
        private void UpdateBestNetwork()
        {
            if (BestGenome != this.cachedBestGenome)
            {
                this.cachedBestGenome = BestGenome;
                this.bestNetwork = (NEATNetwork)CODEC.Decode(BestGenome);
            }
        }
    }
}
