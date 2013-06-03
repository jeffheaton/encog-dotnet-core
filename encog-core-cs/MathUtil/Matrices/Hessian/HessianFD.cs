//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System.Linq;
using Encog.MathUtil.Error;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Util;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    /// Calculate the Hessian matrix using the finite difference method. This is a
    /// very simple method of calculating the Hessian. The algorithm does not vary
    /// greatly by number layers. This makes it very useful as a tool to check the
    /// accuracy of other methods of determining the Hessian.
    /// 
    /// For more information on the Finite Difference Method see the following article.
    /// 
    /// http://en.wikipedia.org/wiki/Finite_difference_method
    /// </summary>
    public class HessianFD : BasicHessian
    {
        /// <summary>
        /// The initial step size for dStep.
        /// </summary>
        public const double InitialStep = 0.001;


        /// <summary>
        /// The center of the point array.
        /// </summary>
        private int _center;

        /// <summary>
        /// The derivative coefficient, used for the finite difference method.
        /// </summary>
        private double[] _dCoeff;

        /// <summary>
        /// The derivative step size, used for the finite difference method.
        /// </summary>
        private double[] _dStep;

        /// <summary>
        /// The number of points actually used, which is (pointsPerSide*2)+1. 
        /// </summary>
        private int _pointCount;

        /// <summary>
        /// The number of points requested per side.  This determines the accuracy of the calculation.
        /// </summary>
        private int _pointsPerSide = 5;

        /// <summary>
        /// The weight count.
        /// </summary>
        private int _weightCount;

        /// <summary>
        /// The number of points per side.
        /// </summary>
        public int PointsPerSide
        {
            get { return _pointsPerSide; }
            set { _pointsPerSide = value; }
        }

        /// <inheritdoc/>
        public override void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {
            base.Init(theNetwork, theTraining);
            _weightCount = theNetwork.Structure.Flat.Weights.Length;

            _center = _pointsPerSide + 1;
            _pointCount = (_pointsPerSide*2) + 1;
            _dCoeff = CreateCoefficients();
            _dStep = new double[theTraining.Count];

            for (int i = 0; i < _weightCount; i++)
            {
                _dStep[i] = InitialStep;
            }
        }

        /// <inheritdoc/>
        public override void Compute()
        {
            sse = 0;

            for (int i = 0; i < network.OutputCount; i++)
            {
                InternalCompute(i);
            }
        }

        /// <summary>
        /// Called internally to compute each output neuron.
        /// </summary>
        /// <param name="outputNeuron">The output neuron to compute.</param>
        private void InternalCompute(int outputNeuron)
        {
            var row = 0;
            var error = new ErrorCalculation();
            var derivative = new double[_weightCount];

            // Loop over every training element
            foreach (var pair in training)
            {
                EngineArray.Fill(derivative, 0);
                var networkOutput = network.Compute(pair.Input);

                double e = pair.Ideal[outputNeuron] - networkOutput[outputNeuron];
                error.UpdateError(networkOutput[outputNeuron], pair.Ideal[outputNeuron]);

                int currentWeight = 0;

                // loop over the output weights
                int outputFeedCount = network.GetLayerTotalNeuronCount(network.LayerCount - 2);
                for (int i = 0; i < network.OutputCount; i++)
                {
                    for (int j = 0; j < outputFeedCount; j++)
                    {
                        double jc;

                        if (i == outputNeuron)
                        {
                            jc = ComputeDerivative(pair.Input, outputNeuron,
                                                   currentWeight, _dStep,
                                                   networkOutput[outputNeuron], row);
                        }
                        else
                        {
                            jc = 0;
                        }

                        gradients[currentWeight] += jc*e;
                        derivative[currentWeight] += jc;
                        currentWeight++;
                    }
                }

                // Loop over every weight in the neural network
                while (currentWeight < network.Flat.Weights.Length)
                {
                    double jc = ComputeDerivative(
                        pair.Input, outputNeuron, currentWeight,
                        _dStep,
                        networkOutput[outputNeuron], row);
                    derivative[currentWeight] += jc;
                    gradients[currentWeight] += jc*e;
                    currentWeight++;
                }

                row++;
                UpdateHessian(derivative);
            }

            

            sse += error.CalculateSSE();
        }

        /// <summary>
        /// Computes the derivative of the output of the neural network with respect to a weight. 
        /// </summary>
        /// <param name="inputData">The input data to the neural network.</param>
        /// <param name="outputNeuron">The output neuron to calculate for.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="stepSize">The step size.</param>
        /// <param name="networkOutput">The output from the neural network.</param>
        /// <param name="row">The training row currently being processed.</param>
        /// <returns>The derivative output.</returns>
        private double ComputeDerivative(IMLData inputData, int outputNeuron, int weight, double[] stepSize,
                                         double networkOutput, int row)
        {
            double temp = network.Flat.Weights[weight];

            var points = new double[_dCoeff.Length];

            stepSize[row] = Math.Max(InitialStep*Math.Abs(temp), InitialStep);

            points[_center] = networkOutput;

            for (int i = 0; i < _dCoeff.Length; i++)
            {
                if (i == _center)
                    continue;

                double newWeight = temp + ((i - _center))
                                   *stepSize[row];

                network.Flat.Weights[weight] = newWeight;

                IMLData output = network.Compute(inputData);
                points[i] = output[outputNeuron];
            }

            double result = _dCoeff.Select((t, i) => t*points[i]).Sum();

            result /= Math.Pow(stepSize[row], 1);

            network.Flat.Weights[weight] = temp;

            return result;
        }

        /// <summary>
        /// Compute finite difference coefficients according to the method provided here:
        /// 
        /// http://en.wikipedia.org/wiki/Finite_difference_coefficients
        ///
        /// </summary>
        /// <returns>An array of the coefficients for FD.</returns>
        public double[] CreateCoefficients()
        {
            var result = new double[_pointCount];

            var delts = new Matrix(_pointCount, _pointCount);
            double[][] t = delts.Data;

            for (int j = 0; j < _pointCount; j++)
            {
                double delt = (j - _center);
                double x = 1.0;

                for (int k = 0; k < _pointCount; k++)
                {
                    t[j][k] = x/EncogMath.Factorial(k);
                    x *= delt;
                }
            }

            Matrix invMatrix = delts.Inverse();
            double f = EncogMath.Factorial(_pointCount);


            for (int k = 0; k < _pointCount; k++)
            {
                result[k] = (Math.Round(invMatrix.Data[1][k]*f))/f;
            }


            return result;
        }
    }
}
