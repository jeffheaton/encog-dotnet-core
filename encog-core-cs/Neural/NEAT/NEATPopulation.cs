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
        public const String PropertyNEATActivation = "neatAct";

        /// <summary>
        /// NEAT output activation function.
        /// </summary>
        public const String PropertyOutputActivation = "outAct";

        /// <summary>
        /// The activation function for neat to use.
        /// </summary>
        ///
        private IActivationFunction _neatActivationFunction;

        /// <summary>
        /// The activation function to use on the output layer of Encog.
        /// </summary>
        ///
        private IActivationFunction _outputActivationFunction;

        /// <summary>
        /// Are we using snapshot?
        /// </summary>
        private bool _snapshot;

        /// <summary>
        /// Construct a starting NEAT population.
        /// </summary>
        ///
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public NEATPopulation(int inputCount, int outputCount,
                              int populationSize) : base(populationSize)
        {
            _neatActivationFunction = new ActivationSigmoid();
            _outputActivationFunction = new ActivationLinear();
            InputCount = inputCount;
            OutputCount = outputCount;

            if (populationSize == 0)
            {
                throw new NeuralNetworkError(
                    "Population must have more than zero genomes.");
            }

            // create the initial population
            for (int i = 0; i < populationSize; i++)
            {
                var genome = new NEATGenome(AssignGenomeID(), inputCount,
                                            outputCount);
                Add(genome);
            }

            // create initial innovations
            var genome2 = (NEATGenome) Genomes[0];
            Innovations = new NEATInnovationList(this, genome2.Links,
                                                 genome2.Neurons);
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        public NEATPopulation()
        {
            _neatActivationFunction = new ActivationSigmoid();
            _outputActivationFunction = new ActivationLinear();
        }


        /// <value>the inputCount to set</value>
        public int InputCount { get; set; }


        /// <value>the outputCount to set</value>
        public int OutputCount { get; set; }


        /// <value>the neatActivationFunction to set</value>
        public IActivationFunction NeatActivationFunction
        {
            get { return _neatActivationFunction; }
            set { _neatActivationFunction = value; }
        }


        /// <value>the outputActivationFunction to set</value>
        public IActivationFunction OutputActivationFunction
        {
            get { return _outputActivationFunction; }
            set { _outputActivationFunction = value; }
        }


        /// <value>the snapshot to set</value>
        public bool Snapshot
        {            
            get { return _snapshot; }
            set { _snapshot = value; }
        }
    }
}
