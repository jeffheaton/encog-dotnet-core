using System;
using Encog.Engine.Network.Activation;
using Encog.ML.Genetic.Population;
using Encog.Neural.NEAT.Training;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// A population that is designed to be used with NEAT.
    /// </summary>
    [Serializable]
    public class NEATPopulation : BasicPopulation
    {
        /// <summary>
        /// NEAT activation function tag.
        /// </summary>
        public const String PROPERTY_NEAT_ACTIVATION = "neatAct";

        /// <summary>
        /// NEAT output activation function.
        /// </summary>
        public const String PROPERTY_OUTPUT_ACTIVATION = "outAct";

        /// <summary>
        /// The number of input units. All members of the population must agree with
        /// this number.
        /// </summary>
        ///
        internal int inputCount;

        /// <summary>
        /// The activation function for neat to use.
        /// </summary>
        ///
        private IActivationFunction neatActivationFunction;

        /// <summary>
        /// The activation function to use on the output layer of Encog.
        /// </summary>
        ///
        private IActivationFunction outputActivationFunction;

        /// <summary>
        /// The number of output units. All members of the population must agree with
        /// this number.
        /// </summary>
        ///
        internal int outputCount;

        /// <summary>
        /// Are we using snapshot?
        /// </summary>
        private bool snapshot;

        /// <summary>
        /// Construct a starting NEAT population.
        /// </summary>
        ///
        /// <param name="inputCount_0">The input neuron count.</param>
        /// <param name="outputCount_1">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATPopulation(int inputCount_0, int outputCount_1,
                              int populationSize) : base(populationSize)
        {
            neatActivationFunction = new ActivationSigmoid();
            outputActivationFunction = new ActivationLinear();
            inputCount = inputCount_0;
            outputCount = outputCount_1;

            if (populationSize == 0)
            {
                throw new NeuralNetworkError(
                    "Population must have more than zero genomes.");
            }

            // create the initial population
            for (int i = 0; i < populationSize; i++)
            {
                var genome = new NEATGenome(AssignGenomeID(), inputCount_0,
                                            outputCount_1);
                Add(genome);
            }

            // create initial innovations
            var genome_2 = (NEATGenome) Genomes[0];
            Innovations = new NEATInnovationList(this, genome_2.Links,
                                                 genome_2.Neurons);
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        public NEATPopulation()
        {
            neatActivationFunction = new ActivationSigmoid();
            outputActivationFunction = new ActivationLinear();
        }


        /// <value>the inputCount to set</value>
        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }


        /// <value>the outputCount to set</value>
        public int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }


        /// <value>the neatActivationFunction to set</value>
        public IActivationFunction NeatActivationFunction
        {
            get { return neatActivationFunction; }
            set { neatActivationFunction = value; }
        }


        /// <value>the outputActivationFunction to set</value>
        public IActivationFunction OutputActivationFunction
        {
            get { return outputActivationFunction; }
            set { outputActivationFunction = value; }
        }


        /// <value>the snapshot to set</value>
        public bool Snapshot
        {            
            get { return snapshot; }
            set { snapshot = value; }
        }
    }
}