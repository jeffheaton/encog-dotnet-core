using System;
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.Neural.Flat.Train.Gradient;
using Encog.Util;
using Encog.Util.Concurrency;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train a flat network using multithreading, and GPU support.
    /// The training data must be indexable, it will be broken into groups for each
    /// thread to process.
    /// At the end of each iteration the training from each thread is aggregated back
    /// to the neural network.
    /// </summary>
    ///
    public abstract class TrainFlatNetworkProp : TrainFlatNetwork
    {
        /// <summary>
        /// The network in indexable form.
        /// </summary>
        ///
        private readonly MLDataSet indexable;

        /// <summary>
        /// The last gradients, from the last training iteration.
        /// </summary>
        ///
        private readonly double[] lastGradient;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        protected internal readonly FlatNetwork network;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly MLDataSet training;

        /// <summary>
        /// The current error is the average error over all of the threads.
        /// </summary>
        ///
        protected internal double currentError;

        /// <summary>
        /// The gradients.
        /// </summary>
        ///
        protected internal double[] gradients;

        /// <summary>
        /// The iteration.
        /// </summary>
        ///
        private int iteration;

        /// <summary>
        /// The number of threads to use.
        /// </summary>
        ///
        private int numThreads;

        /// <summary>
        /// Reported exception from the threads.
        /// </summary>
        ///
        private Exception reportedException;

        /// <summary>
        /// The total error. Used to take the average of.
        /// </summary>
        ///
        private double totalError;

        /// <summary>
        /// The workers.
        /// </summary>
        ///
        private FlatGradientWorker[] workers;

        /// <summary>
        /// Train a flat network multithreaded.
        /// </summary>
        ///
        /// <param name="network_0">The network to train.</param>
        /// <param name="training_1">The training data to use.</param>
        public TrainFlatNetworkProp(FlatNetwork network_0,
                                    MLDataSet training_1)
        {
            training = training_1;
            network = network_0;

            gradients = new double[network.Weights.Length];
            lastGradient = new double[network.Weights.Length];

            indexable = training_1;
            numThreads = 0;
            reportedException = null;
        }

        /// <value>The gradients from the last iteration;</value>
        public double[] LastGradient
        {
            /// <returns>The gradients from the last iteration;</returns>
            get { return lastGradient; }
        }

        #region TrainFlatNetwork Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void FinishTraining()
        {
            // nothing to do
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public double Error
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return currentError; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public int IterationNumber
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return iteration; }
            /// <summary>
            /// 
            /// </summary>
            ///
            set { iteration = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public FlatNetwork Network
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return network; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public int NumThreads
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return numThreads; }
            /// <summary>
            /// 
            /// </summary>
            ///
            set { numThreads = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public MLDataSet Training
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return training; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Iteration()
        {
            iteration++;

            CalculateGradients();

            if (network.Limited)
            {
                LearnLimited();
            }
            else
            {
                Learn();
            }


            foreach (FlatGradientWorker worker  in  workers)
            {
                EngineArray.ArrayCopy(network.Weights, 0,
                                      worker.Weights, 0, network.Weights.Length);
            }

            if (network.HasContext)
            {
                CopyContexts();
            }

            if (reportedException != null)
            {
                throw (new EncogError(reportedException));
            }
        }

        /// <summary>
        /// Perform the specified number of training iterations. This is a basic
        /// implementation that just calls iteration the specified number of times.
        /// However, some training methods, particularly with the GPU, benefit
        /// greatly by calling with higher numbers than 1.
        /// </summary>
        ///
        /// <param name="count">The number of training iterations.</param>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        #endregion

        /// <summary>
        /// Calculate the gradients.
        /// </summary>
        ///
        public virtual void CalculateGradients()
        {
            if (workers == null)
            {
                Init();
            }

            if (network.HasContext)
            {
                workers[0].Network.ClearContext();
            }

            totalError = 0;

            if (workers.Length > 1)
            {
                TaskGroup group = EngineConcurrency.Instance
                    .CreateTaskGroup();


                foreach (FlatGradientWorker worker  in  workers)
                {
                    EngineConcurrency.Instance.ProcessTask(worker, group);
                }

                group.WaitForComplete();
            }
            else
            {
                workers[0].Run();
            }

            currentError = totalError/workers.Length;
        }

        /// <summary>
        /// Copy the contexts to keep them consistent with multithreaded training.
        /// </summary>
        ///
        private void CopyContexts()
        {
            // copy the contexts(layer outputO from each group to the next group
            for (int i = 0; i < (workers.Length - 1); i++)
            {
                double[] src = workers[i].Network.LayerOutput;
                double[] dst = workers[i + 1].Network.LayerOutput;
                EngineArray.ArrayCopy(src, dst);
            }

            // copy the contexts from the final group to the real network
            EngineArray.ArrayCopy(workers[workers.Length - 1].Network.LayerOutput, network.LayerOutput);
        }

        /// <summary>
        /// Init the process.
        /// </summary>
        ///
        private void Init()
        {
            var determine = new DetermineWorkload(
                numThreads, (int) indexable.Count);

            workers = new FlatGradientWorker[determine.ThreadCount];

            int index = 0;


            // handle CPU
            foreach (IntRange r  in  determine.CalculateWorkers())
            {
                workers[index++] = new GradientWorkerCPU(((FlatNetwork) network.Clone()),
                                                         this, indexable.OpenAdditional(), r.Low,
                                                         r.High);
            }
        }

        /// <summary>
        /// Apply and learn.
        /// </summary>
        ///
        protected internal void Learn()
        {
            double[] weights = network.Weights;
            for (int i = 0; i < gradients.Length; i++)
            {
                weights[i] += UpdateWeight(gradients, lastGradient, i);
                gradients[i] = 0;
            }
        }

        /// <summary>
        /// Apply and learn. This is the same as learn, but it checks to see if any
        /// of the weights are below the limit threshold. In this case, these weights
        /// are zeroed out. Having two methods allows the regular learn method, which
        /// is what is usually use, to be as fast as possible.
        /// </summary>
        ///
        protected internal void LearnLimited()
        {
            double limit = network.ConnectionLimit;
            double[] weights = network.Weights;
            for (int i = 0; i < gradients.Length; i++)
            {
                if (Math.Abs(weights[i]) < limit)
                {
                    weights[i] = 0;
                }
                else
                {
                    weights[i] += UpdateWeight(gradients, lastGradient, i);
                }
                gradients[i] = 0;
            }
        }

        /// <summary>
        /// Called by the worker threads to report the progress at each step.
        /// </summary>
        ///
        /// <param name="gradients_0">The gradients from that worker.</param>
        /// <param name="error">The error for that worker.</param>
        /// <param name="ex">The exception.</param>
        public void Report(double[] gradients_0, double error,
                           Exception ex)
        {
            lock (this)
            {
                if (ex == null)
                {
                    for (int i = 0; i < gradients_0.Length; i++)
                    {
                        gradients[i] += gradients_0[i];
                    }
                    totalError += error;
                }
                else
                {
                    reportedException = ex;
                }
            }
        }

        /// <summary>
        /// Update a weight, the means by which weights are updated vary depending on
        /// the training.
        /// </summary>
        ///
        /// <param name="gradients_0">The gradients.</param>
        /// <param name="lastGradient_1">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The update value.</returns>
        public abstract double UpdateWeight(double[] gradients_0,
                                            double[] lastGradient_1, int index);
    }
}