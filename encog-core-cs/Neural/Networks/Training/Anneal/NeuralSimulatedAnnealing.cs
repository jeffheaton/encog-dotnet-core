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
using Encog.ML;
using Encog.ML.Train;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util.Logging;
using Encog.MathUtil;

namespace Encog.Neural.Networks.Training.Anneal
{
    /// <summary>
    /// This class implements a simulated annealing training algorithm for neural
    /// networks. It is based on the generic SimulatedAnnealing class. It is used in
    /// the same manner as any other training class that implements the Train
    /// interface. There are essentially two ways you can make use of this class.
    /// Either way, you will need a score object. The score object tells the
    /// simulated annealing algorithm how well suited a neural network is.
    /// If you would like to use simulated annealing with a training set you should
    /// make use TrainingSetScore class. This score object uses a training set to
    /// score your neural network.
    /// If you would like to be more abstract, and not use a training set, you can
    /// create your own implementation of the CalculateScore method. This class can
    /// then score the networks any way that you like.
    /// </summary>
    ///
    public class NeuralSimulatedAnnealing : BasicTraining
    {
        /// <summary>
        /// The cutoff for random data.
        /// </summary>
        ///
        public const double Cut = 0.5d;

        /// <summary>
        /// This class actually performs the training.
        /// </summary>
        ///
        private readonly NeuralSimulatedAnnealingHelper _anneal;

        /// <summary>
        /// Used to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateScore _calculateScore;

        /// <summary>
        /// The neural network that is to be trained.
        /// </summary>
        ///
        private readonly IMLEncodable _network;

        /// <summary>
        /// Construct a simulated annleaing trainer for a feedforward neural network.
        /// </summary>
        ///
        /// <param name="network">The neural network to be trained.</param>
        /// <param name="calculateScore">Used to calculate the score for a neural network.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles in a training iteration.</param>
        public NeuralSimulatedAnnealing(IMLEncodable network,
                                        ICalculateScore calculateScore, double startTemp,
                                        double stopTemp, int cycles) : base(TrainingImplementationType.Iterative)
        {
            _network = network;
            _calculateScore = calculateScore;
            _anneal = new NeuralSimulatedAnnealingHelper(this)
                          {
                              Temperature = startTemp,
                              StartTemperature = startTemp,
                              StopTemperature = stopTemp,
                              Cycles = cycles
                          };
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Get the network as an array of doubles.
        /// </summary>
        public double[] Array
        {
            get
            {
                return NetworkCODEC
                    .NetworkToArray(_network);
            }
        }


        /// <value>A copy of the annealing array.</value>
        public double[] ArrayCopy
        {
            get { return Array; }
        }


        /// <value>The object used to calculate the score.</value>
        public ICalculateScore CalculateScore
        {
            get { return _calculateScore; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <summary>
        /// Perform one iteration of simulated annealing.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            EncogLogging.Log(EncogLogging.LevelInfo,
                             "Performing Simulated Annealing iteration.");
            PreIteration();
            _anneal.Iteration();
            Error = _anneal.PerformCalculateScore();
            PostIteration();
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// Convert an array of doubles to the current best network.
        /// </summary>
        ///
        /// <param name="array">An array.</param>
        public void PutArray(double[] array)
        {
            NetworkCODEC.ArrayToNetwork(array,
                                        _network);
        }

        /// <summary>
        /// Randomize the weights and bias values. This function does most of the
        /// work of the class. Each call to this class will randomize the data
        /// according to the current temperature. The higher the temperature the more
        /// randomness.
        /// </summary>
        ///
        public void Randomize()
        {
            double[] array = NetworkCODEC
                .NetworkToArray(_network);

            for (int i = 0; i < array.Length; i++)
            {
                double add = Cut - ThreadSafeRandom.NextDouble();
                add /= _anneal.StartTemperature;
                add *= _anneal.Temperature;
                array[i] = array[i] + add;
            }

            NetworkCODEC.ArrayToNetwork(array,
                                        _network);
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
