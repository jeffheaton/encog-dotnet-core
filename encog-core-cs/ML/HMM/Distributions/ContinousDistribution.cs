using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Matrices;
using Encog.MathUtil.Matrices.Decomposition;
using Encog.Util;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.MathUtil.Randomize;

namespace Encog.ML.HMM.Distributions
{
    /// <summary>
    /// A continuous distribution represents an infinite range of choices between two
    /// real numbers. A gaussian distribution is used to distribute the probability.
    /// </summary>
    public class ContinousDistribution : IStateDistribution
    {
        /// <summary>
        /// The dimensions.
        /// </summary>
        private int dimension;

        /// <summary>
        /// The means for each dimension.
        /// </summary>
        private double[] mean;

        /// <summary>
        /// The covariance matrix.
        /// </summary>
        private Matrix covariance;

        /// <summary>
        /// The covariance left side.
        /// </summary>
        private Matrix covarianceL = null;

        /// <summary>
        /// The covariance inverse.
        /// </summary>
        private Matrix covarianceInv = null;

        /// <summary>
        /// The covariance determinant.
        /// </summary>
        private double covarianceDet;

        /// <summary>
        /// Used to perform a decomposition.
        /// </summary>
        private CholeskyDecomposition cd;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private GaussianRandomizer randomizer = new GaussianRandomizer(0.0, 1.0);

        /// <summary>
        /// Construct a continuous distribution. 
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="covariance">The covariance.</param>
        public ContinousDistribution(double[] mean,
                double[][] covariance)
        {
            this.dimension = covariance.Length;
            this.mean = EngineArray.ArrayCopy(mean);
            this.covariance = new Matrix(covariance);
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

            this.dimension = dimension;
            this.mean = new double[dimension];
            this.covariance = new Matrix(dimension, dimension);

        }

        /// <inheritdoc/>
        public void Fit(IMLDataSet co)
        {
            double[] weights = new double[co.Count];
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
            double[] mean = new double[this.dimension];
            for (int r = 0; r < this.dimension; r++)
            {
                i = 0;

                foreach (IMLDataPair o in co)
                {
                    mean[r] += o.Input[r] * weights[i++];
                }
            }

            // Compute covariance
            double[][] covariance = EngineArray.AllocateDouble2D(this.dimension, this.dimension);
            i = 0;
            foreach (IMLDataPair o in co)
            {
                double[] obs = o.Input.Data;
                double[] omm = new double[obs.Length];

                for (int j = 0; j < obs.Length; j++)
                {
                    omm[j] = obs[j] - mean[j];
                }

                for (int r = 0; r < this.dimension; r++)
                {
                    for (int c = 0; c < this.dimension; c++)
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
            double[] d = new double[this.dimension];

            for (int i = 0; i < this.dimension; i++)
            {
                d[i] = this.randomizer.Randomize(0);
            }

            double[] d2 = MatrixMath.Multiply(this.covarianceL, d);
            return new BasicMLDataPair(new BasicMLData(EngineArray.Add(d2,
                    this.mean)));
        }

        /// <inheritdoc/>
        public double Probability(IMLDataPair o)
        {
            double[] v = o.InputArray;
            Matrix vmm = Matrix.CreateColumnMatrix(EngineArray.Subtract(v,
                    this.mean));
            Matrix t = MatrixMath.Multiply(this.covarianceInv, vmm);
            double expArg = MatrixMath.Multiply(MatrixMath.Transpose(vmm), t)
                    [0, 0] * -0.5;
            return Math.Exp(expArg)
                    / (Math.Pow(2.0 * Math.PI, this.dimension / 2.0) * Math.Pow(
                            this.covarianceDet, 0.5));
        }

        /// <summary>
        /// Update the covariance.  
        /// </summary>
        /// <param name="covariance">The new covariance.</param>
        public void Update(double[][] covariance)
        {
            this.cd = new CholeskyDecomposition(new Matrix(covariance));
            this.covarianceL = this.cd.L;
            this.covarianceInv = this.cd.InverseCholesky();
            this.covarianceDet = this.cd.GetDeterminant();
        }

        /// <summary>
        /// The mean for the dimensions of the gaussian curve.
        /// </summary>
        public double[] Mean
        {
            get
            {
                return this.mean;
            }
        }

        /// <summary>
        /// The covariance matrix.
        /// </summary>
        public Matrix Covariance
        {
            get
            {
                return this.covariance;
            }
        }

        /// <inheritdoc/>
        IStateDistribution IStateDistribution.Clone()
        {
            return null;
        }
    }
}
