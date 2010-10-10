using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine
{
    /// <summary>
    /// Generic interface to a Machine Learning class, such as a neural network or
    /// SVM.
    /// </summary>
    public interface IEngineMachineLearning
    {
        /// <summary>
        /// Compute output for the given input. 
        /// </summary>
        /// <param name="input">An array of doubles for the input.</param>
        /// <param name="output">An array of doubles for the output.</param>
        void Compute(double[] input, double[] output);

        /// <summary>
        /// The input count.
        /// </summary>
        int InputCount { get; }

        /// <summary>
        /// The output size.
        /// </summary>
        int OutputCount { get; }
    }
}
