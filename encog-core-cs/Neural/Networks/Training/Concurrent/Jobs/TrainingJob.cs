using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.Neural.NeuralData;
using Encog.Engine.Network.Train.Prop;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Neural.Networks.Training.Strategy.End;

namespace Encog.Neural.Networks.Training.Concurrent.Jobs
{
    /// <summary>
    /// Base class for all concurrent training jobs.
    /// </summary>
    public abstract class TrainingJob
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        public BasicNetwork Network { get; set; }

        /// <summary>
        /// The training data to use.
        /// </summary>
        public MLDataSet Training { get; set; }

        /// <summary>
        /// The strategies to use.
        /// </summary>
        private IList<IStrategy> strategies = new List<IStrategy>();

        /// <summary>
        /// Any errors that were thrown.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// True, if binary training data should be loaded to memory.
        /// </summary>
        public bool LoadToMemory { get; set; }

        /// <summary>
        /// The trainer being used.
        /// </summary>
        public ITrain Train { get; set; }

        /// <summary>
        /// Holds any errors that occur during training.
        /// </summary>
        public Exception CurrentException { get; set; }

        /// <summary>
        /// Iterations per call to kernel.
        /// </summary>
        public int IterationsPer { get; set; }

        /// <summary>
        /// The local ratio.
        /// </summary>
        public double LocalRatio { get; set; }

        /// <summary>
        /// The global ratio.
        /// </summary>
        public int GlobalRatio { get; set; }

        /// <summary>
        /// The segmentation ratio.
        /// </summary>
        public double SegmentationRatio { get; set; }

        /// <summary>
        /// Construct a training job. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="loadToMemory">True, if binary data should be loaded to memory.</param>
        public TrainingJob(BasicNetwork network,
                 MLDataSet training, bool loadToMemory)
            : base()
        {

            this.Network = network;
            this.Training = training;
            this.LoadToMemory = loadToMemory;
            this.IterationsPer = 1;
            this.LocalRatio = 1.0;
            this.GlobalRatio = 1;
            this.SegmentationRatio = 1.0;

        }

        /// <summary>
        /// Create a trainer to use. 
        /// </summary>
        /// <param name="profile">The OpenCL device to use, or null for the CPU.</param>
        /// <param name="singleThreaded">True, if single threaded.</param>
        public abstract void CreateTrainer(bool singleThreaded);

        /// <summary>
        /// The strategies.
        /// </summary>
        public IList<IStrategy> Strategies
        {
            get
            {
                return this.strategies;
            }
        }

        /// <summary>
        /// Check and see if we should continue.
        /// </summary>
        /// <returns>True, if training should continue.</returns>
        public bool ShouldContinue()
        {
            foreach (IStrategy strategy in Train.Strategies)
            {
                if (strategy is IEndTrainingStrategy)
                {
                    IEndTrainingStrategy end = (IEndTrainingStrategy)strategy;

                    if (end.ShouldStop())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
