using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Layers;
using System.Threading;

namespace Encog.Neural.Networks.Training.Propagation.Multi
{
    /// <summary>
    /// MPROP - Multipropagation Training. This is a training technique being
    /// developed by Jeff Heaton. It is meant to be especially optimal for running on
    /// multicore and eventually grid computing systems.
    /// 
    /// MPROP does not currently suppor recurrent networks, this will be addressed in
    /// a later release.
    ///  - Jeff Heaton
    /// </summary>
    public class MultiPropagation : BasicTraining
    {
        /// <summary>
        /// How many threads are being used to train the network.
        /// </summary>
        private int threadCount;

        /// <summary>
        /// The workers to be used, one for each thread.
        /// </summary>
        private MPROPWorker[] workers;

        /// <summary>
        /// The training set to be used. This must be an indexable training set to
        /// that it can be divided by the threads.
        /// </summary>
        private IIndexable training;

        /// <summary>
        ///  The neural network to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// If it is not worthwhile to do MPROP, then we will fall back to using
        /// RPROP.
        /// </summary>
        private ResilientPropagation fallback;

        /// <summary>
        /// The RPROP method being used by the master network.
        /// </summary>
        private ResilientPropagationMethod method;

        /// <summary>
        /// The propagation utility being used by the master network. This is the
        /// master training data.
        /// </summary>
        private PropagationUtil propagationUtil;

        /// <summary>
        /// A map that allows gradients from the worker threads to be quickly copied
        /// to the master.
        /// </summary>
        private GradientMap map;

        /// <summary>
        /// Construct a MPROP trainer that will use the number of available
        /// processors plus 1. If there is only one processor, then threads will not
        /// be used and this trainer will fall back to RPROP.
        /// 
        /// Also make sure that there are not so many threads that the training set
        /// size per thread becomes two small.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        public MultiPropagation(BasicNetwork network,
                 INeuralDataSet training)
        {

            // must be using indexable training set
            if (!(training is IIndexable))
            {
                throw new TrainingError(
                        "Must use a training set that implements Indexable for multipropagation.");
            }
            this.training = (IIndexable)training;

            int threads = System.Environment.ProcessorCount;

            // if there is more than one processor, use processor count +1
            if (threads != 1)
            {
                threads++;
            }
            // if there is a single processor, just use one thread

            // Now see how big the training sets are going to be.
            // We want at least 100 training elements in each.
            // This method will likely be further "tuned" in future versions.

            long recordCount = this.training.Count;
            long workPerThread = recordCount / threads;

            if (workPerThread < 100)
            {
                threads = Math.Max(1, (int)(recordCount / 100));
            }

            Init(network, training, threads);

        }

        /// <summary>
        /// Construct a multi propagation trainer.
        /// </summary>
        /// <param name="network">The network to use.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="threadCount">The thread count to use.</param>
        public MultiPropagation(BasicNetwork network,
                 INeuralDataSet training, int threadCount)
        {
            Init(network, training, threadCount);
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
        /// The trained neural network. Make sure you call "finishTraining"
        /// before attempting to access the neural network. Otherwise you
        /// will end up with a reference to a network that is still being
        /// updated.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                if (this.fallback != null)
                {
                    return this.fallback.Network;
                }
                else
                {
                    return this.network;
                }
            }
        }

        /// <summary>
        /// The thread workers.
        /// </summary>
        public MPROPWorker[] Workers
        {
            get
            {
                return this.workers;
            }
        }

        /**
         * Construct a MPROP trainer using the specified number of threads. You can
         * also call a constructor that determines how many threads to use based on
         * the number of processors in the system.
         * 
         * @param network
         *            The network to train.
         * @param training
         *            The set to use.
         * @param threadCount
         *            The number of threads to use, must be 1 or higher.
         */
        private void Init(BasicNetwork network, INeuralDataSet training,
                 int threadCount)
        {

            // must be using indexable training set
            if (!(training is IIndexable))
            {
                throw new TrainingError(
                        "Must use a training set that implements Indexable for multipropagation.");
            }

            if (network.Structure.ContainsLayerType(typeof(ContextLayer)))
            {
                throw new TrainingError(
                        "Recurrent networks are not yet supported by MPROP.");
            }

            // store params
            this.threadCount = threadCount;
            this.training = (IIndexable)training;
            this.network = network;

            // create the master RPROP method and util

            this.method = new ResilientPropagationMethod(
                    ResilientPropagation.DEFAULT_ZERO_TOLERANCE,
                    ResilientPropagation.DEFAULT_MAX_STEP,
                    ResilientPropagation.DEFAULT_INITIAL_UPDATE);
            this.propagationUtil = new PropagationUtil(network, this.method);

            // setup the workers
            this.workers = new MPROPWorker[threadCount];

            long size = this.training.Count;
            long sizePerThread = size / this.threadCount;

            // should we fall back to RPROP?
            if ((threadCount == 1) || (sizePerThread < 1000))
            {
                this.fallback = new ResilientPropagation(network, training);
                return;
            }

            // create the workers
            for (int i = 0; i < this.threadCount; i++)
            {
                long low = i * sizePerThread;
                long high;

                // if this is the last record, then high to be the last item
                // in the training set.
                if (i == (this.threadCount - 1))
                {
                    high = size - 1;
                }
                else
                {
                    high = ((i + 1) * sizePerThread) - 1;
                }

                BasicNetwork networkClone = (BasicNetwork)this.network
                       .Clone();
                IIndexable trainingClone = this.training.OpenAdditional();
                this.workers[i] = new MPROPWorker(networkClone, trainingClone,
                        this, low, high);
            }

            // link the workers in a ring
            for (int i = 0; i < this.threadCount - 1; i++)
            {
                this.workers[i].Next = this.workers[i + 1];
            }
            this.workers[this.threadCount - 1].Next = this.workers[0];

            // build the gradient map
            this.map = new GradientMap(this.propagationUtil, this);
        }

        /// <summary>
        /// Perform one iteration of training. No work is actually done by this
        /// method, other than providing an indication of what the current error
        /// level is. The threads are already running in the background and going
        /// about their own iterations.
        /// </summary>
        public override void Iteration()
        {

            if (this.fallback != null)
            {
                this.fallback.Iteration();
                this.Error = this.fallback.Error;
                return;
            }

            Thread[] threadList = new Thread[this.workers.Length];

            // start the threads
            for (int i = 0; i < threadList.Length; i++)
            {
                //threadList[i] = new Thread(this.workers[i]);
                //threadList[i].start();
            }

            // wait for the threads to die
            double totalError = 0;
            for (int i = 0; i < threadList.Length; i++)
            {

                threadList[i].Join();

                totalError += this.workers[i].Error;
            }

            totalError /= this.workers.Length;
            this.Error = totalError;

            this.map.Collect();
            this.propagationUtil.Method.Learn();

        }
    }
}
