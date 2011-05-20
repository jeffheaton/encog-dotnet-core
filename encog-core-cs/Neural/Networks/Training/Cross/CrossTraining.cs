using Encog.ML;
using Encog.ML.Data.Folded;
using Encog.ML.Train;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// Base class for cross training trainers. Must use a folded dataset.  
    /// </summary>
    ///
    public abstract class CrossTraining : BasicTraining
    {
        /// <summary>
        /// The folded dataset.
        /// </summary>
        ///
        private readonly FoldedDataSet folded;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly MLMethod network;

        /// <summary>
        /// Construct a cross trainer.
        /// </summary>
        ///
        /// <param name="network_0">The network.</param>
        /// <param name="training">The training data.</param>
        public CrossTraining(MLMethod network_0, FoldedDataSet training) : base(TrainingImplementationType.Iterative)
        {
            network = network_0;
            Training = training;
            folded = training;
        }


        /// <value>The folded training data.</value>
        public FoldedDataSet Folded
        {
            get { return folded; }
        }


        /// <inheritdoc/>
        public override MLMethod Method
        {            
            get { return network; }
        }
    }
}