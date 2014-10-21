//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
        private readonly double[] _deriv;

        /// <summary>
        /// Second derivative.
        /// </summary>
        ///
        private readonly double[] _deriv2;

        /// <summary>
        /// Input neuron count.
        /// </summary>
        ///
        private readonly int _inputCount;

        /// <summary>
        /// Kernel type. 
        /// </summary>
        ///
        private readonly PNNKernelType _kernel;

        /// <summary>
        /// Output neuron count. 
        /// </summary>
        ///
        private readonly int _outputCount;

        /// <summary>
        /// Output mode.
        /// </summary>
        ///
        private readonly PNNOutputMode _outputMode;

        /// <summary>
        /// Confusion work area.
        /// </summary>
        ///
        private int[] _confusion;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="kernel">The kernel type to use.</param>
        /// <param name="outputMode">The output mode to use.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        protected AbstractPNN(PNNKernelType kernel,
                           PNNOutputMode outputMode, int inputCount,
                           int outputCount)
        {
            _kernel = kernel;
            _outputMode = outputMode;
            _inputCount = inputCount;
            _outputCount = outputCount;
            Trained = false;
            Error = -1000;
            _confusion = null;
            Exclude = -1;

            _deriv = new double[inputCount];
            _deriv2 = new double[inputCount];

            if (_outputMode == PNNOutputMode.Classification)
            {
                _confusion = new int[_outputCount + 1];
            }
        }


        /// <value>the deriv</value>
        public double[] Deriv
        {
            get { return _deriv; }
        }


        /// <value>the deriv2</value>
        public double[] Deriv2
        {
            get { return _deriv2; }
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
            get { return _inputCount; }
        }


        /// <value>the kernel</value>
        public PNNKernelType Kernel
        {
            get { return _kernel; }
        }


        /// <value>the outputCount</value>
        public int OutputCount
        {
            get { return _outputCount; }
        }


        /// <value>the outputMode</value>
        public PNNOutputMode OutputMode
        {
            get { return _outputMode; }
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
        public abstract IMLData Compute(IMLData input);

        /// <summary>
        /// Reset the confusion.
        /// </summary>
        ///
        public void ResetConfusion()
        {
        }
    }
}
