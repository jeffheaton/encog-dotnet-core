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
using Encog.Util.CL;
using System.Collections;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Worker class for the mulithreaded training of flat networks.
    /// </summary>
    public abstract class TrainFlatNetworkMulti
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
        /// The average number of ticks that each CL worker took.
        /// </summary>
        private long clTimePerIteration;

        /// <summary>
        /// The average number of ticks that each CPU worker took.
        /// </summary>
        private long cpuTimePerIteration;

        /// <summary>
        /// The performance ratio between CPU and CL.  
        /// Positive number means CL workers are faster than CPU ones.
        /// </summary>
        private double calculatedCLRatio;

        /// <summary>
        /// The enforced CL ratio.
        /// </summary>
        private double enforcedCLRatio;

        /// <summary>
        /// Train a flat network multithreaded. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="enforcedCLRatio">Allows you to determine how much 
        /// work the CPU and GPU get.  For example, to give the CL/GPU 
        /// twice as much work as the CPU, specify 1.5.  To give it half as 
        /// much, choose 0.5.</param>
        public TrainFlatNetworkMulti(FlatNetwork network,
            INeuralDataSet training,
            double enforcedCLRatio)
        {

            if (!(training is IIndexable))
                throw new TrainingError("Training data must be Indexable for this training type.");

            this.training = training;
            this.network = network;

            this.indexable = (IIndexable)training;
            this.enforcedCLRatio = enforcedCLRatio;
            this.NumThreads = 0;   
            
        }


        public abstract double UpdateWeight(double[] gradients, double[] lastGradient, int index);

        private void Init()
        {
            gradients = new double[network.Weights.Length];
            lastGradient = new double[network.Weights.Length];

            weights = network.Weights;

            IList<EncogCLDevice> clDevices = null;

            DetermineWorkload determine;

            //  consider CL, if enabled
            if (Encog.Instance.CL != null)
            {
                // if we are already using CPU devices, then do not reuse them as CL devices.
                // They would be pulling "double duity" and have bad performance.
                if (NumThreads != -1)
                    Encog.Instance.CL.DisableAllCPUs();
                else
                    Encog.Instance.CL.EnableAllCPUs();

                clDevices = Encog.Instance.CL.EnabledDevices;
                determine = new DetermineWorkload(NumThreads, clDevices.Count, (int)this.indexable.Count);
            }
            else
                determine = new DetermineWorkload(NumThreads, (int)this.indexable.Count);

            determine.CLRatio = enforcedCLRatio;
            this.workers = new IFlatGradientWorker[determine.TotalWorkerCount];

            determine.CalculateWorkers();
            int index = 0;

            // if we are using CL, then we need to compile the kernels for this network
            if (Encog.Instance.CL != null)
            {
                IDictionary<String, String> options = new Dictionary<String, String>();
                options["NEURON_COUNT"] = "" + network.NeuronCount;
                options["WEIGHT_COUNT"] = "" + network.Weights.Length;

                // is there only one activation function?  If so, there are some optimizations we can use.
                int act = network.HasSameActivationFunction();

                if (act == FlatNetwork.ACTIVATION_SIGMOID)
                    options["USE_SIGMOID"] = "1";
                else if (act == FlatNetwork.ACTIVATION_TANH)
                    options["USE_TANH"] = "1";

                foreach (EncogCLPlatform platform in Encog.Instance.CL.Platforms)
                {
                    platform.NetworkTrain.Compile(options);
                    platform.NetworkTrain.Init(network);
                }
            }

            // handle CL
            int idx = 0;
            foreach (IntRange r in determine.CLRanges)
            {
                this.workers[index++] = new GradientWorkerCL(clDevices[idx++], network.Clone(), this, indexable, r.Low, r.High);
            }

            // handle CPU
            foreach (IntRange r in determine.CPURanges)
            {
                this.workers[index++] = new GradientWorkerCPU(network.Clone(), this, indexable, r.Low, r.High);
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
            if (this.workers == null)
                Init();

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
                weights[i] += UpdateWeight(gradients, lastGradient, i);
                gradients[i] = 0;
            }
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
            long totalCPU = 0;
            long countCPU = 0;
            long totalCL = 0;
            long countCL = 0;

            foreach(IFlatGradientWorker worker in this.workers )
            {
                if( worker is GradientWorkerCPU )
                {
                    countCPU++;
                    totalCPU+=worker.ElapsedTime;
                }
                else if( worker is GradientWorkerCL )
                {
                    countCL++;
                    totalCL+=worker.ElapsedTime;
                }
            }

            if( countCPU>0 )
                this.cpuTimePerIteration = totalCPU/countCPU;
            
            if( countCL>0 )
                this.clTimePerIteration = totalCL/countCL;
            
            this.calculatedCLRatio = ((double)this.cpuTimePerIteration) /
                ((double)this.clTimePerIteration);
        }

        /// <summary>
        /// The average number of miliseconds that each CL worker took.
        /// </summary>
        public long CLTimePerIteration
        {
            get
            {
                return this.clTimePerIteration;
            }
        }

        /// <summary>
        /// The average number of miliseconds that each CPU worker took.
        /// </summary>
        public long CPUTimePerIteration
        {
            get
            {
                return this.cpuTimePerIteration;
            }
        }

        /// <summary>
        /// The performance ratio between CPU and CL.  
        /// Positive number means CL workers are faster than CPU ones.
        /// </summary>
        public double CalculatedCLRatio
        {
            get
            {
                return this.calculatedCLRatio;
            }
        }
    }
}
