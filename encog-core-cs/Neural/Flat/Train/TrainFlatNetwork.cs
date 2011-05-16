using Encog.ML.Data;

namespace Encog.Neural.Flat.Train
{
    /// <summary>
    /// Common interface for training a flat neural network.
    /// </summary>
    ///
    public interface TrainFlatNetwork
    {
        /// <value>The error from the neural network.</value>
        double Error { /// <returns>The error from the neural network.</returns>
            get; }


        /// <value>The trained neural network.</value>
        FlatNetwork Network { /// <returns>The trained neural network.</returns>
            get; }


        /// <value>The data we are training with.</value>
        MLDataSet Training { /// <returns>The data we are training with.</returns>
            get; }

        /// <summary>
        /// Set the number of threads to use.
        /// </summary>
        ///
        /// <value>The number of threads to use.</value>
        int NumThreads { /// <returns>The number of threads.</returns>
            get;
            /// <summary>
            /// Set the number of threads to use.
            /// </summary>
            ///
            /// <param name="numThreads">The number of threads to use.</param>
            set; }

        /// <summary>
        /// Set the iteration.
        /// </summary>
        ///
        /// <value>The iteration.</value>
        int IterationNumber { /// <returns>The current iteration.</returns>
            get;
            /// <summary>
            /// Set the iteration.
            /// </summary>
            ///
            /// <param name="iteration">The iteration.</param>
            set; }


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