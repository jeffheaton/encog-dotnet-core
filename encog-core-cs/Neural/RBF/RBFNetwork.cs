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
using Encog.MathUtil.Randomize;
using Encog.MathUtil.RBF;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.Neural.RBF
{
    /// <summary>
    /// RBF neural network.
    /// </summary>
    [Serializable]
    public class RBFNetwork : BasicML, IMLError, IMLRegression,
                              IContainsFlat
    {
        /// <summary>
        /// The underlying flat network.
        /// </summary>
        ///
        private readonly FlatNetworkRBF _flat;

        /// <summary>
        /// Construct RBF network.
        /// </summary>
        ///
        public RBFNetwork()
        {
            _flat = new FlatNetworkRBF();
        }

        /// <summary>
        /// Construct RBF network.
        /// </summary>
        ///
        /// <param name="inputCount">The input count.</param>
        /// <param name="hiddenCount">The hidden count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <param name="t">The RBF type.</param>
        public RBFNetwork(int inputCount, int hiddenCount,
                          int outputCount, RBFEnum t)
        {
            if (hiddenCount == 0)
            {
                throw new NeuralNetworkError(
                    "RBF network cannot have zero hidden neurons.");
            }

            var rbf = new IRadialBasisFunction[hiddenCount];

            // Set the standard RBF neuron width.
            // Literature seems to suggest this is a good default value.
            double volumeNeuronWidth = 2.0d/hiddenCount;

            _flat = new FlatNetworkRBF(inputCount, rbf.Length, outputCount, rbf);

            try
            {
                // try this
                SetRBFCentersAndWidthsEqualSpacing(-1, 1, t, volumeNeuronWidth,
                                                   false);
            }
            catch (EncogError)
            {
                // if we have the wrong number of hidden neurons, try this
                RandomizeRBFCentersAndWidths(-1, 1, t);
            }
        }

        /// <summary>
        /// Construct RBF network.
        /// </summary>
        ///
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <param name="rbf">The RBF type.</param>
        public RBFNetwork(int inputCount, int outputCount,
                          IRadialBasisFunction[] rbf)
        {
            _flat = new FlatNetworkRBF(inputCount, rbf.Length, outputCount, rbf) {RBF = rbf};
        }

        /// <summary>
        /// Set the RBF's.
        /// </summary>
        public IRadialBasisFunction[] RBF
        {
            get { return _flat.RBF; }
            set { _flat.RBF = value; }
        }

        #region ContainsFlat Members

        /// <inheritdoc/>
        public FlatNetwork Flat
        {
            get { return _flat; }
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <inheritdoc/>
        public IMLData Compute(IMLData input)
        {
			var output = new double[OutputCount];
            _flat.Compute(input, output);
			return new BasicMLData(output, false);
        }


        /// <inheritdoc/>
        public virtual int InputCount
        {
            get { return _flat.InputCount; }
        }


        /// <inheritdoc/>
        public virtual int OutputCount
        {
            get { return _flat.OutputCount; }
        }

        #endregion

        /// <summary>
        /// Set the RBF components to random values.
        /// </summary>
        ///
        /// <param name="min">Minimum random value.</param>
        /// <param name="max">Max random value.</param>
        /// <param name="t">The type of RBF to use.</param>
        public void RandomizeRBFCentersAndWidths(double min,
                                                 double max, RBFEnum t)
        {
            int dimensions = InputCount;
            var centers = new double[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                centers[i] = RangeRandomizer.Randomize(min, max);
            }

            for (int i = 0; i < _flat.RBF.Length; i++)
            {
                SetRBFFunction(i, t, centers, RangeRandomizer.Randomize(min, max));
            }
        }

        /// <summary>
        /// Array containing center position. Row n contains centers for neuron n.
        /// Row n contains x elements for x number of dimensions.
        /// </summary>
        ///
        /// <param name="centers">The centers.</param>
        /// <param name="widths"></param>
        /// <param name="t">The RBF Function to use for this layer.</param>
        public void SetRBFCentersAndWidths(double[][] centers,
                                           double[] widths, RBFEnum t)
        {
            for (int i = 0; i < _flat.RBF.Length; i++)
            {
                SetRBFFunction(i, t, centers[i], widths[i]);
            }
        }

        /// <summary>
        /// Equally spaces all hidden neurons within the n dimensional variable
        /// space.
        /// </summary>
        ///
        /// <param name="minPosition">The minimum position neurons should be centered. Typically 0.</param>
        /// <param name="maxPosition">The maximum position neurons should be centered. Typically 1</param>
        /// <param name="t">The RBF type.</param>
        /// <param name="volumeNeuronRBFWidth">The neuron width of neurons within the mesh.</param>
        /// <param name="useWideEdgeRBFs">Enables wider RBF's around the boundary of the neuron mesh.</param>
        public void SetRBFCentersAndWidthsEqualSpacing(
            double minPosition, double maxPosition,
            RBFEnum t, double volumeNeuronRBFWidth,
            bool useWideEdgeRBFs)
        {
            int totalNumHiddenNeurons = _flat.RBF.Length;

            int dimensions = InputCount;
            double disMinMaxPosition = Math.Abs(maxPosition - minPosition);

            // Check to make sure we have the correct number of neurons for the
            // provided dimensions
            var expectedSideLength = (int) Math.Pow(totalNumHiddenNeurons, 1.0d/dimensions);
            double cmp = Math.Pow(totalNumHiddenNeurons, 1.0d/dimensions);

            if (expectedSideLength != cmp)
            {
                throw new NeuralNetworkError(
                    "Total number of RBF neurons must be some integer to the power of 'dimensions'.\n"
                    + Format.FormatDouble(expectedSideLength, 5)
                    + " <> " + Format.FormatDouble(cmp, 5));
            }

            double edgeNeuronRBFWidth = 2.5d*volumeNeuronRBFWidth;

            var centers = new double[totalNumHiddenNeurons][];
            var widths = new double[totalNumHiddenNeurons];

            for (int i = 0; i < totalNumHiddenNeurons; i++)
            {
                centers[i] = new double[dimensions];

                int sideLength = expectedSideLength;

                // Evenly distribute the volume neurons.
                int temp = i;

                // First determine the centers
                for (int j = dimensions; j > 0; j--)
                {
                    // i + j * sidelength + k * sidelength ^2 + ... l * sidelength ^
                    // n
                    // i - neuron number in x direction, i.e. 0,1,2,3
                    // j - neuron number in y direction, i.e. 0,1,2,3
                    // Following example assumes sidelength of 4
                    // e.g Neuron 5 - x position is (int)5/4 * 0.33 = 0.33
                    // then take modulus of 5%4 = 1
                    // Neuron 5 - y position is (int)1/1 * 0.33 = 0.33
                    centers[i][j - 1] = ((int) (temp/Math.Pow(sideLength, j - 1))*(disMinMaxPosition/(sideLength - 1)))
                                        + minPosition;
                    temp = temp%(int) (Math.Pow(sideLength, j - 1));
                }

                // Now set the widths
                bool contains = false;

                for (int z = 0; z < centers[0].Length; z++)
                {
                    if ((centers[i][z] == 1.0d) || (centers[i][z] == 0.0d))
                    {
                        contains = true;
                    }
                }

                if (contains && useWideEdgeRBFs)
                {
                    widths[i] = edgeNeuronRBFWidth;
                }
                else
                {
                    widths[i] = volumeNeuronRBFWidth;
                }
            }

            SetRBFCentersAndWidths(centers, widths, t);
        }

        /// <summary>
        /// Set an RBF function.
        /// </summary>
        ///
        /// <param name="index">The index to set.</param>
        /// <param name="t">The function type.</param>
        /// <param name="centers">The centers.</param>
        /// <param name="width">The width.</param>
        public void SetRBFFunction(int index, RBFEnum t,
                                   double[] centers, double width)
        {
            if (t == RBFEnum.Gaussian)
            {
                _flat.RBF[index] = new GaussianFunction(0.5d, centers,
                                                       width);
            }
            else if (t == RBFEnum.Multiquadric)
            {
                _flat.RBF[index] = new MultiquadricFunction(0.5d, centers,
                                                           width);
            }
            else if (t == RBFEnum.InverseMultiquadric)
            {
                _flat.RBF[index] = new InverseMultiquadricFunction(0.5d,
                                                                  centers, width);
            }
        }

        /// <inheritdoc/>
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}
