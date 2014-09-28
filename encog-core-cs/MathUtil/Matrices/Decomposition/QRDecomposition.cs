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
namespace Encog.MathUtil.Matrices.Decomposition
{
    /// <summary>
    /// QR Decomposition.
    ///
    /// For an m-by-n matrix A with m >= n, the QR decomposition is an m-by-n
    /// orthogonal matrix Q and an n-by-n upper triangular matrix R so that A = Q*R.
    ///
    /// The QR decompostion always exists, even if the matrix does not have full
    /// rank, so the constructor will never fail. The primary use of the QR
    /// decomposition is in the least squares solution of nonsquare systems of
    /// simultaneous linear equations. This will fail if isFullRank() returns false.
    /// 
    /// This file based on a class from the public domain JAMA package.
    /// http://math.nist.gov/javanumerics/jama/
    /// </summary>
    public class QRDecomposition
    {
        /// <summary>
        /// Array for internal storage of decomposition.
        /// </summary>
        private readonly double[][] QR;

        /// <summary>
        /// Array for internal storage of diagonal of R.
        /// </summary>
        private readonly double[] Rdiag;

        /// <summary>
        /// Row dimension.
        /// </summary>
        private readonly int m;

        /// <summary>
        /// Column dimension.
        /// </summary>
        private readonly int n;

        /// <summary>
        /// QR Decomposition, computed by Householder reflections.
        /// </summary>
        /// <param name="A">Structure to access R and the Householder vectors and compute Q.</param>
        public QRDecomposition(Matrix A)
        {
            // Initialize.
            QR = A.GetArrayCopy();
            m = A.Rows;
            n = A.Cols;
            Rdiag = new double[n];

            // Main loop.
            for (int k = 0; k < n; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (int i = k; i < m; i++)
                {
                    nrm = EncogMath.Hypot(nrm, QR[i][k]);
                }

                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (QR[k][k] < 0)
                    {
                        nrm = -nrm;
                    }
                    for (int i = k; i < m; i++)
                    {
                        QR[i][k] /= nrm;
                    }
                    QR[k][k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++)
                    {
                        double s = 0.0;
                        for (int i = k; i < m; i++)
                        {
                            s += QR[i][k]*QR[i][j];
                        }
                        s = -s/QR[k][k];
                        for (int i = k; i < m; i++)
                        {
                            QR[i][j] += s*QR[i][k];
                        }
                    }
                }
                Rdiag[k] = -nrm;
            }
        }

        /// <summary>
        /// Return the Householder vectors
        /// </summary>
        public Matrix H
        {
            get
            {
                var x = new Matrix(m, n);
                double[][] h = x.Data;
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i >= j)
                        {
                            h[i][j] = QR[i][j];
                        }
                        else
                        {
                            h[i][j] = 0.0;
                        }
                    }
                }
                return x;
            }
        }

        /**
         * Return the upper triangular factor
         * 
         * @return R
         */

        public Matrix R
        {
            get
            {
                var x = new Matrix(n, n);
                double[][] r = x.Data;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i < j)
                        {
                            r[i][j] = QR[i][j];
                        }
                        else if (i == j)
                        {
                            r[i][j] = Rdiag[i];
                        }
                        else
                        {
                            r[i][j] = 0.0;
                        }
                    }
                }
                return x;
            }
        }

        /// <summary>
        /// Generate and return the (economy-sized) orthogonal factor
        /// </summary>
        public Matrix Q
        {
            get
            {
                var x = new Matrix(m, n);
                double[][] q = x.Data;
                for (int k = n - 1; k >= 0; k--)
                {
                    for (int i = 0; i < m; i++)
                    {
                        q[i][k] = 0.0;
                    }
                    q[k][k] = 1.0;
                    for (int j = k; j < n; j++)
                    {
                        if (QR[k][k] != 0)
                        {
                            double s = 0.0;
                            for (int i = k; i < m; i++)
                            {
                                s += QR[i][k]*q[i][j];
                            }
                            s = -s/QR[k][k];
                            for (int i = k; i < m; i++)
                            {
                                q[i][j] += s*QR[i][k];
                            }
                        }
                    }
                }
                return x;
            }
        }

        /// <summary>
        /// Is the matrix full rank? 
        /// </summary>
        /// <returns>true if R, and hence A, has full rank.</returns>
        public bool IsFullRank()
        {
            for (int j = 0; j < n; j++)
            {
                if (Rdiag[j] == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Least squares solution of A*X = B
        /// </summary>
        /// <param name="B">A Matrix with as many rows as A and any number of columns.</param>
        /// <returns>that minimizes the two norm of Q*R*X-B.</returns>
        public Matrix Solve(Matrix B)
        {
            if (B.Rows != m)
            {
                throw new MatrixError(
                    "Matrix row dimensions must agree.");
            }
            if (!IsFullRank())
            {
                throw new MatrixError("Matrix is rank deficient.");
            }

            // Copy right hand side
            int nx = B.Cols;
            double[][] X = B.GetArrayCopy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < nx; j++)
                {
                    double s = 0.0;
                    for (int i = k; i < m; i++)
                    {
                        s += QR[i][k]*X[i][j];
                    }
                    s = -s/QR[k][k];
                    for (int i = k; i < m; i++)
                    {
                        X[i][j] += s*QR[i][k];
                    }
                }
            }
            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X[k][j] /= Rdiag[k];
                }
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j]*QR[i][k];
                    }
                }
            }
            return (new Matrix(X).GetMatrix(0, n - 1, 0, nx - 1));
        }
    }
}
