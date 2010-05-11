using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Concurrency;
using Encog.MathUtil;
using Encog.Util;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Worker class for the mulithreaded training of flat networks.
    /// </summary>
    public class TrainFlatNetworkMulti
    {
        /// <summary>
        /// The number of threads to use.
        /// </summary>
        public int NumThreads { get; set; }

        /// <summary>
        /// The gradients
        /// </summary>
        private double[] gradients;

        /// <summary>
        /// The last gradients, from the last training iteration.
        /// </summary>
        private double[] lastGradient;

        /// <summary>
        /// The network to train.
        /// </summary>
        private FlatNetwork network;

        /// <summary>
        /// The training data.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// The update values, for the weights and thresholds.
        /// </summary>
        private double[] updateValues;

        /// <summary>
        /// The network in indexable form.
        /// </summary>
        private IIndexable indexable;

        /// <summary>
        /// The weights and thresholds.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The workers.
        /// </summary>
        private IFlatGradientWorker[] workers;

        /// <summary>
        /// The total error.  Used to take the average of.
        /// </summary>
        private double totalError;

        /// <summary>
        /// The current error is the average error over all of the threads.
        /// </summary>
        private double currentError;

        /// <summary>
        /// The average number of miliseconds that each GPU worker took.
        /// </summary>
        private int gpuTimePerIteration;

        /// <summary>
        /// The average number of miliseconds that each CPU worker took.
        /// </summary>
        private int cpuTimePerIteration;

        /// <summary>
        /// The performance ratio between CPU & GPU.  
        /// Positive number means GPU workers are faster than CPU ones.
        /// </summary>
        private double gpuRatio;

        /// <summary>
        /// Train a flat network multithreaded. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        public TrainFlatNetworkMulti(FlatNetwork network,
                 INeuralDataSet training)
        {

            if (!(training is IIndexable))
                throw new TrainingError("Training data must be Indexable for this training type.");

            this.training = training;
            this.network = network;

            this.indexable = (IIndexable)training;

            gradients = new double[network.Weights.Length];
            updateValues = new double[network.Weights.Length];
            lastGradient = new double[network.Weights.Length];

            weights = network.Weights;

            for (int i = 0; i < updateValues.Length; i++)
            {
                updateValues[i] = ResilientPropagation.DEFAULT_INITIAL_UPDATE;
            }

            DetermineWorkload determine;
            
            //  consider GPU, if enabled
            /*if (Encog.Instance.GPU != null)
                determine = new DetermineWorkload(NumThreads, 1, (int)this.indexable.Count);
            else
                determine = new DetermineWorkload(NumThreads, (int)this.indexable.Count);
            */

            if (Encog.Instance.GPU != null)
                determine = new DetermineWorkload(4, 1, (int)this.indexable.Count);
            else
                determine = new DetermineWorkload(4, 0, (int)this.indexable.Count);
            
            
            this.workers = new IFlatGradientWorker[determine.TotalWorkerCount];

            determine.CalculateWorkers();
            int index = 0;

            // handle CPU
            foreach (IntRange r in determine.CPURanges )
            {
                this.workers[index++] = new GradientWorkerCPU(network.Clone(), this, indexable, r.Low, r.High);
            }

            // handle GPU
            foreach (IntRange r in determine.GPURanges)
            {
                this.workers[index++] = new GradientWorkerGPU(network.Clone(), this, indexable, r.Low, r.High);
            }

        }

        /// <summary>
        /// Called by the worker threads to report the progress at each step.
        /// </summary>
        /// <param name="gradients">The gradients from that worker.</param>
        /// <param name="error">The error for that worker.</param>
        public void Report(double[] gradients, double error)
        {
            lock (this)
            {
                for (int i = 0; i < gradients.Length; i++)
                {
                    this.gradients[i] += gradients[i];
                }
                this.totalError += error;
            }
        }

        /// <summary>
        /// The error from the neural network.
        /// </summary>
        public double Error
        {
            get
            {
                return this.currentError;
            }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public void Iteration()
        {

            TaskGroup group = EncogConcurrency.Instance.CreateTaskGroup();
            this.totalError = 0;

            foreach (IFlatGradientWorker worker in this.workers)
            {
                EncogConcurrency.Instance.ProcessTask(worker, group);
            }

            group.WaitForComplete();

            Learn();
            this.currentError = this.totalError / this.workers.Length;

            foreach (IFlatGradientWorker worker in this.workers)
            {
                EncogArray.ArrayCopy(this.weights, 0, worker.Weights, 0, this.weights.Length);
            }

            EncogArray.ArrayCopy(this.weights, 0, this.network.Weights, 0, this.weights.Length);

            CalculatePerformance();
        }

        /// <summary>
        /// Apply and learn.
        /// </summary>
        private void Learn()
        {
            for (int i = 0; i < gradients.Length; i++)
            {
                weights[i] += UpdateWeight(gradients, i);
                gradients[i] = 0;
            }
        }


        /// <summary>
        /// Determine the sign of the value. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double value)
        {
            if (Math.Abs(value) < ResilientPropagation.DEFAULT_ZERO_TOLERANCE)
            {
                return 0;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Determine the amount to change a weight by.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="index">The weight to adjust.</param>
        /// <returns>The amount to change this weight by.</returns>
        private double UpdateWeight(double[] gradients, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(this.gradients[index] * lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = updateValues[index]
                        * ResilientPropagation.POSITIVE_ETA;
                delta = Math.Min(delta, ResilientPropagation.DEFAULT_MAX_STEP);
                weightChange = Sign(this.gradients[index]) * delta;
                updateValues[index] = delta;
                lastGradient[index] = this.gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = updateValues[index]
                        * ResilientPropagation.NEGATIVE_ETA;
                delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                updateValues[index] = delta;
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = lastGradient[index];
                weightChange = Sign(this.gradients[index]) * delta;
                lastGradient[index] = this.gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }

        /// <summary>
        /// The trained neural network.
        /// </summary>
        public FlatNetwork Network
        {
            get
            {
                return network;
            }
        }

        /// <summary>
        /// The data we are training with.
        /// </summary>
        public INeuralDataSet Training
        {
            get
            {
                return training;
            }
        }

        private void CalculatePerformance()
        {
            int totalCPU = 0;
            int countCPU = 0;
            int totalGPU = 0;
            int countGPU = 0;

            foreach(IFlatGradientWorker worker in this.workers )
            {
                if( worker is GradientWorkerCPU )
                {
                    countCPU++;
                    totalCPU+=worker.ElapsedTime;
                }
                else if( worker is GradientWorkerGPU )
                {
                    countGPU++;
                    totalGPU+=worker.ElapsedTime;
                }
            }

            this.cpuTimePerIteration = totalCPU/countCPU;
            this.gpuTimePerIteration = totalGPU/countGPU;
            this.gpuRatio = ((double)this.gpuTimePerIteration) /
                ((double)this.cpuTimePerIteration);
        }

        /// <summary>
        /// The average number of miliseconds that each GPU worker took.
        /// </summary>
        public int GPUTimePerIteration
        {
            get
            {
                return this.gpuTimePerIteration;
            }
        }

        /// <summary>
        /// The average number of miliseconds that each CPU worker took.
        /// </summary>
        public int CPUTimePerIteration
        {
            get
            {
                return this.cpuTimePerIteration;
            }
        }

        /// <summary>
        /// The performance ratio between CPU & GPU.  
        /// Positive number means GPU workers are faster than CPU ones.
        /// </summary>
        public double GPURatio
        {
            get
            {
                return this.gpuRatio;
            }
        }
    }
}
