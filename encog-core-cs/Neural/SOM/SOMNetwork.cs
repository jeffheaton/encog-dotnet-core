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
using Encog.MathUtil.Matrices;
using Encog.ML;
using Encog.ML.Data;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.Util;

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
        /// The weights of the output neurons base on the input from the input
	    /// neurons.
        /// </summary>
        private Matrix _weights;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SOMNetwork()
        {
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="inputCount">Number of input neurons</param>
        /// <param name="outputCount">Number of output neurons</param>
        public SOMNetwork(int inputCount, int outputCount)
        {
            _weights = new Matrix(outputCount, inputCount);
        }

        /// <summary>
        /// The weights.
        /// </summary>
        public Matrix Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <summary>
        /// Classify the input into one of the output clusters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The cluster it was clasified into.</returns>
        public int Classify(IMLData input)
        {
            if (input.Count > InputCount)
            {
                throw new NeuralNetworkError(
                    "Can't classify SOM with input size of " + InputCount
                    + " with input data of count " + input.Count);
            }

            double[][] m = _weights.Data;
            double minDist = Double.PositiveInfinity;
            int result = -1;

            for (int i = 0; i < OutputCount; i++)
            {
                double dist = EngineArray.EuclideanDistance(input, m[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    result = i;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get { return _weights.Cols; }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get { return _weights.Rows; }
        }

        /// <summary>
        /// Calculate the error for the specified data set. The error is the largest distance.
        /// </summary>
        /// <param name="data">The data set to check.</param>
        /// <returns>The error.</returns>
        public double CalculateError(IMLDataSet data)
        {
            var bmu = new BestMatchingUnit(this);

            bmu.Reset();

            // Determine the BMU for each training element.
            foreach (IMLDataPair pair in data)
            {
                IMLData input = pair.Input;
                bmu.CalculateBMU(input);
            }

            // update the error
            return bmu.WorstDistance/100.0;
        }

        /// <summary>
        /// Randomize the network.
        /// </summary>
        public void Reset()
        {
            _weights.Randomize(-1, 1);
        }

        /// <summary>
        /// Randomize the network.
        /// </summary>
        /// <param name="seed">Not used.</param>
        public void Reset(int seed)
        {
            Reset();
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void UpdateProperties()
        {
            // unneeded
        }

        /// <summary>
        /// An alias for the classify method, kept for compatibility 
	    /// with earlier versions of Encog.
        /// </summary>
        /// <param name="input">The input pattern.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(IMLData input)
        {
            return Classify(input);
        }
    }
}
