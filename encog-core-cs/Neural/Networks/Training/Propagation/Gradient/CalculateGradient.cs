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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using System.Threading;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data.Basic;
using Encog.MathUtil;
using Encog.Util.Concurrency;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Propagation.Gradient
{
    /// <summary>
    /// This class is used to calculate the gradients for each of the weights and
    /// thresholds values in a neural network. It is used by the propagation training
    /// methods. This class must visit every training set element. Multithreading is
    /// used to process every training set element, however it requires an indexable
    /// training set to run in multithreaded mode. Multithreaded mode allows the
    /// training method to run much faster on a multicore machine.
    /// </summary>
    public class CalculateGradient
    {

        /// <summary>
        /// How many threads are being used to train the network.
        /// </summary>
        private int threadCount;

        /// <summary>
        /// The training set to be used. This must be an indexable training set to
        /// that it can be divided by the threads.
        /// </summary>
        private IIndexable indexed;

        /// <summary>
        /// The training set that we are using.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// True if context layers are present. If they are, special handling is
        /// required when multithreading.
        /// </summary>
        private bool hasContext;

        /// <summary>
        /// The workers to be used, one for each thread.
        /// </summary>
        private GradientWorker[] workers;

        /// <summary>
        /// The network being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The weights and thresholds being trained.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The gradients calculated for every weight and threshold.
        /// </summary>
        private double[] gradients;

        /// <summary>
        /// Determine the thread counts and workloads.
        /// </summary>
        private DetermineWorkload determine;

        /// <summary>
        /// The overall error.
        /// </summary>
        private double error;

        /// <summary>
        /// The number of training patterns.
        /// </summary>
        private int count;

        /// <summary>
        /// Construct the object using a network and training set. This constructor
        /// will use only a single thread. 
        /// </summary>
        /// <param name="network">The network to be used to calculate.</param>
        /// <param name="training">The training set to use.</param>
        public CalculateGradient(BasicNetwork network,
                 INeuralDataSet training)
            : this(network, training, 1)
        {
        }

        /// <summary>
        /// Construct the object for multithreaded use. The number of threads can be
        /// specified. 
        /// </summary>
        /// <param name="network">The network to use.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="threads">The number of threads. Specify one for single threaded.
        /// Specify zero to allow Encog to determine the best number of
        /// threads to use, based on how many processors this machine has.</param>
        public CalculateGradient(BasicNetwork network,
                 INeuralDataSet training, int threads)
        {
            this.training = training;
            this.network = network;

            if (!(this.training is IIndexable))
            {
                this.network = network;
                this.threadCount = threads;
            }
            else
            {
                this.indexed = (IIndexable)this.training;
                this.determine = new DetermineWorkload(threads, (int)this.indexed.Count);
                this.threadCount = this.determine.TotalWorkerCount;
            }

            // setup workers
            this.gradients = new double[network.Structure.CalculateSize()];

            if (this.threadCount == 1)
            {
                CreateWorkersSingleThreaded(training);
            }
            else
            {
                EncogConcurrency.Instance.MaxThreads = this.threadCount;
                if (!(training is IIndexable))
                {
                    throw new TrainingError(
                            "Must use indexable training set for multithreaded.");
                }

                CreateWorkersMultiThreaded((IIndexable)training);
            }

            this.hasContext = this.network.Structure.ContainsLayerType(
                    typeof(ContextLayer));
        }

        /// <summary>
        /// Aggregate the results from all of the threads.
        /// </summary>
        private void Aggregate()
        {
            for (int i = 0; i < this.gradients.Length; i++)
            {
                this.gradients[i] = 0;
                for (int j = 0; j < this.threadCount; j++)
                {
                    this.gradients[i] += this.workers[j].Errors[i];
                }
            }

            this.count = 0;
            for (int i = 0; i < this.threadCount; i++)
            {
                this.count += this.workers[i].Count;
            }
        }

        /// <summary>
        /// Calculate the gradients based on the specified weights.
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        public void Calculate(double[] weights)
        {
            this.weights = weights;

            if (this.threadCount == 1)
            {
                RunWorkersSingleThreaded();
            }
            else
            {
                RunWorkersMultiThreaded();
            }

            Aggregate();
            DetermineError();

            if (this.hasContext)
            {
                LinkContext();
            }

        }

        /// <summary>
        /// Create a new neural data pair object of the correct size for the neural
        /// network that is being trained. This object will be passed to the getPair
        /// method to allow the neural data pair objects to be copied to it. 
        /// </summary>
        /// <returns>A new neural data pair object.</returns>
        public INeuralDataPair CreatePair()
        {
            INeuralDataPair result;

            int idealSize = this.training.IdealSize;
            int inputSize = this.training.InputSize;

            if (idealSize > 0)
            {
                result = new BasicNeuralDataPair(new BasicNeuralData(inputSize),
                        new BasicNeuralData(idealSize));
            }
            else
            {
                result = new BasicNeuralDataPair(new BasicNeuralData(inputSize));
            }

            return result;
        }

        /// <summary>
        /// Create the worker threads for use in multithreaded training. 
        /// </summary>
        /// <param name="training">The training set to use.</param>
        private void CreateWorkersMultiThreaded(IIndexable training)
        {
            this.indexed = training;
            // setup the workers
            this.workers = new GradientWorker[this.threadCount];
            this.determine.CalculateWorkers();
            IList<IntRange> workloadRange = this.determine.CPURanges;

            int i = 0;
            foreach (IntRange range in workloadRange)
            {
                IIndexable trainingClone = this.indexed.OpenAdditional();
                this.workers[i++] = new GradientWorker(this, trainingClone, range.Low, range.High);
            }
        }

        /// <summary>
        /// Create a single worker to handle the single threaded mode.
        /// </summary>
        /// <param name="training">The training set to use.</param>
        private void CreateWorkersSingleThreaded(INeuralDataSet training)
        {
            // setup the workers
            this.workers = new GradientWorker[this.threadCount];
            this.workers[0] = new GradientWorker(this, training, 0, 0);
        }

        /// <summary>
        /// Determine the error.
        /// </summary>
        private void DetermineError()
        {
            double totalError = 0;
            for (int i = 0; i < this.threadCount; i++)
            {
                totalError += this.workers[i].Error;
            }
            this.error = (totalError / this.threadCount);
        }

        /// <summary>
        /// The training set count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// The current overall error.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error;
            }
        }

        /// <summary>
        /// The gradients.
        /// </summary>
        public double[] Gradients
        {
            get
            {
                return this.gradients;
            }
        }

        /// <summary>
        /// The network that is being trained.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// The weights and thresholds from the network that is being
        ///         trained.
        /// </summary>
        public double[] Weights
        {
            get
            {
                return this.weights;
            }
        }

        /// <summary>
        /// Link the context layers. This ensures that the workers pass on the state
        /// of their context layer to the next worker. Without this multithreaded
        /// training could not be used on recurrent neural networks.
        /// </summary>
        private void LinkContext()
        {
            IDictionary<ContextLayer, Object> workload = new Dictionary<ContextLayer, Object>();

            // first loop through and build a map of where every context should be
            // copied to
            for (int indexThisWorker = 0; indexThisWorker < this.workers.Length; indexThisWorker++)
            {
                GradientWorker thisWorker = this.workers[indexThisWorker];
                int indexNextWorker = indexThisWorker + 1;
                if (indexNextWorker == this.workers.Length)
                {
                    indexNextWorker = 0;
                }
                GradientWorker nextWorker = this.workers[indexNextWorker];

                Object[] thisLayers = thisWorker.Network.Structure
                    .Layers.ToArray();
                Object[] nextLayers = nextWorker.Network.Structure
                        .Layers.ToArray();

                for (int i = 0; i < thisLayers.Length; i++)
                {
                    ILayer thisLayer = (ILayer)thisLayers[i];
                    ILayer nextLayer = (ILayer)nextLayers[i];

                    if (thisLayer is ContextLayer)
                    {
                        ContextLayer thisContext = (ContextLayer)thisLayer;
                        ContextLayer nextContext = (ContextLayer)nextLayer;

                        double[] source = thisContext.Context.Data;
                        double[] target = new double[source.Length];
                        EncogArray.ArrayCopy(source, target);
                        workload[nextContext] = target;
                    }
                }
            }

            // now actually copy it
            foreach (ContextLayer layer in workload.Keys)
            {
                double[] source = (double[])workload[layer];
                double[] target = layer.Context.Data;
                EncogArray.ArrayCopy(source, target);
            }
        }

        /// <summary>
        /// Run all of the workers in a multithreaded way. This function will block
        /// until all threads are done.
        /// </summary>
        private void RunWorkersMultiThreaded()
        {
            TaskGroup group = EncogConcurrency.Instance.CreateTaskGroup();

            // start the workers
            for (int i = 0; i < this.threadCount; i++)
            {
                EncogConcurrency.Instance.ProcessTask(this.workers[i], group);
            }

            // wait for all workers to finish
            group.WaitForComplete();
        }

        /// <summary>
        /// Run the single worker for the single threaded mode.
        /// </summary>
        private void RunWorkersSingleThreaded()
        {
            this.workers[0].Run();
        }
    }
}
