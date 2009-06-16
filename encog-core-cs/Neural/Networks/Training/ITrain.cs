using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Interface for all neural network training methods. This allows the training
    /// methods to be largely interchangeable. Though some training methods require
    /// specific types of neural network structure.
    /// </summary>
    public interface ITrain
    {
        /// <summary>
        /// Training strategies can be added to improve the training results. There
        /// are a number to choose from, and several can be used at once.
        /// </summary>
        /// <param name="strategy">The strategy to add.</param>
        void AddStrategy(IStrategy strategy);

        /// <summary>
        /// Get the current error percent from the training. You can also set the current error rate. 
        /// This is usually used by training strategies.
        /// </summary>
        double Error
        {
            get;
            set;
        }

        /// <summary>
        /// Get the current best network from the training.
        /// </summary>
        BasicNetwork Network
        {
            get;
        }

        /// <summary>
        /// The strategies to use.
        /// </summary>
        IList<IStrategy> Strategies
        {
            get;
        }

        /// <summary>
        /// The training data to use.
        /// </summary>
        INeuralDataSet Training
        {
            get;
        }

        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        void Iteration();



    }

}
