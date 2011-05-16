using System;
using Encog.ML.Data;

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train the flat network using Manhattan update rule.
    /// </summary>
    ///
    public class TrainFlatNetworkManhattan : TrainFlatNetworkProp
    {
        /// <summary>
        /// The zero tolerance to use.
        /// </summary>
        ///
        private readonly double zeroTolerance;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// Construct a trainer for flat networks to use the Manhattan update rule.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="theLearningRate">The learning rate to use.</param>
        public TrainFlatNetworkManhattan(FlatNetwork network,
                                         MLDataSet training, double theLearningRate) : base(network, training)
        {
            learningRate = theLearningRate;
            zeroTolerance = RPROPConst.DEFAULT_ZERO_TOLERANCE;
        }


        /// <value>the learningRate to set</value>
        public double LearningRate
        {
            /// <returns>the learningRate</returns>
            get { return learningRate; }
            /// <param name="theLearningRate">the learningRate to set</param>
            set { learningRate = value; }
        }


        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override sealed double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < zeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return learningRate;
            }
            else
            {
                return -learningRate;
            }
        }
    }
}