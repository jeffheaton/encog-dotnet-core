/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.Util;
using Encog.Util.Concurrency;

namespace Encog.Engine.Network.Train.Prop
{

    using Encog.Engine;
    using Encog.Engine.Network.Flat;
    using Encog.Engine.Network.Train;
    using Encog.Engine.Network.Train.Gradient;
    using System;

    /// <summary>
    /// Train a flat network using multithreading, and GPU support.
    /// The training data must be indexable, it will be broken into groups for each
    /// thread to process.
    /// At the end of each iteration the training from each thread is aggregated back
    /// to the neural network.
    /// </summary>
    ///
    public abstract class TrainFlatNetworkProp : ITrainFlatNetwork
    {

        /// <summary>
        /// The number of threads to use.
        /// </summary>
        ///
        protected internal int numThreads;

        /// <summary>
        /// The gradients.
        /// </summary>
        ///
        protected internal double[] gradients;

        /// <summary>
        /// The last gradients, from the last training iteration.
        /// </summary>
        ///
        protected internal double[] lastGradient;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        protected internal readonly FlatNetwork network;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        protected internal readonly MLDataSet training;

        /// <summary>
        /// The network in indexable form.
        /// </summary>
        ///
        protected internal readonly MLDataSet indexable;

        /// <summary>
        /// The workers.
        /// </summary>
        ///
        protected internal IFlatGradientWorker[] workers;

        /// <summary>
        /// The total error. Used to take the average of.
        /// </summary>
        ///
        protected internal double totalError;

        /// <summary>
        /// The current error is the average error over all of the threads.
        /// </summary>
        ///
        protected internal double currentError;

        /// <summary>
        /// Reported exception from the threads.
        /// </summary>
        ///
        protected internal Exception reportedException;

        /// <summary>
        /// The iteration.
        /// </summary>
        ///
        protected internal int iteration;

        /// <summary>
        /// Train a flat network multithreaded.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        public TrainFlatNetworkProp(FlatNetwork network,
                MLDataSet training)
        {
            this.training = training;
            this.network = network;

            this.gradients = new double[this.network.Weights.Length];
            this.lastGradient = new double[this.network.Weights.Length];

            this.indexable = training;
            this.numThreads = 0;
            this.reportedException = null;
        }

        /// <summary>
        /// Calculatee the gradients.
        /// </summary>
        public virtual void CalculateGradients()
        {
            if (this.workers == null)
            {
                Init();
            }

            if( this.network.HasContext )
                this.workers[0].Network.ClearContext();

            this.totalError = 0;

            if (this.workers.Length > 1)
            {

                TaskGroup group = EngineConcurrency.Instance
                        .CreateTaskGroup();

                /* foreach */
                foreach (IFlatGradientWorker worker in this.workers)
                {
                    EngineConcurrency.Instance.ProcessTask(worker, group);
                }

                group.WaitForComplete();
            }
            else
            {
                this.workers[0].Run();
            }

            this.currentError = this.totalError / this.workers.Length;

        }

        /// <summary>
        /// Copy the contexts to keep them consistent with multithreaded training.
        /// </summary>
        ///
        private void CopyContexts()
        {
            // copy the contexts(layer outputO from each group to the next group
            for (int i = 0; i < (this.workers.Length - 1); i++)
            {
                double[] src = this.workers[i].Network.LayerOutput;
                double[] dst = this.workers[i + 1].Network.LayerOutput;
                EngineArray.ArrayCopy(src, dst);
            }

            // copy the contexts from the final group to the real network
            EngineArray.ArrayCopy(
                this.workers[this.workers.Length - 1].Network.LayerOutput, 
                this.network.LayerOutput);
        }

        /// <inheritdoc />
        public virtual void FinishTraining()
        {
            // nothing to do
        }

        /// <inheritdoc />
        public virtual double Error
        {
            get
            {
                return this.currentError;
            }
        }



        /// <returns>The gradients from the last iteration;</returns>
        public double[] LastGradient
        {
            get
            {
                return this.lastGradient;
            }
        }


        /// <inheritdoc />
        public virtual FlatNetwork Network
        {
            get
            {
                return this.network;
            }
        }


        /// <inheritdoc />
        public virtual int NumThreads
        {
            get
            {
                return this.numThreads;
            }
            set
            {
                this.numThreads = value;
            }
        }


        /// <inheritdoc />
        public virtual MLDataSet Training
        {
            get
            {
                return this.training;
            }
        }


        /// <inheritdoc />
        private void Init()
        {

            DetermineWorkload determine = new DetermineWorkload(
                    this.numThreads, (int)this.indexable.Count);

            this.workers = new IFlatGradientWorker[determine.ThreadCount];

            int index = 0;

            /* foreach */
            // handle CPU
            foreach (IntRange r in determine.CalculateWorkers())
            {
                this.workers[index++] = new GradientWorkerCPU(((FlatNetwork)this.network.Clone()),
                        this, this.indexable.OpenAdditional(), r.Low,
                        r.High);
            }
        }

        /// <inheritdoc />
        public virtual void Iteration()
        {
            this.iteration++;

            CalculateGradients();

            if (this.network.Limited)
            {
                LearnLimited();
            }
            else
            {
                Learn();
            }

            /* foreach */
            foreach (IFlatGradientWorker worker in this.workers)
            {
                EngineArray.ArrayCopy(this.network.Weights, 0,
                        worker.Weights, 0, this.network.Weights.Length);
            }

            if( this.network.HasContext )
                CopyContexts();

            if (this.reportedException != null)
            {
                throw (new EncogEngineError(this.reportedException));
            }
        }

        /// <summary>
        /// Apply and learn.
        /// </summary>
        ///
        protected internal void Learn()
        {
            double[] weights = this.network.Weights;
            for (int i = 0; i < this.gradients.Length; i++)
            {
                weights[i] += UpdateWeight(this.gradients, this.lastGradient, i);
                this.gradients[i] = 0;
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
            double limit = this.network.ConnectionLimit;
            double[] weights = this.network.Weights;
            for (int i = 0; i < this.gradients.Length; i++)
            {
                if (weights[i] < limit)
                {
                    weights[i] = 0;
                }
                else
                {
                    weights[i] += UpdateWeight(this.gradients, this.lastGradient, i);
                }
                this.gradients[i] = 0;
            }
        }

        /// <summary>
        /// Called by the worker threads to report the progress at each step.
        /// </summary>
        ///
        /// <param name="gradients">The gradients from that worker.</param>
        /// <param name="error">The error for that worker.</param>
        /// <param name="ex">The exception.</param>
        public void Report(double[] gradients, double error,
                Exception ex)
        {
            lock (this)
            {
                if (ex == null)
                {

                    for (int i = 0; i < gradients.Length; i++)
                    {
                        this.gradients[i] += gradients[i];
                    }
                    this.totalError += error;
                }
                else
                {
                    this.reportedException = ex;
                }
            }
        }

        /// <summary>
        /// Update a weight, the means by which weights are updated vary depending on
        /// the training.
        /// </summary>
        ///
        /// <param name="gradient">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The update value.</returns>
        public abstract double UpdateWeight(double[] gradient,
                double[] lastGradient, int index);

        /// <summary>
        /// Perform the specified number of training iterations. This is a basic implementation 
        /// that just calls iteration the specified number of times.  However, some training 
        /// methods, particularly with the GPU, benefit greatly by calling with higher numbers than 1.
        /// </summary>
        ///
        /// <param name="count">The number of training iterations.</param>
        public virtual void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc />
        public virtual int CurrentIteration
        {
            get
            {
                return this.iteration;
            }
            set
            {
                this.iteration = value;
            }
        }



    }
}
