// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if logging
using log4net;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Concurrency.Job;

namespace Encog.Neural.Networks.Prune
{
    /// <summary>
    /// This class is used to help determine the optimal configuration for the hidden
    /// layers of a neural network. It can accept a pattern, which specifies the type
    /// of neural network to create, and a list of the maximum and minimum hidden
    /// layer neurons. It will then attempt to train the neural network at all
    /// configurations and see which hidden neuron counts work the best.
    /// </summary>
    public class PruneIncremental : ConcurrentJob
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PruneIncremental));
#endif
        private bool done = false;


        /// <summary>
        /// Format the network as a human readable string that lists the 
        /// hidden layers.
        /// </summary>
        /// <param name="network">The network to format.</param>
        /// <returns>A human readable string.</returns>
        public static String NetworkToString(BasicNetwork network)
        {
            StringBuilder result = new StringBuilder();
            int num = 1;

            ILayer layer = network.GetLayer(BasicNetwork.TAG_INPUT);

            // display only hidden layers
            while (layer.Next.Count > 0)
            {
                layer = layer.Next[0].ToLayer;

                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append("H");
                result.Append(num++);
                result.Append("=");
                result.Append(layer.NeuronCount);
            }

            return result.ToString();

        }

        /**
         * The training set to use as different neural networks are evaluated.
         */
        private INeuralDataSet training;

        /**
         * The pattern for which type of neural network we would like to create.
         */
        private INeuralNetworkPattern pattern;

        /**
         * The ranges for the hidden layers.
         */
        private IList<HiddenLayerParams> hidden =
            new List<HiddenLayerParams>();

        /**
         * The number if training iterations that should be tried for
         * each network.
         */
        private int iterations;

        /**
         * The best error rate found so far.
         */
        private double bestResult;

        /**
         * The best network found so far.
         */
        private BasicNetwork bestNetwork;

        /**
         * How many networks have been tried so far?
         */
        private int currentTry;

        /**
         * Keeps track of how many neurons in each hidden layer as training the
         * evaluation progresses.
         */
        private int[] hiddenCounts;

        /**
         * Construct an object to determine the optimal number of hidden layers and
         * neurons for the specified training data and pattern.
         * 
         * @param training
         *            The training data to use.
         * @param pattern
         *            The network pattern to use to solve this data.
         * @param iterations
         * 			  How many iterations to try per network.
         * @param report
         * 			  Object used to report status to.
         */
        public PruneIncremental(INeuralDataSet training,
                 INeuralNetworkPattern pattern, int iterations,
                 IStatusReportable report)
            : base(report)
        {
            this.training = training;
            this.pattern = pattern;
            this.iterations = iterations;
        }

        /**
         * Add a hidden layer's min and max. Call this once per hidden layer.
         * Specify a zero min if it is possible to remove this hidden layer.
         * 
         * @param min
         *            The minimum number of neurons for this layer.
         * @param max
         *            The maximum number of neurons for this layer.
         */
        public void AddHiddenLayer(int min, int max)
        {
            HiddenLayerParams param = new HiddenLayerParams(min, max);
            this.hidden.Add(param);
        }


        /**
         * Generate a network according to the current hidden layer counts.
         * @return The network based on current hidden layer counts.
         */
        private BasicNetwork GenerateNetwork()
        {
            this.pattern.Clear();

            foreach (int element in this.hiddenCounts)
            {
                if (element > 0)
                {
                    this.pattern.AddHiddenLayer(element);
                }
            }

            return this.pattern.Generate();
        }

        /**
         * @return The hidden layer max and min.
         */
        public IList<HiddenLayerParams> Hidden
        {
            get
            {
                return this.hidden;
            }
        }

        /**
         * @return The number of training iterations to try for each network.
         */
        public int Iterations
        {
            get
            {
                return this.iterations;
            }
        }

        /**
         * @return The network pattern to use.
         */
        public INeuralNetworkPattern Pattern
        {
            get
            {
                return this.pattern;
            }
        }

        /**
         * @return The training set to use.
         */
        public INeuralDataSet Training
        {
            get
            {
                return this.training;
            }
        }

        /**
         * Increase the hidden layer counts according to the hidden layer 
         * parameters.  Increase the first hidden layer count by one, if
         * it is maxed out, then set it to zero and increase the next 
         * hidden layer.
         * @return False if no more increases can be done, true otherwise.
         */
        private bool IncreaseHiddenCounts()
        {
            int i = 0;
            do
            {
                HiddenLayerParams param = this.hidden[i];
                this.hiddenCounts[i]++;

                // is this hidden layer still within the range?
                if (this.hiddenCounts[i] <= param.Max)
                {
                    return true;
                }

                // increase the next layer if we've maxed out this one
                this.hiddenCounts[i] = param.Min;
                i++;

            } while (i < this.hiddenCounts.Length);

            // can't increase anymore, we're done!

            return false;
        }


        /// <summary>
        /// Begin process and find a good network.
        /// </summary>
        public override void Process()
        {

            if (this.hidden.Count == 0)
            {
                String str = "To calculate the optimal hidden size, at least "
                       + "one hidden layer must be defined.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new EncogError(str);
            }

            this.hiddenCounts = new int[this.hidden.Count];

            // set the best network
            this.bestNetwork = null;
            this.bestResult = double.MaxValue;

            // set to minimums
            int i = 0;
            foreach (HiddenLayerParams parm in this.hidden)
            {
                this.hiddenCounts[i++] = parm.Min;
            }

            // make sure hidden layer 1 has at least one neuron
            if (this.hiddenCounts[0] == 0)
            {
                String str = "To calculate the optimal hidden size, at least "
                       + "one neuron must be the minimum for the first hidden layer.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new EncogError(str);

            }

            base.Process();
        }

        /// <summary>
        /// Load all workloads, calculate how many networks we will examine.
        /// </summary>
        /// <returns></returns>
        public override int LoadWorkload()
        {
            int result = 1;

            foreach (HiddenLayerParams param in this.hidden)
            {
                result *= (param.Max - param.Min) + 1;
            }

            return result;
        }

        /// <summary>
        /// Evaluate one network.
        /// </summary>
        /// <param name="context">The job context.</param>
        public override void PerformJobUnit(JobUnitContext context)
        {

            BasicNetwork network = (BasicNetwork)context.JobUnit;

            // train the neural network
            ITrain train = new ResilientPropagation(network, this.training);

            for (int i = 0; i < this.iterations; i++)
            {
                train.Iteration();
            }

            double error = train.Error;

            if ((error < this.bestResult) || (this.bestNetwork == null))
            {
#if logging
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug("Prune found new best network: error="
                            + error + ", network=" + network);
                }
#endif
                this.bestNetwork = network;
                this.bestResult = error;
            }
            this.currentTry++;

            this.ReportStatus(context,
                    "Current: " + PruneIncremental.NetworkToString(network)
                    + ", Best: "
                    + PruneIncremental.NetworkToString(this.bestNetwork));

        }

        /// <summary>
        /// Request the next task, the next network to try.
        /// </summary>
        /// <returns></returns>
        public override Object RequestNextTask()
        {
            if (done)
            {
                return null;
            }

            BasicNetwork network = GenerateNetwork();

            if (!IncreaseHiddenCounts())
                done = true;

            return network;
        }

        /// <summary>
        /// The best network found.
        /// </summary>
        public BasicNetwork BestNetwork
        {
            get
            {
                return bestNetwork;
            }
        }
    }
}
