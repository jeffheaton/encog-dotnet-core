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
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.Util;
using Encog.Util.Concurrency;
using Encog.Engine.Network.Activation;
using Encog.Neural.Error;

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
    public abstract class TrainFlatNetworkProp : ITrainFlatNetwork
    {
        /// <summary>
        /// The network in indexable form.
        /// </summary>
        ///
        private readonly IMLDataSet _indexable;

        /// <summary>
        /// The last gradients, from the last training iteration.
        /// </summary>
        ///
        private readonly double[] _lastGradient;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly FlatNetwork _network;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// The current error is the average error over all of the threads.
        /// </summary>
        ///
        protected internal double CurrentError;

        /// <summary>
        /// The gradients.
        /// </summary>
        ///
        protected internal double[] Gradients;

        /// <summary>
        /// The iteration.
        /// </summary>
        ///
        private int _iteration;

        /// <summary>
        /// The number of threads to use.
        /// </summary>
        ///
        private int _numThreads;

        /// <summary>
        /// Reported exception from the threads.
        /// </summary>
        ///
        private Exception _reportedException;

        /// <summary>
        /// The total error. Used to take the average of.
        /// </summary>
        ///
        private double _totalError;

        /// <summary>
        /// The workers.
        /// </summary>
        ///
        private GradientWorker[] _workers;

        /// <summary>
        /// True (default) if we should fix flatspots on supported activation functions.
        /// </summary>
        public bool FixFlatSpot { get; set; }

        /// <summary>
        /// The flat spot constants.
        /// </summary>
        private double[] _flatSpot;

        /// <summary>
        /// The error function.
        /// </summary>
        public IErrorFunction ErrorFunction { get; set; }



        /// <summary>
        /// Train a flat network multithreaded.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        protected TrainFlatNetworkProp(FlatNetwork network,
                                    IMLDataSet training)
        {
            _training = training;
            _network = network;

            Gradients = new double[_network.Weights.Length];
            _lastGradient = new double[_network.Weights.Length];

            _indexable = training;
            _numThreads = 0;
            _reportedException = null;
            FixFlatSpot = true;
            ErrorFunction = new LinearErrorFunction();
        }

        /// <value>The gradients from the last iteration;</value>
        public double[] LastGradient
        {
            get { return _lastGradient; }
        }

        #region TrainFlatNetwork Members

        /// <inheritdoc/>
        public virtual void FinishTraining()
        {
            // nothing to do
        }

        /// <inheritdoc/>
        public double Error
        {
            get { return CurrentError; }
        }


        /// <inheritdoc/>
        public int IterationNumber
        {
            get { return _iteration; }
            set { _iteration = value; }
        }


        /// <inheritdoc/>
        public FlatNetwork Network
        {
            get { return _network; }
        }


        /// <inheritdoc/>
        public int NumThreads
        {
            get { return _numThreads; }
            set { _numThreads = value; }
        }


        /// <inheritdoc/>
        public IMLDataSet Training
        {
            get { return _training; }
        }


        /// <inheritdoc/>
        public virtual void Iteration()
        {
            _iteration++;

            CalculateGradients();

            if (_network.Limited)
            {
                LearnLimited();
            }
            else
            {
                Learn();
            }


            foreach (GradientWorker worker in _workers)
            {
                EngineArray.ArrayCopy(_network.Weights, 0,
                                      worker.Weights, 0, _network.Weights.Length);
            }

            if (_network.HasContext)
            {
                CopyContexts();
            }

            if (_reportedException != null)
            {
                throw (new EncogError(_reportedException));
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
            if (_workers == null)
            {
                Init();
            }

            if (_network.HasContext)
            {
                _workers[0].Network.ClearContext();
            }

            _totalError = 0;

            if (_workers.Length > 1)
            {
                TaskGroup group = EngineConcurrency.Instance
                    .CreateTaskGroup();


                foreach (GradientWorker worker in _workers)
                {
                    EngineConcurrency.Instance.ProcessTask(worker, group);
                }

                group.WaitForComplete();
            }
            else
            {
                _workers[0].Run();
            }

            CurrentError = _totalError / _workers.Length;
        }

        /// <summary>
        /// Copy the contexts to keep them consistent with multithreaded training.
        /// </summary>
        ///
        private void CopyContexts()
        {
            // copy the contexts(layer outputO from each group to the next group
            for (int i = 0; i < (_workers.Length - 1); i++)
            {
                double[] src = _workers[i].Network.LayerOutput;
                double[] dst = _workers[i + 1].Network.LayerOutput;
                EngineArray.ArrayCopy(src, dst);
            }

            // copy the contexts from the final group to the real network
            EngineArray.ArrayCopy(_workers[_workers.Length - 1].Network.LayerOutput, _network.LayerOutput);
        }

        /// <summary>
        /// Init the process.
        /// </summary>
        ///
        private void Init()
        {
            // fix flat spot, if needed
            _flatSpot = new double[_network.ActivationFunctions.Length];

            if (FixFlatSpot)
            {
                for (int i = 0; i < _network.ActivationFunctions.Length; i++)
                {
                    IActivationFunction af = _network.ActivationFunctions[i];
                    if( af is ActivationSigmoid )
                    {
                        _flatSpot[i] = 0.1;
                    }
                    else
                    {
                        _flatSpot[i] = 0.0;
                    }
                }
            }
            else
            {
                EngineArray.Fill(_flatSpot, 0.0);
            }


            var determine = new DetermineWorkload(
                _numThreads, (int)_indexable.Count);

            _workers = new GradientWorker[determine.ThreadCount];

            int index = 0;


            // handle CPU
            foreach (IntRange r in determine.CalculateWorkers())
            {
                _workers[index++] = new GradientWorker(((FlatNetwork)_network.Clone()),
                                                         this, _indexable.OpenAdditional(), r.Low,
                                                         r.High, _flatSpot, ErrorFunction);
            }

            InitOthers();
        }

        /// <summary>
        /// Apply and learn.
        /// </summary>
        ///
        protected internal void Learn()
        {
            double[] weights = _network.Weights;
            for (int i = 0; i < Gradients.Length; i++)
            {
                weights[i] += UpdateWeight(Gradients, _lastGradient, i);
                Gradients[i] = 0;
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
            double limit = _network.ConnectionLimit;
            double[] weights = _network.Weights;
            for (int i = 0; i < Gradients.Length; i++)
            {
                if (Math.Abs(weights[i]) < limit)
                {
                    weights[i] = 0;
                }
                else
                {
                    weights[i] += UpdateWeight(Gradients, _lastGradient, i);
                }
                Gradients[i] = 0;
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
                        Gradients[i] += gradients[i];
                    }
                    _totalError += error;
                }
                else
                {
                    _reportedException = ex;
                }
            }
        }

        /// <summary>
        /// Update a weight, the means by which weights are updated vary depending on
        /// the training.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The update value.</returns>
        public abstract double UpdateWeight(double[] gradients,
                                            double[] lastGradient, int index);

        /// <summary>
        /// Allow other training methods to init.
        /// </summary>
        public abstract void InitOthers();
    }
}
