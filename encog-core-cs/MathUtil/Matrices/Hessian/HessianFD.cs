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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.MathUtil.Error;
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
        public const double INITIAL_STEP = 0.001;


        /// <summary>
        /// The derivative step size, used for the finite difference method.
        /// </summary>
        private double[] dStep;

        /// <summary>
        /// The derivative coefficient, used for the finite difference method.
        /// </summary>
        private double[] dCoeff;

        /// <summary>
        /// The center of the point array.
        /// </summary>
        private int center;

        /// <summary>
        /// The number of points requested per side.  This determines the accuracy of the calculation.
        /// </summary>
        private int pointsPerSide = 5;

        /// <summary>
        /// The number of points actually used, which is (pointsPerSide*2)+1. 
        /// </summary>
        private int pointCount;

        /// <inheritdoc/>
        public void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {

            base.Init(theNetwork, theTraining);
            int weightCount = theNetwork.Structure.Flat.Weights.Length;

            this.center = this.pointsPerSide + 1;
            this.pointCount = (this.pointsPerSide * 2) + 1;
            this.dCoeff = CreateCoefficients();
            this.dStep = new double[weightCount];

            for (int i = 0; i < weightCount; i++)
            {
                this.dStep[i] = INITIAL_STEP;
            }

        }

        /// <inheritdoc/>
        public override void Compute()
        {
            this.sse = 0;

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
            double e;

            int row = 0;
            ErrorCalculation error = new ErrorCalculation();
            EngineArray.Fill(this.derivative, 0);

            // Loop over every training element
            foreach (IMLDataPair pair in this.training)
            {
                IMLData networkOutput = this.network.Compute(pair.Input);

                e = pair.Ideal.Data[outputNeuron] - networkOutput[outputNeuron];
                error.UpdateError(networkOutput[outputNeuron], pair.Ideal[outputNeuron]);

                int currentWeight = 0;

                // loop over the output weights
                int outputFeedCount = network.GetLayerTotalNeuronCount(network.LayerCount - 2);
                for (int i = 0; i < this.network.OutputCount; i++)
                {
                    for (int j = 0; j < outputFeedCount; j++)
                    {
                        double jc;

                        if (i == outputNeuron)
                        {
                            jc = ComputeDerivative(pair.Input, outputNeuron,
                                    currentWeight, this.dStep,
                                    networkOutput[outputNeuron], row);
                        }
                        else
                        {
                            jc = 0;
                        }

                        this.gradients[currentWeight] += jc * e;
                        this.derivative[currentWeight] += jc;
                        currentWeight++;
                    }
                }

                // Loop over every weight in the neural network
                while (currentWeight < this.network.Flat.Weights.Length)
                {
                    double jc = ComputeDerivative(
                            pair.Input, outputNeuron, currentWeight,
                            this.dStep,
                            networkOutput[outputNeuron], row);
                    this.derivative[currentWeight] += jc;
                    this.gradients[currentWeight] += jc * e;
                    currentWeight++;
                }

                row++;
            }

            UpdateHessian(this.derivative);

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

            double[] points = new double[this.dCoeff.Length];

            stepSize[row] = Math.Max(INITIAL_STEP * Math.Abs(temp), INITIAL_STEP);

            points[this.center] = networkOutput;

            for (int i = 0; i < this.dCoeff.Length; i++)
            {
                if (i == this.center)
                    continue;

                double newWeight = temp + ((i - this.center))
                        * stepSize[row];

                this.network.Flat.Weights[weight] = newWeight;

                IMLData output = this.network.Compute(inputData);
                points[i] = output.Data[outputNeuron];
            }

            double result = 0.0;
            for (int i = 0; i < this.dCoeff.Length; i++)
            {
                result += this.dCoeff[i] * points[i];
            }

            result /= Math.Pow(stepSize[row], 1);

            this.network.Flat.Weights[weight] = temp;

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

            double[] result = new double[this.pointCount];

            Matrix delts = new Matrix(this.pointCount, this.pointCount);
            double[][] t = delts.Data;

            for (int j = 0; j < this.pointCount; j++)
            {
                double delt = (j - this.center);
                double x = 1.0;

                for (int k = 0; k < this.pointCount; k++)
                {
                    t[j][k] = x / EncogMath.Factorial(k);
                    x *= delt;
                }
            }

            Matrix invMatrix = delts.Inverse();
            double f = EncogMath.Factorial(this.pointCount);


            for (int k = 0; k < this.pointCount; k++)
            {
                result[k] = (Math.Round(invMatrix.Data[1][k] * f)) / f;
            }



            return result;
        }

        /// <summary>
        /// The number of points per side.
        /// </summary>
        public int PointsPerSide
        {
            get
            {
                return pointsPerSide;
            }
            set
            {
                this.pointsPerSide = value;
            }
        }        
    }
}
