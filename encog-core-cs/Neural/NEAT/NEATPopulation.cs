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
    /// A population for a NEAT or HyperNEAT system.  population holds the
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
    /// </summary>
    [Serializable]
    public class NEATPopulation : BasicPopulation, IMLError, IMLRegression
    {
        /// <summary>
        /// The default survival rate.
        /// </summary>
        public const double DefaultSurvivalRate = 0.2;

        /// <summary>
        /// The activation function to use.
        /// </summary>
        public const String PropertyNEATActivation = "neatAct";

        /// <summary>
        /// Property tag for the population size.
        /// </summary>
        public const String PropertyPopulationSize = "populationSize";

        /// <summary>
        /// Property tag for the survival rate.
        /// </summary>
        public const String PropertySurvivalRate = "survivalRate";

        /// <summary>
        /// Default number of activation cycles.
        /// </summary>
        public const int DefaultCycles = 4;

        /// <summary>
        /// Property to hold the number of cycles.
        /// </summary>
        public const String PropertyCycles = "cycles";

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
            if (w > weightRange)
            {
                return weightRange;
            }
            return w;
        }

        /// <summary>
        /// The number of activation cycles that the networks produced by 
        /// population will use.
        /// </summary>
        private int _activationCycles = NEATPopulation.DefaultCycles;

        /// <summary>
        /// Generate gene id's.
        /// </summary>
        private readonly IGenerateID _geneIdGenerate = new BasicGenerateID();

        /// <summary>
        /// Generate innovation id's.
        /// </summary>
        private readonly IGenerateID _innovationIdGenerate = new BasicGenerateID();

        /// <summary>
        /// A list of innovations, or null if  feature is not being used.
        /// </summary>
        public NEATInnovationList Innovations { get; set; }

        /// <summary>
        /// The weight range. Weights will be between -weight and +weight.
        /// </summary>
        public double WeightRange { get; set; }

        /// <summary>
        /// The best genome that we've currently decoded into the bestNetwork
        /// property. If  value changes to point to a new genome reference then
        /// the phenome will need to be recalculated.
        /// </summary>
        private IGenome _cachedBestGenome;

        /// <summary>
        /// The best network. If the population is used as an MLMethod, then 
        /// network will represent.
        /// </summary>
        private NEATNetwork _bestNetwork;

        /// <summary>
        /// The number of input units. All members of the population must agree with
        ///  number.
        /// </summary>
        public int InputCount { get; set; }

        /// <summary>
        /// The number of output units. All members of the population must agree with
        ///  number.
        /// </summary>
        public int OutputCount { get; set; }

        /// <summary>
        /// The survival rate.
        /// </summary>
        public double SurvivalRate { get; set; }

        /// <summary>
        /// The substrate, if  is a hyperneat network.
        /// </summary>
        public Substrate CurrentSubstrate { get; set; }

        /// <summary>
        /// The activation functions that we can choose from.
        /// </summary>
        private readonly ChooseObject<IActivationFunction> _activationFunctions = new ChooseObject<IActivationFunction>();

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
            SurvivalRate = DefaultSurvivalRate;
            WeightRange = 5;
            InitialConnectionDensity = 0.1;
            RandomNumberFactory = EncogFramework.Instance
                .RandomFactory.FactorFactory();
        }

        /// <summary>
        /// Construct a starting NEAT population.  does not generate the initial
        /// random population of genomes.
        /// </summary>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATPopulation(int inputCount, int outputCount,
                int populationSize)
            : base(populationSize, null)
        {

            SurvivalRate = DefaultSurvivalRate;
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
        /// Construct a starting HyperNEAT population.  does not generate the
        /// initial random population of genomes.
        /// </summary>
        /// <param name="theSubstrate">The substrate ID.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATPopulation(Substrate theSubstrate, int populationSize)
            : base(populationSize, new FactorHyperNEATGenome())
        {
            SurvivalRate = DefaultSurvivalRate;
            WeightRange = 5;
            InitialConnectionDensity = 0.1;
            RandomNumberFactory = EncogFramework.Instance
                .RandomFactory.FactorFactory();

            CurrentSubstrate = theSubstrate;
            InputCount = 6;
            OutputCount = 2;
            HyperNEATGenome.BuildCPPNActivationFunctions(_activationFunctions);
        }

        /// <summary>
        /// A newly generated gene id.
        /// </summary>
        /// <returns>A newly generated gene id.</returns>
        public long AssignGeneId()
        {
            return _geneIdGenerate.Generate();
        }

        /// <summary>
        /// Assign an innovation id.
        /// </summary>
        /// <returns>A newly generated innovation id.</returns>
        public long AssignInnovationId()
        {
            return _innovationIdGenerate.Generate();
        }

        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {
            UpdateBestNetwork();
            return _bestNetwork.CalculateError(data);
        }

        /// <inheritdoc/>
        public IMLData Compute(IMLData input)
        {
            UpdateBestNetwork();
            return _bestNetwork.Compute(input);
        }

        /// <summary>
        /// The activation cycles.
        /// </summary>
        public int ActivationCycles
        {
            get
            {
                return _activationCycles;
            }
            set
            {
                _activationCycles = value;
            }
        }

        /// <summary>
        /// The activation functions.
        /// </summary>
        public ChooseObject<IActivationFunction> ActivationFunctions
        {
            get
            {
                return _activationFunctions;
            }
        }


        /// <summary>
        /// Generate a gene id.
        /// </summary>
        public IGenerateID GeneIdGenerate
        {
            get
            {
                return _geneIdGenerate;
            }
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
                return _innovationIdGenerate;
            }
        }

        /// <summary>
        /// Returns true if  is a hyperneat population.
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
            GeneIdGenerate.CurrentID = 1;
            InnovationIDGenerate.CurrentID = 1;

            EncogRandom rnd = RandomNumberFactory.Factor();

            // create one default species
            BasicSpecies defaultSpecies = new BasicSpecies();
            defaultSpecies.Population = this;

            // create the initial population
            for (int i = 0; i < PopulationSize; i++)
            {
                NEATGenome genome = GenomeFactory.Factor(rnd, this ,
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
        /// Specify to use a single activation function.  is typically the case
        /// for NEAT, but not for HyperNEAT.
        /// </summary>
        public IActivationFunction NEATActivationFunction
        {
            set
            {
                _activationFunctions.Clear();
                _activationFunctions.Add(1.0, value);
                _activationFunctions.FinalizeStructure();
            }
        }

        /// <summary>
        /// See if the best genome has changed, and decode a new best network, if
        /// needed.
        /// </summary>
        private void UpdateBestNetwork()
        {
            if (BestGenome != _cachedBestGenome)
            {
                _cachedBestGenome = BestGenome;
                _bestNetwork = (NEATNetwork)CODEC.Decode(BestGenome);
            }
        }
    }
}
