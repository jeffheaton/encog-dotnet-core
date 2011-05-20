//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML;
using Encog.ML.Data;

namespace Encog.Neural.PNN
{
    /// <summary>
    /// Abstract class to build PNN networks upon.
    /// </summary>
    [Serializable]
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
            get { return deriv; }
        }


        /// <value>the deriv2</value>
        public double[] Deriv2
        {
            get { return deriv2; }
        }


        /// <value>the error to set</value>
        public double Error { 
            get;
            set; }


        /// <value>the exclude to set</value>
        public int Exclude { 
            get; 
            set; }


        /// <value>the inputCount</value>
        public int InputCount
        {
            get { return inputCount; }
        }


        /// <value>the kernel</value>
        public PNNKernelType Kernel
        {
            get { return kernel; }
        }


        /// <value>the outputCount</value>
        public int OutputCount
        {
            get { return outputCount; }
        }


        /// <value>the outputMode</value>
        public PNNOutputMode OutputMode
        {
            get { return outputMode; }
        }


        /// <value>the trained to set</value>
        public bool Trained { 
            get; 
            set; }


        /// <value>the separateClass to set</value>
        public bool SeparateClass { 
            get; 
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
