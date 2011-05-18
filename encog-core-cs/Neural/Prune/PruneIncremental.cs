using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Buffer;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.Concurrency.Job;
using Encog.Util.Logging;

namespace Encog.Neural.Prune
{
    /// <summary>
    /// This class is used to help determine the optimal configuration for the hidden
    /// layers of a neural network. It can accept a pattern, which specifies the type
    /// of neural network to create, and a list of the maximum and minimum hidden
    /// layer neurons. It will then attempt to train the neural network at all
    /// configurations and see which hidden neuron counts work the best.
    /// This method does not simply choose the network with the lowest error rate. A
    /// specifiable number of best networks are kept, which represent the networks
    /// with the lowest error rates. From this collection of networks, the best
    /// network is defined to be the one with the fewest number of connections.
    /// Not all starting random weights are created equal. Because of this, an option
    /// is provided to allow you to choose how many attempts you want the process to
    /// make, with different weights. All random weights are created using the
    /// default Nguyen-Widrow method normally used by Encog.
    /// </summary>
    ///
    public class PruneIncremental : ConcurrentJob
    {
        /// <summary>
        /// The ranges for the hidden layers.
        /// </summary>
        ///
        private readonly IList<HiddenLayerParams> hidden;

        /// <summary>
        /// The number if training iterations that should be tried for each network.
        /// </summary>
        ///
        private readonly int iterations;

        /// <summary>
        /// The pattern for which type of neural network we would like to create.
        /// </summary>
        ///
        private readonly NeuralNetworkPattern pattern;

        /// <summary>
        /// The object that status should be reported to.
        /// </summary>
        ///
        private readonly IStatusReportable report;

        /// <summary>
        /// An array of the top errors.
        /// </summary>
        ///
        private readonly double[] topErrors;

        /// <summary>
        /// An array of the top networks.
        /// </summary>
        ///
        private readonly BasicNetwork[] topNetworks;

        /// <summary>
        /// The training set to use as different neural networks are evaluated.
        /// </summary>
        ///
        private readonly MLDataSet training;

        /// <summary>
        /// The number of tries with random weights.
        /// </summary>
        ///
        private readonly int weightTries;

        /// <summary>
        /// The best network found so far.
        /// </summary>
        ///
        private BasicNetwork bestNetwork;

        /// <summary>
        /// How many networks have been tried so far?
        /// </summary>
        ///
        private int currentTry;

        /// <summary>
        /// Are we done?
        /// </summary>
        ///
        private bool done;

        /// <summary>
        /// The size of the first hidden layer.
        /// </summary>
        ///
        private int hidden1Size;

        /// <summary>
        /// The size of the second hidden layer.
        /// </summary>
        ///
        private int hidden2Size;

        /// <summary>
        /// Keeps track of how many neurons in each hidden layer as training the
        /// evaluation progresses.
        /// </summary>
        ///
        private int[] hiddenCounts;

        /// <summary>
        /// The current highest error.
        /// </summary>
        ///
        private double high;

        /// <summary>
        /// The current lowest error.
        /// </summary>
        ///
        private double low;

        /// <summary>
        /// The results in a 2d array.
        /// </summary>
        ///
        private double[][] results;

        /// <summary>
        /// Construct an object to determine the optimal number of hidden layers and
        /// neurons for the specified training data and pattern.
        /// </summary>
        ///
        /// <param name="training_0">The training data to use.</param>
        /// <param name="pattern_1">The network pattern to use to solve this data.</param>
        /// <param name="iterations_2">How many iterations to try per network.</param>
        /// <param name="weightTries_3">The number of random weights to use.</param>
        /// <param name="numTopResults"></param>
        /// <param name="report_4">Object used to report status to.</param>
        public PruneIncremental(MLDataSet training_0,
                                NeuralNetworkPattern pattern_1, int iterations_2,
                                int weightTries_3, int numTopResults,
                                IStatusReportable report_4) : base(report_4)
        {
            done = false;
            hidden = new List<HiddenLayerParams>();
            training = training_0;
            pattern = pattern_1;
            iterations = iterations_2;
            report = report_4;
            weightTries = weightTries_3;
            topNetworks = new BasicNetwork[numTopResults];
            topErrors = new double[numTopResults];
        }


        /// <value>The network being processed.</value>
        public BasicNetwork BestNetwork
        {
            get { return bestNetwork; }
        }


        /// <value>The hidden layer max and min.</value>
        public IList<HiddenLayerParams> Hidden
        {
            get { return hidden; }
        }


        /// <value>The size of the first hidden layer.</value>
        public int Hidden1Size
        {
            get { return hidden1Size; }
        }


        /// <value>The size of the second hidden layer.</value>
        public int Hidden2Size
        {
            get { return hidden2Size; }
        }


        /// <value>The higest error so far.</value>
        public double High
        {
            get { return high; }
        }


        /// <value>The number of training iterations to try for each network.</value>
        public int Iterations
        {
            get { return iterations; }
        }


        /// <value>The lowest error so far.</value>
        public double Low
        {
            get { return low; }
        }


        /// <value>The network pattern to use.</value>
        public NeuralNetworkPattern Pattern
        {
            get { return pattern; }
        }


        /// <value>The error results.</value>
        public double[][] Results
        {
            get { return results; }
        }


        /// <value>the topErrors</value>
        public double[] TopErrors
        {
            get { return topErrors; }
        }


        /// <value>the topNetworks</value>
        public BasicNetwork[] TopNetworks
        {
            get { return topNetworks; }
        }


        /// <value>The training set to use.</value>
        public MLDataSet Training
        {
            get { return training; }
        }

        /// <summary>
        /// Format the network as a human readable string that lists the hidden
        /// layers.
        /// </summary>
        ///
        /// <param name="network">The network to format.</param>
        /// <returns>A human readable string.</returns>
        public static String NetworkToString(BasicNetwork network)
        {
            var result = new StringBuilder();
            int num = 1;

            // display only hidden layers
            for (int i = 1; i < network.LayerCount - 1; i++)
            {
                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append("H");
                result.Append(num++);
                result.Append("=");
                result.Append(network.GetLayerNeuronCount(i));
            }

            return result.ToString();
        }

        /// <summary>
        /// Add a hidden layer's min and max. Call this once per hidden layer.
        /// Specify a zero min if it is possible to remove this hidden layer.
        /// </summary>
        ///
        /// <param name="min">The minimum number of neurons for this layer.</param>
        /// <param name="max">The maximum number of neurons for this layer.</param>
        public void AddHiddenLayer(int min, int max)
        {
            var param = new HiddenLayerParams(min, max);
            hidden.Add(param);
        }

        /// <summary>
        /// Generate a network according to the current hidden layer counts.
        /// </summary>
        ///
        /// <returns>The network based on current hidden layer counts.</returns>
        private BasicNetwork GenerateNetwork()
        {
            pattern.Clear();


            foreach (int element  in  hiddenCounts)
            {
                if (element > 0)
                {
                    pattern.AddHiddenLayer(element);
                }
            }

            return (BasicNetwork) pattern.Generate();
        }


        /// <summary>
        /// Increase the hidden layer counts according to the hidden layer
        /// parameters. Increase the first hidden layer count by one, if it is maxed
        /// out, then set it to zero and increase the next hidden layer.
        /// </summary>
        ///
        /// <returns>False if no more increases can be done, true otherwise.</returns>
        private bool IncreaseHiddenCounts()
        {
            int i = 0;
            do
            {
                HiddenLayerParams param = hidden[i];
                hiddenCounts[i]++;

                // is this hidden layer still within the range?
                if (hiddenCounts[i] <= param.Max)
                {
                    return true;
                }

                // increase the next layer if we've maxed out this one
                hiddenCounts[i] = param.Min;
                i++;
            } while (i < hiddenCounts.Length);

            // can't increase anymore, we're done!

            return false;
        }

        /// <summary>
        /// Init for prune.
        /// </summary>
        ///
        public void Init()
        {
            // handle display for one layer
            if (hidden.Count == 1)
            {
                hidden1Size = (hidden[0].Max - hidden[0].Min) + 1;
                hidden2Size = 0;
                results = EngineArray.AllocateDouble2D(hidden1Size, 1);
            }
            else if (hidden.Count == 2)
            {
                // handle display for two layers
                hidden1Size = (hidden[0].Max - hidden[0].Min) + 1;
                hidden2Size = (hidden[1].Max - hidden[1].Min) + 1;
                results = EngineArray.AllocateDouble2D(hidden1Size, hidden2Size);
            }
            else
            {
                // we don't handle displays for more than two layers
                hidden1Size = 0;
                hidden2Size = 0;
                results = null;
            }

            // reset min and max
            high = Double.NegativeInfinity;
            low = Double.PositiveInfinity;
        }

        /// <summary>
        /// Get the next workload. This is the number of hidden neurons. This is the
        /// total amount of work to be processed.
        /// </summary>
        ///
        /// <returns>The amount of work to be processed by this.</returns>
        public override sealed int LoadWorkload()
        {
            int result = 1;


            foreach (HiddenLayerParams param  in  hidden)
            {
                result *= (param.Max - param.Min) + 1;
            }

            Init();

            return result;
        }

        /// <summary>
        /// Perform an individual job unit, which is a single network to train and
        /// evaluate.
        /// </summary>
        ///
        /// <param name="context">Contains information about the job unit.</param>
        public override sealed void PerformJobUnit(JobUnitContext context)
        {
            var network = (BasicNetwork) context.JobUnit;
            BufferedMLDataSet buffer = null;
            MLDataSet useTraining = training;

            if (training is BufferedMLDataSet)
            {
                buffer = (BufferedMLDataSet) training;
                useTraining = (buffer.OpenAdditional());
            }

            // train the neural network

            double error = Double.PositiveInfinity;
            for (int z = 0; z < weightTries; z++)
            {
                network.Reset();
                Propagation train = new ResilientPropagation(network,
                                                             useTraining);
                var strat = new StopTrainingStrategy(0.001d,
                                                     5);

                train.AddStrategy(strat);
                train.NumThreads = 1; // force single thread mode

                for (int i = 0;
                     (i < iterations) && !ShouldStop
                     && !strat.ShouldStop();
                     i++)
                {
                    train.Iteration();
                }

                error = Math.Min(error, train.Error);
            }

            if (buffer != null)
            {
                buffer.Close();
            }

            if (!ShouldStop)
            {
                // update min and max

                high = Math.Max(high, error);
                low = Math.Min(low, error);

                if (hidden1Size > 0)
                {
                    int networkHidden1Count;
                    int networkHidden2Count;

                    if (network.LayerCount > 3)
                    {
                        networkHidden2Count = network.GetLayerNeuronCount(1);
                        networkHidden1Count = network.GetLayerNeuronCount(2);
                    }
                    else
                    {
                        networkHidden2Count = 0;
                        networkHidden1Count = network.GetLayerNeuronCount(1);
                    }

                    int row, col;

                    if (hidden2Size == 0)
                    {
                        row = networkHidden1Count - hidden[0].Min;
                        col = 0;
                    }
                    else
                    {
                        row = networkHidden1Count - hidden[0].Min;
                        col = networkHidden2Count - hidden[1].Min;
                    }

                    if ((row < 0) || (col < 0))
                    {
                        Console.Out.WriteLine("STOP");
                    }
                    results[row][col] = error;
                }

                // report status
                currentTry++;

                UpdateBest(network, error);
                ReportStatus(
                    context,
                    "Current: "
                    + NetworkToString(network)
                    + "; Best: "
                    + NetworkToString(bestNetwork));
            }
        }

        /// <summary>
        /// Begin the prune process.
        /// </summary>
        ///
        public override sealed void Process()
        {
            if (hidden.Count == 0)
            {
                throw new EncogError(
                    "To calculate the optimal hidden size, at least "
                    + "one hidden layer must be defined.");
            }

            hiddenCounts = new int[hidden.Count];

            // set the best network
            bestNetwork = null;

            // set to minimums
            int i = 0;

            foreach (HiddenLayerParams parm  in  hidden)
            {
                hiddenCounts[i++] = parm.Min;
            }

            // make sure hidden layer 1 has at least one neuron
            if (hiddenCounts[0] == 0)
            {
                throw new EncogError(
                    "To calculate the optimal hidden size, at least "
                    + "one neuron must be the minimum for the first hidden layer.");
            }

            base.Process();
        }

        /// <summary>
        /// Request the next task. This is the next network to attempt to train.
        /// </summary>
        ///
        /// <returns>The next network to train.</returns>
        public override sealed Object RequestNextTask()
        {
            if (done || ShouldStop)
            {
                return null;
            }

            BasicNetwork network = GenerateNetwork();

            if (!IncreaseHiddenCounts())
            {
                done = true;
            }

            return network;
        }

        /// <summary>
        /// Update the best network.
        /// </summary>
        ///
        /// <param name="network">The network to consider.</param>
        /// <param name="error">The error for this network.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateBest(BasicNetwork network,
                                double error)
        {
            high = Math.Max(high, error);
            low = Math.Min(low, error);

            int selectedIndex = -1;

            // find a place for this in the top networks, if it is a top network
            for (int i = 0; i < topNetworks.Length; i++)
            {
                if (topNetworks[i] == null)
                {
                    selectedIndex = i;
                    break;
                }
                else if (topErrors[i] > error)
                {
                    // this network might be worth replacing, see if the one
                    // already selected is a better option.
                    if ((selectedIndex == -1)
                        || (topErrors[selectedIndex] < topErrors[i]))
                    {
                        selectedIndex = i;
                    }
                }
            }

            // replace the selected index
            if (selectedIndex != -1)
            {
                topErrors[selectedIndex] = error;
                topNetworks[selectedIndex] = network;
            }

            // now select the best network, which is the most simple of the
            // top networks.

            BasicNetwork choice = null;


            foreach (BasicNetwork n  in  topNetworks)
            {
                if (n == null)
                {
                    continue;
                }

                if (choice == null)
                {
                    choice = n;
                }
                else
                {
                    if (n.Structure.CalculateSize() < choice.Structure
                                                          .CalculateSize())
                    {
                        choice = n;
                    }
                }
            }

            if (choice != bestNetwork)
            {
                EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                 "Prune found new best network: error=" + error
                                 + ", network=" + choice);
            }
        }
    }
}