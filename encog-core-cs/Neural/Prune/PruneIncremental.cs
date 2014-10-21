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
        private readonly IList<HiddenLayerParams> _hidden;

        /// <summary>
        /// The number if training iterations that should be tried for each network.
        /// </summary>
        ///
        private readonly int _iterations;

        /// <summary>
        /// The pattern for which type of neural network we would like to create.
        /// </summary>
        ///
        private readonly INeuralNetworkPattern _pattern;

        /// <summary>
        /// The object that status should be reported to.
        /// </summary>
        ///
        private readonly IStatusReportable _report;

        /// <summary>
        /// An array of the top errors.
        /// </summary>
        ///
        private readonly double[] _topErrors;

        /// <summary>
        /// An array of the top networks.
        /// </summary>
        ///
        private readonly BasicNetwork[] _topNetworks;

        /// <summary>
        /// The training set to use as different neural networks are evaluated.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The number of tries with random weights.
        /// </summary>
        ///
        private readonly int _weightTries;

        /// <summary>
        /// The best network found so far.
        /// </summary>
        ///
        private BasicNetwork _bestNetwork;

        /// <summary>
        /// How many networks have been tried so far?
        /// </summary>
        ///
        private int _currentTry;

        /// <summary>
        /// Are we done?
        /// </summary>
        ///
        private bool _done;

        /// <summary>
        /// The size of the first hidden layer.
        /// </summary>
        ///
        private int _hidden1Size;

        /// <summary>
        /// The size of the second hidden layer.
        /// </summary>
        ///
        private int _hidden2Size;

        /// <summary>
        /// Keeps track of how many neurons in each hidden layer as training the
        /// evaluation progresses.
        /// </summary>
        ///
        private int[] _hiddenCounts;

        /// <summary>
        /// The current highest error.
        /// </summary>
        ///
        private double _high;

        /// <summary>
        /// The current lowest error.
        /// </summary>
        ///
        private double _low;

        /// <summary>
        /// The results in a 2d array.
        /// </summary>
        ///
        private double[][] _results;

        /// <summary>
        /// Construct an object to determine the optimal number of hidden layers and
        /// neurons for the specified training data and pattern.
        /// </summary>
        ///
        /// <param name="training">The training data to use.</param>
        /// <param name="pattern">The network pattern to use to solve this data.</param>
        /// <param name="iterations">How many iterations to try per network.</param>
        /// <param name="weightTries">The number of random weights to use.</param>
        /// <param name="numTopResults"></param>
        /// <param name="report">Object used to report status to.</param>
        public PruneIncremental(IMLDataSet training,
                                INeuralNetworkPattern pattern, int iterations,
                                int weightTries, int numTopResults,
                                IStatusReportable report) : base(report)
        {
            _done = false;
            _hidden = new List<HiddenLayerParams>();
            _training = training;
            _pattern = pattern;
            _iterations = iterations;
            _report = report;
            _weightTries = weightTries;
            _topNetworks = new BasicNetwork[numTopResults];
            _topErrors = new double[numTopResults];
        }


        /// <value>The network being processed.</value>
        public BasicNetwork BestNetwork
        {
            get { return _bestNetwork; }
        }


        /// <value>The hidden layer max and min.</value>
        public IList<HiddenLayerParams> Hidden
        {
            get { return _hidden; }
        }


        /// <value>The size of the first hidden layer.</value>
        public int Hidden1Size
        {
            get { return _hidden1Size; }
        }


        /// <value>The size of the second hidden layer.</value>
        public int Hidden2Size
        {
            get { return _hidden2Size; }
        }


        /// <value>The higest error so far.</value>
        public double High
        {
            get { return _high; }
        }


        /// <value>The number of training iterations to try for each network.</value>
        public int Iterations
        {
            get { return _iterations; }
        }


        /// <value>The lowest error so far.</value>
        public double Low
        {
            get { return _low; }
        }


        /// <value>The network pattern to use.</value>
        public INeuralNetworkPattern Pattern
        {
            get { return _pattern; }
        }


        /// <value>The error results.</value>
        public double[][] Results
        {
            get { return _results; }
        }


        /// <value>the topErrors</value>
        public double[] TopErrors
        {
            get { return _topErrors; }
        }


        /// <value>the topNetworks</value>
        public BasicNetwork[] TopNetworks
        {
            get { return _topNetworks; }
        }


        /// <value>The training set to use.</value>
        public IMLDataSet Training
        {
            get { return _training; }
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
            if (network != null)
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
            else
            {
                return "N/A";
            }
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
            _hidden.Add(param);
        }

        /// <summary>
        /// Generate a network according to the current hidden layer counts.
        /// </summary>
        ///
        /// <returns>The network based on current hidden layer counts.</returns>
        private BasicNetwork GenerateNetwork()
        {
            lock(this)
            {
                _pattern.Clear();


                foreach (int element  in  _hiddenCounts)
                {
                    if (element > 0)
                    {
                        _pattern.AddHiddenLayer(element);
                    }
                }

                return (BasicNetwork) _pattern.Generate();
            }
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
            lock(this)
            {
                int i = 0;
                do
                {
                    HiddenLayerParams param = _hidden[i];
                    _hiddenCounts[i]++;

                    // is this hidden layer still within the range?
                    if (_hiddenCounts[i] <= param.Max)
                    {
                        return true;
                    }

                    // increase the next layer if we've maxed out this one
                    _hiddenCounts[i] = param.Min;
                    i++;
                } while (i < _hiddenCounts.Length);

                // can't increase anymore, we're done!

                return false;
            }
        }

        /// <summary>
        /// Init for prune.
        /// </summary>
        ///
        public void Init()
        {
            // handle display for one layer
            if (_hidden.Count == 1)
            {
                _hidden1Size = (_hidden[0].Max - _hidden[0].Min) + 1;
                _hidden2Size = 0;
                _results = EngineArray.AllocateDouble2D(_hidden1Size, 1);
            }
            else if (_hidden.Count == 2)
            {
                // handle display for two layers
                _hidden1Size = (_hidden[0].Max - _hidden[0].Min) + 1;
                _hidden2Size = (_hidden[1].Max - _hidden[1].Min) + 1;
                _results = EngineArray.AllocateDouble2D(_hidden1Size, _hidden2Size);
            }
            else
            {
                // we don't handle displays for more than two layers
                _hidden1Size = 0;
                _hidden2Size = 0;
                _results = null;
            }

            // reset min and max
            _high = Double.NegativeInfinity;
            _low = Double.PositiveInfinity;
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


            foreach (HiddenLayerParams param  in  _hidden)
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
            IMLDataSet useTraining = _training;

            if (_training is BufferedMLDataSet)
            {
                buffer = (BufferedMLDataSet) _training;
                useTraining = (buffer.OpenAdditional());
            }

            // train the neural network

            double error = Double.PositiveInfinity;
            for (int z = 0; z < _weightTries; z++)
            {
                network.Reset();
                Propagation train = new ResilientPropagation(network,
                                                             useTraining);
                var strat = new StopTrainingStrategy(0.001d,
                                                     5);

                train.AddStrategy(strat);
                train.ThreadCount = 1; // force single thread mode

                for (int i = 0;
                     (i < _iterations) && !ShouldStop
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

                _high = Math.Max(_high, error);
                _low = Math.Min(_low, error);

                if (_hidden1Size > 0)
                {
                    int networkHidden1Count;
                    int networkHidden2Count;

                    if (network.LayerCount > 3)
                    {
                        networkHidden2Count = network.GetLayerNeuronCount(2);
                        networkHidden1Count = network.GetLayerNeuronCount(1);
                    }
                    else
                    {
                        networkHidden2Count = 0;
                        networkHidden1Count = network.GetLayerNeuronCount(1);
                    }

                    int row, col;

                    if (_hidden2Size == 0)
                    {
                        row = networkHidden1Count - _hidden[0].Min;
                        col = 0;
                    }
                    else
                    {
                        row = networkHidden1Count - _hidden[0].Min;
                        col = networkHidden2Count - _hidden[1].Min;
                    }

                    if ((row < 0) || (col < 0))
                    {
                        Console.Out.WriteLine("STOP");
                    }
                    _results[row][col] = error;
                }

                // report status
                _currentTry++;

                UpdateBest(network, error);
                ReportStatus(
                    context,
                    "Current: "
                    + NetworkToString(network)
                    + "; Best: "
                    + NetworkToString(_bestNetwork));
            }
        }

        /// <summary>
        /// Begin the prune process.
        /// </summary>
        ///
        public override sealed void Process()
        {
            if (_hidden.Count == 0)
            {
                throw new EncogError(
                    "To calculate the optimal hidden size, at least "
                    + "one hidden layer must be defined.");
            }

            _hiddenCounts = new int[_hidden.Count];

            // set the best network
            _bestNetwork = null;

            // set to minimums
            int i = 0;

            foreach (HiddenLayerParams parm  in  _hidden)
            {
                _hiddenCounts[i++] = parm.Min;
            }

            // make sure hidden layer 1 has at least one neuron
            if (_hiddenCounts[0] == 0)
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
            if (_done || ShouldStop)
            {
                return null;
            }

            BasicNetwork network = GenerateNetwork();

            if (!IncreaseHiddenCounts())
            {
                _done = true;
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
            _high = Math.Max(_high, error);
            _low = Math.Min(_low, error);

            int selectedIndex = -1;

            // find a place for this in the top networks, if it is a top network
            for (int i = 0; i < _topNetworks.Length; i++)
            {
                if (_topNetworks[i] == null)
                {
                    selectedIndex = i;
                    break;
                }
                else if (_topErrors[i] > error)
                {
                    // this network might be worth replacing, see if the one
                    // already selected is a better option.
                    if ((selectedIndex == -1)
                        || (_topErrors[selectedIndex] < _topErrors[i]))
                    {
                        selectedIndex = i;
                    }
                }
            }

            // replace the selected index
            if (selectedIndex != -1)
            {
                _topErrors[selectedIndex] = error;
                _topNetworks[selectedIndex] = network;
            }

            // now select the best network, which is the most simple of the
            // top networks.

            BasicNetwork choice = null;


            foreach (BasicNetwork n  in  _topNetworks)
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

            if (choice != _bestNetwork)
            {
                _bestNetwork = choice;
                EncogLogging.Log(EncogLogging.LevelDebug,
                                 "Prune found new best network: error=" + error
                                 + ", network=" + choice);
            }
        }
    }
}
