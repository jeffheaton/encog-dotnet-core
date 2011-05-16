using System;
using Encog.ML.Data;
using Encog.Neural.Flat.Train.Prop;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Propagation.Resilient
{
    /// <summary>
    /// One problem with the backpropagation algorithm is that the magnitude of the
    /// partial derivative is usually too large or too small. Further, the learning
    /// rate is a single value for the entire neural network. The resilient
    /// propagation learning algorithm uses a special update value(similar to the
    /// learning rate) for every neuron connection. Further these update values are
    /// automatically determined, unlike the learning rate of the backpropagation
    /// algorithm.
    /// For most training situations, we suggest that the resilient propagation
    /// algorithm (this class) be used for training.
    /// There are a total of three parameters that must be provided to the resilient
    /// training algorithm. Defaults are provided for each, and in nearly all cases,
    /// these defaults are acceptable. This makes the resilient propagation algorithm
    /// one of the easiest and most efficient training algorithms available.
    /// The optional parameters are:
    /// zeroTolerance - How close to zero can a number be to be considered zero. The
    /// default is 0.00000000000000001.
    /// initialUpdate - What are the initial update values for each matrix value. The
    /// default is 0.1.
    /// maxStep - What is the largest amount that the update values can step. The
    /// default is 50.
    /// Usually you will not need to use these, and you should use the constructor
    /// that does not require them.
    /// </summary>
    ///
    public class ResilientPropagation : Propagation
    {
        /// <summary>
        /// Continuation tag for the last gradients.
        /// </summary>
        ///
        public const String LAST_GRADIENTS = "LAST_GRADIENTS";

        /// <summary>
        /// Continuation tag for the last values.
        /// </summary>
        ///
        public const String UPDATE_VALUES = "UPDATE_VALUES";

        /// <summary>
        /// Construct an RPROP trainer, allows an OpenCL device to be specified. Use
        /// the defaults for all training parameters. Usually this is the constructor
        /// to use as the resilient training algorithm is designed for the default
        /// parameters to be acceptable for nearly all problems.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        public ResilientPropagation(ContainsFlat network,
                                    MLDataSet training)
            : this(network, training, RPROPConst.DEFAULT_INITIAL_UPDATE, RPROPConst.DEFAULT_MAX_STEP)
        {
        }

        /// <summary>
        /// Construct a resilient training object, allow the training parameters to
        /// be specified. Usually the default parameters are acceptable for the
        /// resilient training algorithm. Therefore you should usually use the other
        /// constructor, that makes use of the default values.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="initialUpdate"></param>
        /// <param name="maxStep">The maximum that a delta can reach.</param>
        public ResilientPropagation(ContainsFlat network,
                                    MLDataSet training, double initialUpdate,
                                    double maxStep) : base(network, training)
        {
            var rpropFlat = new TrainFlatNetworkResilient(
                network.Flat, Training,
                RPROPConst.DEFAULT_ZERO_TOLERANCE, initialUpdate, maxStep);
            FlatTraining = rpropFlat;
        }


        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return true; }
        }

        /// <summary>
        /// Determine if the specified continuation object is valid to resume with.
        /// </summary>
        ///
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
        /// training method and network.</returns>
        public bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(
                LAST_GRADIENTS)
                || !state.Contents.ContainsKey(
                    UPDATE_VALUES))
            {
                return false;
            }

            if (!state.TrainingType.Equals(GetType().Name))
            {
                return false;
            }

            var d = (double[]) state
                                   .Get(LAST_GRADIENTS);
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

            result.Set(LAST_GRADIENTS,
                       ((TrainFlatNetworkResilient) FlatTraining).LastGradient);
            result.Set(UPDATE_VALUES,
                       ((TrainFlatNetworkResilient) FlatTraining).UpdateValues);

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
            var lastGradient = (double[]) state
                                              .Get(LAST_GRADIENTS);
            var updateValues = (double[]) state
                                              .Get(UPDATE_VALUES);

            EngineArray.ArrayCopy(lastGradient,
                                  ((TrainFlatNetworkResilient) FlatTraining).LastGradient);
            EngineArray.ArrayCopy(updateValues,
                                  ((TrainFlatNetworkResilient) FlatTraining).UpdateValues);
        }
    }
}