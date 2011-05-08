using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.Neural.NeuralData;
using Encog.Engine.Network.Train.Prop;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Neural.Networks.Training.Concurrent.Jobs
{
    /// <summary>
    /// A training definition for RPROP training.
    /// </summary>
    public class RPROPJob : TrainingJob
    {
        /// <summary>
        /// The initial update value.
        /// </summary>
        private double InitialUpdate { get; set; }

        /// <summary>
        /// The maximum step value.
        /// </summary>
        private double MaxStep { get; set; }
      
        /// <summary>
        /// Construct an RPROP job. For more information on RPROP see the
        /// ResilientPropagation class. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="loadToMemory">True if binary training data should be loaded to memory.</param>
        public RPROPJob(BasicNetwork network, MLDataSet training,
                bool loadToMemory) :
            this(network, training, loadToMemory, RPROPConst.DEFAULT_INITIAL_UPDATE, RPROPConst.DEFAULT_MAX_STEP, 1, 1, 1, 1)
        {

        }

        /// <summary>
        /// Construct an RPROP job. For more information on RPROP see the
        /// ResilientPropagation class. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="loadToMemory">True if binary training data should be loaded to memory.</param>
        /// <param name="localRatio">The local ratio, used if this job is performed by an OpenCL Device.</param>
        /// <param name="globalRatio">The global ratio, used if this job is performed by an OpenCL Device.</param>
        /// <param name="segmentationRatio">The segmentation ratio, used if this job is performed by an OpenCL Device.</param>
        /// <param name="iterationsPer">How many iterations to process per cycle.</param>
        public RPROPJob(BasicNetwork network, MLDataSet training,
                bool loadToMemory, double localRatio, int globalRatio, double segmentationRatio, int iterationsPer) :
            this(network, training,
                 loadToMemory, RPROPConst.DEFAULT_INITIAL_UPDATE,
                 RPROPConst.DEFAULT_MAX_STEP, localRatio, globalRatio, segmentationRatio, iterationsPer)
        {
        }

        /// <summary>
        /// Construct an RPROP job. For more information on RPROP see the
        /// ResilientPropagation class. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="loadToMemory">True if binary training data should be loaded to memory.</param>
        /// <param name="initialUpdate">The initial update.</param>
        /// <param name="maxStep">The max step.</param>
        /// <param name="localRatio">The local ratio, used if this job is performed by an OpenCL Device.</param>
        /// <param name="globalRatio">The global ratio, used if this job is performed by an OpenCL Device.</param>
        /// <param name="segmentationRatio">The segmentation ratio, used if this job is performed by an OpenCL Device.</param>
        /// <param name="iterationsPer">How many iterations to process per cycle.</param>
        public RPROPJob(BasicNetwork network, MLDataSet training,
                bool loadToMemory, double initialUpdate,
                double maxStep, double localRatio, int globalRatio, double segmentationRatio, int iterationsPer) :
            base(network, training, loadToMemory)
        {
            this.InitialUpdate = initialUpdate;
            this.MaxStep = maxStep;
            this.LocalRatio = localRatio;
            this.GlobalRatio = globalRatio;
            this.SegmentationRatio = segmentationRatio;
            this.IterationsPer = iterationsPer;
        }

        /// <inheritdoc/>
        public override void CreateTrainer(bool singleThreaded)
        {
            Propagation.Propagation train = new ResilientPropagation(Network,
                    Training, InitialUpdate, MaxStep);

            if (singleThreaded)
                train.NumThreads = 1;
            else
                train.NumThreads = 0;


            foreach (IStrategy strategy in Strategies)
            {
                train.AddStrategy(strategy);
            }

            Train = train;
        }

    }
}
