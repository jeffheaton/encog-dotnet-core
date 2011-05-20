using System;
using Encog.ML.Data;
using Encog.Neural.Flat.Train.Prop;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Util.Validate;

namespace Encog.Neural.Networks.Training.Propagation.Back
{
    /// <summary>
    /// This class implements a backpropagation training algorithm for feed forward
    /// neural networks. It is used in the same manner as any other training class
    /// that implements the Train interface.
    /// Backpropagation is a common neural network training algorithm. It works by
    /// analyzing the error of the output of the neural network. Each neuron in the
    /// output layer's contribution, according to weight, to this error is
    /// determined. These weights are then adjusted to minimize this error. This
    /// process continues working its way backwards through the layers of the neural
    /// network.
    /// This implementation of the backpropagation algorithm uses both momentum and a
    /// learning rate. The learning rate specifies the degree to which the weight
    /// matrixes will be modified through each iteration. The momentum specifies how
    /// much the previous learning iteration affects the current. To use no momentum
    /// at all specify zero.
    /// One primary problem with backpropagation is that the magnitude of the partial
    /// derivative is often detrimental to the training of the neural network. The
    /// other propagation methods of Manhatten and Resilient address this issue in
    /// different ways. In general, it is suggested that you use the resilient
    /// propagation technique for most Encog training tasks over back propagation.
    /// </summary>
    ///
    public class Backpropagation : Propagation, IMomentum,
                                   ILearningRate
    {
        /// <summary>
        /// The resume key for backpropagation.
        /// </summary>
        ///
        public const String LAST_DELTA = "LAST_DELTA";

        /// <summary>
        /// Create a class to train using backpropagation. Use auto learn rate and
        /// momentum. Use the CPU to train.
        /// </summary>
        ///
        /// <param name="network">The network that is to be trained.</param>
        /// <param name="training">The training data to be used for backpropagation.</param>
        public Backpropagation(ContainsFlat network, MLDataSet training) : this(network, training, 0, 0)
        {
            AddStrategy(new SmartLearningRate());
            AddStrategy(new SmartMomentum());
        }


        /// <param name="network">The network that is to be trained</param>
        /// <param name="training">The training set</param>
        /// <param name="learnRate"></param>
        /// <param name="momentum"></param>
        public Backpropagation(ContainsFlat network,
                               MLDataSet training, double learnRate,
                               double momentum) : base(network, training)
        {
            ValidateNetwork.ValidateMethodToData(network, training);
            var backFlat = new TrainFlatNetworkBackPropagation(
                network.Flat, Training, learnRate, momentum);
            FlatTraining = backFlat;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return true; }
        }


        /// <value>Ther last delta values.</value>
        public double[] LastDelta
        {
            get { return ((TrainFlatNetworkBackPropagation) FlatTraining).LastDelta; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate, this is value is essentially a percent. It is the
        /// degree to which the gradients are applied to the weight matrix to allow
        /// learning.
        /// </summary>
        public virtual double LearningRate
        {
            get { return ((TrainFlatNetworkBackPropagation) FlatTraining).LearningRate; }
            set { ((TrainFlatNetworkBackPropagation) FlatTraining).LearningRate = value; }
        }

        #endregion

        #region IMomentum Members

        /// <summary>
        /// Set the momentum for training. This is the degree to which changes from
        /// which the previous training iteration will affect this training
        /// iteration. This can be useful to overcome local minima.
        /// </summary>
        public virtual double Momentum
        {
            get { return ((TrainFlatNetworkBackPropagation) FlatTraining).Momentum; }
            set { ((TrainFlatNetworkBackPropagation) FlatTraining).LearningRate = value; }
        }

        #endregion

        /// <summary>
        /// Determine if the specified continuation object is valid to resume with.
        /// </summary>
        ///
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
        /// training method and network.</returns>
        public bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(LAST_DELTA))
            {
                return false;
            }

            if (!state.TrainingType.Equals(GetType().Name))
            {
                return false;
            }

            var d = (double[]) state.Get(LAST_DELTA);
            return d.Length == ((ContainsFlat) Method).Flat.Weights.Length;
        }

        /// <summary>
        /// Pause the training.
        /// </summary>
        ///
        /// <returns>A training continuation object to continue with.</returns>
        public override sealed TrainingContinuation Pause()
        {
            var result = new TrainingContinuation();
            result.TrainingType = GetType().Name;
            var backFlat = (TrainFlatNetworkBackPropagation) FlatTraining;
            double[] d = backFlat.LastDelta;
            result.Set(LAST_DELTA, d);
            return result;
        }

        /// <summary>
        /// Resume training.
        /// </summary>
        ///
        /// <param name="state">The training state to return to.</param>
        public override sealed void Resume(TrainingContinuation state)
        {
            if (!IsValidResume(state))
            {
                throw new TrainingError("Invalid training resume data length");
            }

            ((TrainFlatNetworkBackPropagation) FlatTraining).LastDelta = (double[]) state.Get(LAST_DELTA);
        }
    }
}