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
using Encog.MathUtil.RBF;
using Encog.Util;

namespace Encog.Neural.RBF.Training
{
    /// <summary>
    /// Perform a SVD decomp on a matrix.
    /// Contributed to Encog By M.Fletcher and M.Dean University of Cambridge, Dept.
    /// of Physics, UK
    /// </summary>
    ///
    public class SVD
    {
        /// <summary>
        /// Perform a SVD fit.
        /// </summary>
        /// <param name="x">The X matrix.</param>
        /// <param name="y">The Y matrix.</param>
        /// <param name="a">The A matrix.</param>
        /// <param name="funcs">The RBF functions.</param>
        /// <returns>The fit.</returns>
        public static double Svdfit(double[][] x, double[][] y, double[][] a,
                                    IRadialBasisFunction[] funcs)
        {
            int i, j, k;
            double wmax, tmp, thresh, sum, TOL = 1e-13d;

            //Allocated memory for svd matrices
            double[][] u = EngineArray.AllocateDouble2D(x.Length, funcs.Length);
            double[][] v = EngineArray.AllocateDouble2D(funcs.Length, funcs.Length);
            var w = new double[funcs.Length];

            //Fill input matrix with values based on fitting functions and input coordinates 
            for (i = 0; i < x.Length; i++)
            {
                for (j = 0; j < funcs.Length; j++)
                    u[i][j] = funcs[j].Calculate(x[i]);
            }

            //Perform decomposition
            Svdcmp(u, w, v);

            //Check for w values that are close to zero and replace them with zeros such that they are ignored in backsub
            wmax = 0;
            for (j = 0; j < funcs.Length; j++)
                if (w[j] > wmax)
                    wmax = w[j];

            thresh = TOL*wmax;

            for (j = 0; j < funcs.Length; j++)
                if (w[j] < thresh)
                    w[j] = 0;

            //Perform back substitution to get result
            Svdbksb(u, w, v, y, a);

            //Calculate chi squared for the fit
            double chisq = 0;
            for (k = 0; k < y[0].Length; k++)
            {
                for (i = 0; i < y.Length; i++)
                {
                    sum = 0.0d;
                    for (j = 0; j < funcs.Length; j++)
                        sum += a[j][k]*funcs[j].Calculate(x[i]);
                    tmp = (y[i][k] - sum);
                    chisq += tmp*tmp;
                }
            }

            return Math.Sqrt(chisq/(y.Length*y[0].Length));
        }

        public static void Svdbksb(double[][] u, double[] w, double[][] v,
                                   double[][] b, double[][] x)
        {
            int jj, j, i, m, n, k;
            double s;

            m = u.Length;
            n = u[0].Length;

            var temp = new double[n];

            for (k = 0; k < b[0].Length; k++)
            {
                for (j = 0; j < n; j++)
                {
                    s = 0;

                    if (w[j] != 0)
                    {
                        for (i = 0; i < m; i++)
                            s += u[i][j]*b[i][k];
                        s /= w[j];
                    }
                    temp[j] = s;
                }

                for (j = 0; j < n; j++)
                {
                    s = 0;
                    for (jj = 0; jj < n; jj++)
                        s += v[j][jj]*temp[jj];
                    x[j][k] = s;
                }
            }
        }

        /// <summary>
        /// Given a matrix a[1..m][1..n], this routine computes its singular value
        /// decomposition, A = U.W.VT.  The matrix U replaces a on output.  The diagonal
        /// matrix of singular values W is output as a vector w[1..n].  The matrix V (not
        /// the transpose VT) is output as v[1..n][1..n].
        /// </summary>
        /// <param name="a"></param>
        /// <param name="w"></param>
        /// <param name="v"></param>
        public static void Svdcmp(double[][] a, double[] w, double[][] v)
        {
            bool flag;
            int i, its, j, jj, k, l = 0, nm = 0;
            double anorm, c, f, g, h, s, scale, x, y, z;

            int m = a.Length;
            int n = a[0].Length;
            var rv1 = new double[n];
            g = scale = anorm = 0.0d;
            for (i = 0; i < n; i++)
            {
                l = i + 2;
                rv1[i] = scale*g;
                g = s = scale = 0.0d;
                if (i < m)
                {
                    for (k = i; k < m; k++)
                        scale += Math.Abs(a[k][i]);
                    if (scale != 0.0d)
                    {
                        for (k = i; k < m; k++)
                        {
                            a[k][i] /= scale;
                            s += a[k][i]*a[k][i];
                        }
                        f = a[i][i];
                        g = -SIGN(Math.Sqrt(s), f);
                        h = f*g - s;
                        a[i][i] = f - g;
                        for (j = l - 1; j < n; j++)
                        {
                            for (s = 0.0d, k = i; k < m; k++)
                                s += a[k][i]*a[k][j];
                            f = s/h;
                            for (k = i; k < m; k++)
                                a[k][j] += f*a[k][i];
                        }
                        for (k = i; k < m; k++)
                            a[k][i] *= scale;
                    }
                }
                w[i] = scale*g;
                g = s = scale = 0.0d;
                if (i + 1 <= m && i + 1 != n)
                {
                    for (k = l - 1; k < n; k++)
                        scale += Math.Abs(a[i][k]);
                    if (scale != 0.0d)
                    {
                        for (k = l - 1; k < n; k++)
                        {
                            a[i][k] /= scale;
                            s += a[i][k]*a[i][k];
                        }
                        f = a[i][l - 1];
                        g = -SIGN(Math.Sqrt(s), f);
                        h = f*g - s;
                        a[i][l - 1] = f - g;
                        for (k = l - 1; k < n; k++)
                            rv1[k] = a[i][k]/h;
                        for (j = l - 1; j < m; j++)
                        {
                            for (s = 0.0d, k = l - 1; k < n; k++)
                                s += a[j][k]*a[i][k];
                            for (k = l - 1; k < n; k++)
                                a[j][k] += s*rv1[k];
                        }
                        for (k = l - 1; k < n; k++)
                            a[i][k] *= scale;
                    }
                }
                anorm = MAX(anorm, (Math.Abs(w[i]) + Math.Abs(rv1[i])));
            }
            for (i = n - 1; i >= 0; i--)
            {
                if (i < n - 1)
                {
                    if (g != 0.0d)
                    {
                        for (j = l; j < n; j++)
                            v[j][i] = (a[i][j]/a[i][l])/g;
                        for (j = l; j < n; j++)
                        {
                            for (s = 0.0d, k = l; k < n; k++)
                                s += a[i][k]*v[k][j];
                            for (k = l; k < n; k++)
                                v[k][j] += s*v[k][i];
                        }
                    }
                    for (j = l; j < n; j++)
                        v[i][j] = v[j][i] = 0.0d;
                }
                v[i][i] = 1.0d;
                g = rv1[i];
                l = i;
            }
            for (i = MIN(m, n) - 1; i >= 0; i--)
            {
                l = i + 1;
                g = w[i];
                for (j = l; j < n; j++)
                    a[i][j] = 0.0d;
                if (g != 0.0d)
                {
                    g = 1.0d/g;
                    for (j = l; j < n; j++)
                    {
                        for (s = 0.0d, k = l; k < m; k++)
                            s += a[k][i]*a[k][j];
                        f = (s/a[i][i])*g;
                        for (k = i; k < m; k++)
                            a[k][j] += f*a[k][i];
                    }
                    for (j = i; j < m; j++)
                        a[j][i] *= g;
                }
                else
                    for (j = i; j < m; j++)
                        a[j][i] = 0.0d;
                ++a[i][i];
            }
            for (k = n - 1; k >= 0; k--)
            {
                for (its = 0; its < 30; its++)
                {
                    flag = true;
                    for (l = k; l >= 0; l--)
                    {
                        nm = l - 1;
                        if (Math.Abs(rv1[l]) + anorm == anorm)
                        {
                            flag = false;
                            break;
                        }
                        if (Math.Abs(w[nm]) + anorm == anorm)
                            break;
                    }
                    if (flag)
                    {
                        c = 0.0d;
                        s = 1.0d;
                        for (i = l; i < k + 1; i++)
                        {
                            f = s*rv1[i];
                            rv1[i] = c*rv1[i];
                            if (Math.Abs(f) + anorm == anorm)
                                break;
                            g = w[i];
                            h = Pythag(f, g);
                            w[i] = h;
                            h = 1.0d/h;
                            c = g*h;
                            s = -f*h;
                            for (j = 0; j < m; j++)
                            {
                                y = a[j][nm];
                                z = a[j][i];
                                a[j][nm] = y*c + z*s;
                                a[j][i] = z*c - y*s;
                            }
                        }
                    }
                    z = w[k];
                    if (l == k)
                    {
                        if (z < 0.0d)
                        {
                            w[k] = -z;
                            for (j = 0; j < n; j++)
                                v[j][k] = -v[j][k];
                        }
                        break;
                    }
                    if (its == 29)
                    {
                        //	Debug.Print("no convergence in 30 svdcmp iterations");
                    }
                    x = w[l];
                    nm = k - 1;
                    y = w[nm];
                    g = rv1[nm];
                    h = rv1[k];
                    f = ((y - z)*(y + z) + (g - h)*(g + h))/(2.0d*h*y);
                    g = Pythag(f, 1.0d);
                    f = ((x - z)*(x + z) + h*((y/(f + SIGN(g, f))) - h))/x;
                    c = s = 1.0d;
                    for (j = l; j <= nm; j++)
                    {
                        i = j + 1;
                        g = rv1[i];
                        y = w[i];
                        h = s*g;
                        g = c*g;
                        z = Pythag(f, h);
                        rv1[j] = z;
                        c = f/z;
                        s = h/z;
                        f = x*c + g*s;
                        g = g*c - x*s;
                        h = y*s;
                        y *= c;
                        for (jj = 0; jj < n; jj++)
                        {
                            x = v[jj][j];
                            z = v[jj][i];
                            v[jj][j] = x*c + z*s;
                            v[jj][i] = z*c - x*s;
                        }
                        z = Pythag(f, h);
                        w[j] = z;
                        if (z != 0)
                        {
                            z = 1.0d/z;
                            c = f*z;
                            s = h*z;
                        }
                        f = c*g + s*y;
                        x = c*y - s*g;
                        for (jj = 0; jj < m; jj++)
                        {
                            y = a[jj][j];
                            z = a[jj][i];
                            a[jj][j] = y*c + z*s;
                            a[jj][i] = z*c - y*s;
                        }
                    }
                    rv1[l] = 0.0d;
                    rv1[k] = f;
                    w[k] = x;
                }
            }
        }

        /// <summary>
        /// Take the min of two numbers.
        /// </summary>
        /// <param name="m">First number.</param>
        /// <param name="n">Second number.</param>
        /// <returns>The min.</returns>
        public static int MIN(int m, int n)
        {
            return (m < n) ? m : n;
        }

        /// <summary>
        /// Take the max of two numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The max.</returns>
        public static double MAX(double a, double b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// Take the sign of two numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns></returns>
        public static double SIGN(double a, double b)
        {
            return (((b) >= 0.0d) ? Math.Abs(a) : -Math.Abs(a));
        }

        /// <summary>
        /// Compute the pythag distance of two numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The result.</returns>
        public static double Pythag(double a, double b)
        {
            double absa, absb;
            absa = Math.Abs(a);
            absb = Math.Abs(b);
            if (absa > absb)
                return absa*Math.Sqrt(1.0d + (absb/absa)*(absb/absa));
            else
                return ((absb == 0.0d)
                            ? 0.0d
                            : absb
                              *Math.Sqrt(1.0d + (absa/absb)*(absa/absb)));
        }
    }
}
