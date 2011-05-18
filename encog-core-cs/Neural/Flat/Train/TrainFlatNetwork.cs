using Encog.ML.Data;

namespace Encog.Neural.Flat.Train
{
    /// <summary>
    /// Common interface for training a flat neural network.
    /// </summary>
    ///
    public interface TrainFlatNetwork
    {
        /// <summary>
        /// The error.
        /// </summary>
        double Error { get; }


        /// <value>The trained neural network.</value>
        FlatNetwork Network { get; }

        /// <value>The data we are training with.</value>
        MLDataSet Training { get; }

        /// <summary>
        /// Set the number of threads to use.
        /// </summary>
        ///
        /// <value>The number of threads to use.</value>
        int NumThreads { get; set; }

        /// <summary>
        /// Set the iteration.
        /// </summary>
        int IterationNumber { get; set; }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        void Iteration();

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        /// <param name="count">The number of iterations.</param>
        void Iteration(int count);


        /// <summary>
        /// Training is to stop, free any resources.
        /// </summary>
        ///
        void FinishTraining();
    }
}