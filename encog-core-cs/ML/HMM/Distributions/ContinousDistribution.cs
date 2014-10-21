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
using Encog.MathUtil.Matrices.Decomposition;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.ML.HMM.Distributions
{
    /// <summary>
    /// A continuous distribution represents an infinite range of choices between two
    /// real numbers. A gaussian distribution is used to distribute the probability.
    /// </summary>
    [Serializable]
    public class ContinousDistribution : IStateDistribution
    {
        /// <summary>
        /// The covariance matrix.
        /// </summary>
        private readonly Matrix _covariance;

        /// <summary>
        /// The dimensions.
        /// </summary>
        private readonly int _dimension;

        /// <summary>
        /// The means for each dimension.
        /// </summary>
        private readonly double[] _mean;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private readonly GaussianRandomizer _randomizer = new GaussianRandomizer(0.0, 1.0);

        /// <summary>
        /// Used to perform a decomposition.
        /// </summary>
        private CholeskyDecomposition _cd;

        /// <summary>
        /// The covariance determinant.
        /// </summary>
        private double _covarianceDet;

        /// <summary>
        /// The covariance inverse.
        /// </summary>
        private Matrix _covarianceInv;

        /// <summary>
        /// The covariance left side.
        /// </summary>
        private Matrix _covarianceL;

        /// <summary>
        /// Construct a continuous distribution. 
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="covariance">The covariance.</param>
        public ContinousDistribution(double[] mean,
                                     double[][] covariance)
        {
            _dimension = covariance.Length;
            _mean = EngineArray.ArrayCopy(mean);
            _covariance = new Matrix(covariance);
            Update(covariance);
        }

        /// <summary>
        /// Construct a continuous distribution with the specified number of dimensions. 
        /// </summary>
        /// <param name="dimension">The dimensions.</param>
        public ContinousDistribution(int dimension)
        {
            if (dimension <= 0)
            {
                throw new EncogError("Invalid number of dimensions");
            }

            _dimension = dimension;
            _mean = new double[dimension];
            _covariance = new Matrix(dimension, dimension);
        }

        /// <summary>
        /// The mean for the dimensions of the gaussian curve.
        /// </summary>
        public double[] Mean
        {
            get { return _mean; }
        }

        /// <summary>
        /// The covariance matrix.
        /// </summary>
        public Matrix Covariance
        {
            get { return _covariance; }
        }

        #region IStateDistribution Members

        /// <inheritdoc/>
        public void Fit(IMLDataSet co)
        {
            var weights = new double[co.Count];
            EngineArray.Fill(weights, 1.0 / co.Count);

            Fit(co, weights);
        }

        /// <inheritdoc/>
        public void Fit(IMLDataSet co, double[] weights)
        {
            int i;

            if ((co.Count < 1) || (co.Count != weights.Length))
            {
                throw new EncogError("Invalid weight size");
            }

            // Compute mean
            var mean = new double[_dimension];
            for (int r = 0; r < _dimension; r++)
            {
                i = 0;

                foreach (IMLDataPair o in co)
                {
                    mean[r] += o.Input[r] * weights[i++];
                }
            }

            // Compute covariance
            double[][] covariance = EngineArray.AllocateDouble2D(_dimension, _dimension);
            i = 0;
            foreach (IMLDataPair o in co)
            {
                //double[] obs = o.Input.CreateCentroid();

                var omm = new double[o.Input.Count];

                for (int j = 0; j < omm.Length; j++)
                {
                    omm[j] = o.Input[j] - mean[j];
                }

                for (int r = 0; r < _dimension; r++)
                {
                    for (int c = 0; c < _dimension; c++)
                    {
                        covariance[r][c] += omm[r] * omm[c] * weights[i];
                    }
                }

                i++;
            }

            Update(covariance);
        }

        /// <inheritdoc/>
        public IMLDataPair Generate()
        {
            var d = new double[_dimension];

            for (int i = 0; i < _dimension; i++)
            {
                d[i] = _randomizer.Randomize(0);
            }

            double[] d2 = MatrixMath.Multiply(_covarianceL, d);
            return new BasicMLDataPair(new BasicMLData(EngineArray.Add(d2, _mean)));
        }


        /// <inheritdoc/>
        public double Probability(IMLDataPair o)
        {
            // double[] v = o.InputArray;
            //  Matrix vmm = Matrix.CreateColumnMatrix(EngineArray.Subtract(v,
            Matrix vmm = Matrix.CreateColumnMatrix(EngineArray.Subtract(o.Input,
                                                                        _mean));
            Matrix t = MatrixMath.Multiply(_covarianceInv, vmm);
            double expArg = MatrixMath.Multiply(MatrixMath.Transpose(vmm), t)
                                [0, 0] * -0.5;
            return Math.Exp(expArg)
                   / (Math.Pow(2.0 * Math.PI, _dimension / 2.0) * Math.Pow(
                       _covarianceDet, 0.5));
        }

        /// <inheritdoc/>
        IStateDistribution IStateDistribution.Clone()
        {
            return new ContinousDistribution((double[])_mean.Clone(), (double[][])_covariance.Data.Clone());
        }

        #endregion

        /// <summary>
        /// Update the covariance.  
        /// </summary>
        /// <param name="covariance">The new covariance.</param>
        public void Update(double[][] covariance)
        {
            _cd = new CholeskyDecomposition(new Matrix(covariance));
            _covarianceL = _cd.L;
            _covarianceInv = _cd.InverseCholesky();
            _covarianceDet = _cd.GetDeterminant();
        }
    }
}
