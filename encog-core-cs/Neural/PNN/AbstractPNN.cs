using System;
using Encog.ML;
using Encog.ML.Data;

namespace Encog.Neural.PNN
{
    /// <summary>
    /// Abstract class to build PNN networks upon.
    /// </summary>
    ///
    public abstract class AbstractPNN : BasicML
    {
        /// <summary>
        /// First derivative. 
        /// </summary>
        ///
        private readonly double[] deriv;

        /// <summary>
        /// Second derivative.
        /// </summary>
        ///
        private readonly double[] deriv2;

        /// <summary>
        /// Input neuron count.
        /// </summary>
        ///
        private readonly int inputCount;

        /// <summary>
        /// Kernel type. 
        /// </summary>
        ///
        private readonly PNNKernelType kernel;

        /// <summary>
        /// Output neuron count. 
        /// </summary>
        ///
        private readonly int outputCount;

        /// <summary>
        /// Output mode.
        /// </summary>
        ///
        private readonly PNNOutputMode outputMode;

        /// <summary>
        /// Confusion work area.
        /// </summary>
        ///
        private int[] confusion;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="kernel_0">The kernel type to use.</param>
        /// <param name="outputMode_1">The output mode to use.</param>
        /// <param name="inputCount_2">The input count.</param>
        /// <param name="outputCount_3">The output count.</param>
        public AbstractPNN(PNNKernelType kernel_0,
                           PNNOutputMode outputMode_1, int inputCount_2,
                           int outputCount_3)
        {
            kernel = kernel_0;
            outputMode = outputMode_1;
            inputCount = inputCount_2;
            outputCount = outputCount_3;
            Trained = false;
            Error = Double.MinValue;
            confusion = null;
            Exclude = -1;

            deriv = new double[inputCount_2];
            deriv2 = new double[inputCount_2];

            if (outputMode == PNNOutputMode.Classification)
            {
                confusion = new int[outputCount + 1];
            }
        }


        /// <value>the deriv</value>
        public double[] Deriv
        {
            /// <returns>the deriv</returns>
            get { return deriv; }
        }


        /// <value>the deriv2</value>
        public double[] Deriv2
        {
            /// <returns>the deriv2</returns>
            get { return deriv2; }
        }


        /// <value>the error to set</value>
        public double Error { /// <returns>the error</returns>
            get; /// <param name="error_0">the error to set</param>
            set; }


        /// <value>the exclude to set</value>
        public int Exclude { /// <returns>the exclude</returns>
            get; /// <param name="exclude_0">the exclude to set</param>
            set; }


        /// <value>the inputCount</value>
        public int InputCount
        {
            /// <returns>the inputCount</returns>
            get { return inputCount; }
        }


        /// <value>the kernel</value>
        public PNNKernelType Kernel
        {
            /// <returns>the kernel</returns>
            get { return kernel; }
        }


        /// <value>the outputCount</value>
        public int OutputCount
        {
            /// <returns>the outputCount</returns>
            get { return outputCount; }
        }


        /// <value>the outputMode</value>
        public PNNOutputMode OutputMode
        {
            /// <returns>the outputMode</returns>
            get { return outputMode; }
        }


        /// <value>the trained to set</value>
        public bool Trained { /// <returns>the trained</returns>
            get; /// <param name="trained_0">the trained to set</param>
            set; }


        /// <value>the separateClass to set</value>
        public bool SeparateClass { /// <returns>the separateClass</returns>
            get; /// <param name="separateClass_0">the separateClass to set</param>
            set; }

        /// <summary>
        /// Compute the output from the network.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public abstract MLData Compute(MLData input);

        /// <summary>
        /// Reset the confusion.
        /// </summary>
        ///
        public void ResetConfusion()
        {
        }
    }
}