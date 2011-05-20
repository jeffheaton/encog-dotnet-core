using Encog.ML.Data;
using Encog.Neural.Flat.Train.Prop;

namespace Encog.Neural.Networks.Training.Propagation.Manhattan
{
    /// <summary>
    /// One problem that the backpropagation technique has is that the magnitude of
    /// the partial derivative may be calculated too large or too small. The
    /// Manhattan update algorithm attempts to solve this by using the partial
    /// derivative to only indicate the sign of the update to the weight matrix. The
    /// actual amount added or subtracted from the weight matrix is obtained from a
    /// simple constant. This constant must be adjusted based on the type of neural
    /// network being trained. In general, start with a higher constant and decrease
    /// it as needed.
    /// The Manhattan update algorithm can be thought of as a simplified version of
    /// the resilient algorithm. The resilient algorithm uses more complex techniques
    /// to determine the update value.
    /// </summary>
    ///
    public class ManhattanPropagation : Propagation, ILearningRate
    {
        /// <summary>
        /// The default tolerance to determine of a number is close to zero.
        /// </summary>
        ///
        internal const double DEFAULT_ZERO_TOLERANCE = 0.001d;

        /// <summary>
        /// Construct a Manhattan propagation training object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">The learning rate.</param>
        public ManhattanPropagation(ContainsFlat network,
                                    MLDataSet training, double learnRate) : base(network, training)
        {
            FlatTraining = new TrainFlatNetworkManhattan(network.Flat,
                                                         Training, learnRate);
        }


        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public virtual double LearningRate
        {
            get { return ((TrainFlatNetworkManhattan) FlatTraining).LearningRate; }
            set { ((TrainFlatNetworkManhattan) FlatTraining).LearningRate = value; }
        }

        #endregion

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <returns>Always returns null.</returns>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <param name="state">Not used.</param>
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }
}