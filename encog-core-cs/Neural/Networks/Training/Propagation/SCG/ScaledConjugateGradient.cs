using Encog.ML.Data;
using Encog.Neural.Flat.Train.Prop;

namespace Encog.Neural.Networks.Training.Propagation.SCG
{
    /// <summary>
    /// This is a training class that makes use of scaled conjugate gradient methods.
    /// It is a very fast and efficient training algorithm.
    /// </summary>
    ///
    public class ScaledConjugateGradient : Propagation
    {
        /// <summary>
        /// Construct a training class.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public ScaledConjugateGradient(ContainsFlat network,
                                       MLDataSet training) : base(network, training)
        {
            var rpropFlat = new TrainFlatNetworkSCG(
                network.Flat, Training);
            FlatTraining = rpropFlat;
        }

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <returns>Always returns false.</returns>
        public override sealed bool CanContinue
        {
            get { return false; }
        }

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