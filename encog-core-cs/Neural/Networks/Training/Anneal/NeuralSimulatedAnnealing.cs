using System;
using Encog.ML;
using Encog.ML.Train;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util.Logging;

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
        public const double CUT = 0.5d;

        /// <summary>
        /// This class actually performs the training.
        /// </summary>
        ///
        private readonly NeuralSimulatedAnnealingHelper anneal;

        /// <summary>
        /// Used to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateScore calculateScore;

        /// <summary>
        /// The neural network that is to be trained.
        /// </summary>
        ///
        private readonly BasicNetwork network;

        /// <summary>
        /// Construct a simulated annleaing trainer for a feedforward neural network.
        /// </summary>
        ///
        /// <param name="network_0">The neural network to be trained.</param>
        /// <param name="calculateScore_1">Used to calculate the score for a neural network.</param>
        /// <param name="startTemp">The starting temperature.</param>
        /// <param name="stopTemp">The ending temperature.</param>
        /// <param name="cycles">The number of cycles in a training iteration.</param>
        public NeuralSimulatedAnnealing(BasicNetwork network_0,
                                        ICalculateScore calculateScore_1, double startTemp,
                                        double stopTemp, int cycles) : base(TrainingImplementationType.Iterative)
        {
            network = network_0;
            calculateScore = calculateScore_1;
            anneal = new NeuralSimulatedAnnealingHelper(this);
            anneal.Temperature = startTemp;
            anneal.StartTemperature = startTemp;
            anneal.StopTemperature = stopTemp;
            anneal.Cycles = cycles;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Get the network as an array of doubles.
        /// </summary>
        ///
        /// <value>The network as an array of doubles.</value>
        public double[] Array
        {
            /// <summary>
            /// Get the network as an array of doubles.
            /// </summary>
            ///
            /// <returns>The network as an array of doubles.</returns>
            get
            {
                return NetworkCODEC
                    .NetworkToArray(network);
            }
        }


        /// <value>A copy of the annealing array.</value>
        public double[] ArrayCopy
        {
            /// <returns>A copy of the annealing array.</returns>
            get { return Array; }
        }


        /// <value>The object used to calculate the score.</value>
        public ICalculateScore CalculateScore
        {
            /// <returns>The object used to calculate the score.</returns>
            get { return calculateScore; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override MLMethod Method
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return network; }
        }


        /// <summary>
        /// Perform one iteration of simulated annealing.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            EncogLogging.Log(EncogLogging.LEVEL_INFO,
                             "Performing Simulated Annealing iteration.");
            PreIteration();
            anneal.Iteration();
            Error = anneal.PerformCalculateScore();
            PostIteration();
        }

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
                                        network);
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
                .NetworkToArray(network);

            for (int i = 0; i < array.Length; i++)
            {
                double add = CUT - (new Random()).Next();
                add /= anneal.StartTemperature;
                add *= anneal.Temperature;
                array[i] = array[i] + add;
            }

            NetworkCODEC.ArrayToNetwork(array,
                                        network);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}