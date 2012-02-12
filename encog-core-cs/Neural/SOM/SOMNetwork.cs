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
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.Util;
using System;

namespace Encog.Neural.SOM
{
    /// <summary>
    /// A self organizing map neural network.
    /// </summary>
    [Serializable]
    public class SOMNetwork : BasicML, IMLClassification, IMLResettable,
                              IMLError
    {
        /// <summary>
        /// Do not allow patterns to go below this very small number.
        /// </summary>
        ///
        public const double Verysmall = 1.0E-30d;

        /// <summary>
        /// The weights of the output neurons base on the input from the input
        /// neurons.
        /// </summary>
        ///
        private Matrix _weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public SOMNetwork()
        {
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        ///
        /// <param name="inputCount">Number of input neurons</param>
        /// <param name="outputCount">Number of output neurons</param>
        public SOMNetwork(int inputCount, int outputCount)
        {
            InputCount = inputCount;
            OutputCount = outputCount;
            _weights = new Matrix(inputCount, outputCount);
        }


        /// <value>the weights to set</value>
        public Matrix Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        #region MLClassification Members

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            IMLData result = Compute(input);
            return EngineArray.MaxIndex(result.Data);
        }

        /// <summary>
        /// Set the input count.
        /// </summary>
        public virtual int InputCount { get; set; }

        /// <inheritdoc/>
        public virtual int OutputCount { get; set; }

        #endregion

        #region MLError Members

        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {
            var bmu = new BestMatchingUnit(this);

            bmu.Reset();


            // Determine the BMU foreach each training element.
            foreach (IMLDataPair pair  in  data)
            {
                IMLData input = pair.Input;
                bmu.CalculateBMU(input);
            }

            // update the error
            return bmu.WorstDistance/100.0d;
        }

        #endregion

        #region MLResettable Members

        /// <inheritdoc/>
        public void Reset()
        {
            _weights.Randomize(-1, 1);
        }

        /// <inheritdoc/>
        public void Reset(int seed)
        {
            Reset();
        }

        #endregion

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        ///
        /// <param name="input">The input pattern.</param>
        /// <returns>The winning neuron.</returns>
        public IMLData Compute(IMLData input)
        {
            IMLData result = new BasicMLData(OutputCount);

            for (int i = 0; i < OutputCount; i++)
            {
                Matrix optr = _weights.GetCol(i);
                Matrix inputMatrix = Matrix.CreateRowMatrix(input.Data);
                result[i] = MatrixMath.DotProduct(inputMatrix, optr);
            }

            return result;
        }

        /// <inheritdoc/>
        public override sealed void UpdateProperties()
        {
            // unneeded
        }

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        ///
        /// <param name="input">The input pattern.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(IMLData input)
        {
            IMLData output = Compute(input);
            int win = EngineArray.IndexOfLargest(output.Data);
            return win;
        }
    }
}
