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
using System.Threading.Tasks;
using Encog.Engine.Network.Activation;
using Encog.MathUtil;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Flat;
using Encog.Util;
using Encog.Util.Logging;
using Encog.Neural.Error;
using Encog.Util.Concurrency;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Implements basic functionality that is needed by each of the propagation
    /// methods. The specifics of each of the propagation methods is implemented
    /// inside of the PropagationMethod interface implementors.
    /// </summary>
    ///
    public abstract class Propagation : BasicTraining, ITrain, IMultiThreadable, IBatchSize
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
        private IContainsFlat _network;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly FlatNetwork _flat;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

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
        /// Construct a propagation object.
        /// </summary>
        ///
        /// <param name="network">The network.</param>
        /// <param name="training">The training set.</param>
        protected Propagation(IContainsFlat network, IMLDataSet training) : base(TrainingImplementationType.Iterative)
        {
            _network = network;
            _flat = network.Flat;
            _training = training;

            Gradients = new double[_flat.Weights.Length];
            _lastGradient = new double[_flat.Weights.Length];

            _indexable = training;
            _numThreads = 0;
            _reportedException = null;
            FixFlatSpot = true;
            ErrorFunction = new LinearErrorFunction();
        }

        /// <summary>
        /// Set the number of threads. Specify zero to tell Encog to automatically
        /// determine the best number of threads for the processor. If OpenCL is used
        /// as the target device, then this value is not used.
        /// </summary>
        public int ThreadCount
        {
            get { return _numThreads; }
            set { _numThreads = value; }
        }

        /// <summary>
        /// Increase the iteration count by one.
        /// </summary>
        public void RollIteration()
        {
            _iteration++;
        }

        #region Train Members



        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        /// Perform the specified number of training iterations. This can be more
        /// efficient than single training iterations. This is particularly true if
        /// you are training with a GPU.
        /// </summary>        
        public override void Iteration()
        {
            try
            {
                PreIteration();

                RollIteration();

                if (BatchSize == 0)
                {
                    ProcessPureBatch();
                }
                else
                {
                    ProcessBatches();
                }


                foreach (GradientWorker worker in _workers)
                {
                    EngineArray.ArrayCopy(_flat.Weights, 0,
                                          worker.Weights, 0, _flat.Weights.Length);
                }

                if (_flat.HasContext)
                {
                    CopyContexts();
                }

                if (_reportedException != null)
                {
                    throw (new EncogError(_reportedException));
                }

                PostIteration();

                EncogLogging.Log(EncogLogging.LevelInfo,
                                 "Training iterations done, error: " + Error);
            }
            catch (IndexOutOfRangeException ex)
            {
                EncogValidate.ValidateNetworkForTraining(_network,
                                                         Training);
                throw new EncogError(ex);
            }
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
        public override int IterationNumber
        {
            get { return _iteration; }
            set { _iteration = value; }
        }


        /// <inheritdoc/>
        public IContainsFlat Network
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

            if (_flat.HasContext)
            {
                _workers[0].Network.ClearContext();
            }

            _totalError = 0;

            Parallel.ForEach(_workers, worker => worker.Run());
            

            Error = _totalError / _workers.Length;
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
            EngineArray.ArrayCopy(_workers[_workers.Length - 1].Network.LayerOutput, _flat.LayerOutput);
        }

        /// <summary>
        /// Init the process.
        /// </summary>
        ///
        private void Init()
        {
            // fix flat spot, if needed
            _flatSpot = new double[_flat.ActivationFunctions.Length];

            if (FixFlatSpot)
            {
                for (int i = 0; i < _flat.ActivationFunctions.Length; i++)
                {
                    IActivationFunction af = _flat.ActivationFunctions[i];
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
                _workers[index++] = new GradientWorker(((FlatNetwork)_network.Flat.Clone()),
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
            double[] weights = _flat.Weights;
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
            double limit = _flat.ConnectionLimit;
            double[] weights = _flat.Weights;
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


        #endregion

        /// <summary>
        /// Process as pure batch (size 0). Batch size equal to training set size.
        /// </summary>
        private void ProcessPureBatch()
        {
            CalculateGradients();

            if (_flat.Limited)
            {
                LearnLimited();
            }
            else
            {
                Learn();
            }
        }

        private void ProcessBatches()
        {
            if (_workers == null)
            {
                Init();
            }

            if (_flat.HasContext)
            {
                _workers[0].Network.ClearContext();
            }

            _workers[0].CalculateError.Reset();

            int lastLearn = 0;

            for (int i = 0; i < Training.Count; i++)
            {
                _workers[0].Run(i);

                lastLearn++;

                if (lastLearn++ >= BatchSize)
                {
                    if (_flat.Limited)
                    {
                        LearnLimited();
                    }
                    else
                    {
                        Learn();
                        lastLearn = 0;
                    }
                }
            }

            // handle any remaining learning
            if (lastLearn > 0)
            {
                Learn();
            }

            this.Error = _workers[0].CalculateError.Calculate();

        }


        /// <summary>
        /// The batch size. Specify 1 for pure online training. Specify 0 for pure
	    /// batch training (complete training set in one batch). Otherwise specify
	    /// the batch size for batch training.
        /// </summary>
        public int BatchSize { get; set; }
    }
}
