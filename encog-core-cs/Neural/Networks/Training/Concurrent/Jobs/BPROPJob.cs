using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Engine.Network.Train.Prop;
using Encog.Neural.Networks.Training.Propagation.Back;

namespace Encog.Neural.Networks.Training.Concurrent.Jobs
{
    /// <summary>
    /// Performers actually perform the training. Currently there are performers for
    /// OpenCL and CPU.
    /// </summary>
    public class BPROPJob : TrainingJob
    {
        /// <summary>
        /// The learning rate to use.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// The momentum to use.
        /// </summary>
        public double Momentum { get; set; }

        /// <summary>
        /// Construct a job definition for RPROP. For more information on backprop,
        /// see the Backpropagation class. 
        /// </summary>
        /// <param name="network">The network to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="loadToMemory">Should binary data be loaded to memory?</param>
        /// <param name="learningRate">THe learning rate to use.</param>
        /// <param name="momentum">The momentum to use.</param>
        public BPROPJob(BasicNetwork network, INeuralDataSet training,
                 bool loadToMemory, double learningRate,
                 double momentum)
            : base(network, training, loadToMemory)
        {

            this.LearningRate = learningRate;
            this.Momentum = momentum;
        }

        /// <inheritDoc/>
        public override void CreateTrainer(OpenCLTrainingProfile profile, Boolean singleThreaded)
        {
            Propagation.Propagation train = new Backpropagation(Network, Training,
                   profile, LearningRate, Momentum);

            if (singleThreaded)
                train.NumThreads = 1;

            foreach (IStrategy strategy in Strategies)
            {
                train.AddStrategy(strategy);
            }

            Train = train;
        }
    }
}
