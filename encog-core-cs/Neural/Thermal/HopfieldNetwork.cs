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
using Encog.ML.Data;
using Encog.ML.Data.Specific;
using Encog.Util;

namespace Encog.Neural.Thermal
{
    /// <summary>
    /// Implements a Hopfield network.
    /// </summary>
    [Serializable]
    public class HopfieldNetwork : ThermalNetwork
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public HopfieldNetwork()
        {
        }

        /// <summary>
        /// Construct a Hopfield with the specified neuron count.
        /// </summary>
        ///
        /// <param name="neuronCount">The neuron count.</param>
        public HopfieldNetwork(int neuronCount) : base(neuronCount)
        {
        }

        /// <inheritdoc/>
        public override int InputCount
        {
            get { return NeuronCount; }
        }


        /// <inheritdoc/>
        public override int OutputCount
        {
            get { return NeuronCount; }
        }

        /// <summary>
        /// Train the neural network for the specified pattern. The neural network
        /// can be trained for more than one pattern. To do this simply call the
        /// train method more than once.
        /// </summary>
        ///
        /// <param name="pattern">The pattern to train for.</param>
        public void AddPattern(IMLData pattern)
        {
            if (pattern.Count != NeuronCount)
            {
                throw new NeuralNetworkError("Network with " + NeuronCount
                                             + " neurons, cannot learn a pattern of size "
                                             + pattern.Count);
            }

            // Create a row matrix from the input, convert boolean to bipolar
            Matrix m2 = Matrix.CreateRowMatrix(pattern);
            // Transpose the matrix and multiply by the original input matrix
            Matrix m1 = MatrixMath.Transpose(m2);
            Matrix m3 = MatrixMath.Multiply(m1, m2);

            // matrix 3 should be square by now, so create an identity
            // matrix of the same size.
            Matrix identity = MatrixMath.Identity(m3.Rows);

            // subtract the identity matrix
            Matrix m4 = MatrixMath.Subtract(m3, identity);

            // now add the calculated matrix, for this pattern, to the
            // existing weight matrix.
            ConvertHopfieldMatrix(m4);
        }

        /// <summary>
        /// Note: for Hopfield networks, you will usually want to call the "run"
        /// method to compute the output.
        /// This method can be used to copy the input data to the current state. A
        /// single iteration is then run, and the new current state is returned.
        /// </summary>
        ///
        /// <param name="input">The input pattern.</param>
        /// <returns>The new current state.</returns>
        public override sealed IMLData Compute(IMLData input)
        {
            var result = new BiPolarMLData(input.Count);
			input.CopyTo(CurrentState.Data, 0, input.Count);
            Run();

            for (int i = 0; i < CurrentState.Count; i++)
            {
                result.SetBoolean(i,
                                  BiPolarUtil.Double2bipolar(CurrentState[i]));
            }
            EngineArray.ArrayCopy(CurrentState.Data, result.Data);
            return result;
        }

        /// <summary>
        /// Update the Hopfield weights after training.
        /// </summary>
        ///
        /// <param name="delta">The amount to change the weights by.</param>
        private void ConvertHopfieldMatrix(Matrix delta)
        {
            // add the new weight matrix to what is there already
            for (int row = 0; row < delta.Rows; row++)
            {
                for (int col = 0; col < delta.Rows; col++)
                {
                    AddWeight(row, col, delta[row, col]);
                }
            }
        }


        /// <summary>
        /// Perform one Hopfield iteration.
        /// </summary>
        ///
        public void Run()
        {
            for (int toNeuron = 0; toNeuron < NeuronCount; toNeuron++)
            {
                double sum = 0;
                for (int fromNeuron = 0; fromNeuron < NeuronCount; fromNeuron++)
                {
                    sum += CurrentState[fromNeuron]
                           *GetWeight(fromNeuron, toNeuron);
                }
                CurrentState[toNeuron] = sum;
            }
        }

        /// <summary>
        /// Run the network until it becomes stable and does not change from more
        /// runs.
        /// </summary>
        ///
        /// <param name="max">The maximum number of cycles to run before giving up.</param>
        /// <returns>The number of cycles that were run.</returns>
        public int RunUntilStable(int max)
        {
            bool done = false;
            String currentStateStr = (CurrentState.ToString());

            int cycle = 0;
            do
            {
                Run();
                cycle++;

                String lastStateStr = (CurrentState.ToString());

                if (!currentStateStr.Equals(lastStateStr))
                {
                    if (cycle > max)
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }

                currentStateStr = lastStateStr;
            } while (!done);

            return cycle;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void UpdateProperties()
        {
            // nothing needed here
        }
    }
}
