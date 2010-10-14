using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public double InitialUpdate { get; set; }

        /// <summary>
        /// The maximum step value.
        /// </summary>
        public double MaxStep { get; set; }
        
        /// <summary>
        /// Construct an RPROP job. For more information on RPROP see the
        /// ResilientPropagation class. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="loadToMemory">True if binary training data should be loaded to memory.</param>
        /// <param name="initialUpdate">The initial update.</param>
        /// <param name="maxStep">The max step.</param>
        public RPROPJob(BasicNetwork network, INeuralDataSet training,
                 bool loadToMemory, double initialUpdate,
                 double maxStep)
            : base(network, training, loadToMemory)
        {

            this.InitialUpdate = initialUpdate;
            this.MaxStep = maxStep;
        }

        /// <inheritDoc/>
        public override void CreateTrainer(OpenCLTrainingProfile profile, bool singleThreaded)
        {
            Propagation.Propagation train = new ResilientPropagation(Network,
                    Training, profile, InitialUpdate, MaxStep);

            if (singleThreaded)
            {
                train.NumThreads = 1;
            }

            foreach (IStrategy strategy in Strategies)
            {
                train.AddStrategy(strategy);
            }

            Train = train;
        }

    }
}
