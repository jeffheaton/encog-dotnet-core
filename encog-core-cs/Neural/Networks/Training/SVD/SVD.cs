// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Encog.Neural.Networks.Training.SVD
{
    /// <summary>
    /// Contains methods for computing singular value decompositions of matrices and solving them against multiple right hand sides
    /// 
    // Contributed to Encog By M.Dean and M.Fletcher
    // University of Cambridge, Dept. of Physics, UK
    /// </summary>
    internal static class SVD
    {
        internal static void svdfit(double[][] x, double[][] y, double[][] a, out double chisq, Func<double[], double>[] funcs)
        {
            int i, j, k;
            double wmax, tmp, thresh, sum, TOL = 1e-13;

            //Allocated memory for svd matrices
            double[,] u = new double[x.Length, funcs.Length];
            double[,] v = new double[funcs.Length, funcs.Length];
            double[] w = new double[funcs.Length];

            //Fill input matrix with values based on fitting functions and input coordinates 
            for (i = 0; i < x.Length; i++)
            {
                for (j = 0; j < funcs.Length; j++)
                    u[i, j] = funcs[j](x[i]);
            }

            //Perform decomposition
            svdcmp(u, w, v);

            //Check for w values that are close to zero and replace them with zeros such that they are ignored in backsub
            wmax = 0;
            for (j = 0; j < funcs.Length; j++)
                if (w[j] > wmax) wmax = w[j];

            thresh = TOL * wmax;

            for (j = 0; j < funcs.Length; j++)
                if (w[j] < thresh) w[j] = 0;

            //Perform back substitution to get result
            svdbksb(u, w, v, y, a);

            //Calculate chi squared for the fit
            chisq = 0;
            for (k = 0; k < y[0].Length; k++)
            {
                for (i = 0; i < y.Length; i++)
                {
                    sum = 0.0;
                    for (j = 0; j < funcs.Length; j++) sum += a[j][k] * funcs[j](x[i]);
                    tmp = (y[i][k] - sum);
                    chisq += tmp * tmp;
                }
            }

            chisq = Math.Sqrt(chisq / (y.Length * y[0].Length)); 
        }

        internal static void svdbksb(double[,] u, double[] w, double[,] v, double[][] b, double[][] x)
        {
            int jj, j, i, m, n, k;
            double s;

            m = u.GetLength(0); n = u.GetLength(1);

            double[] temp = new double[n];

            for (k = 0; k < b[0].Length; k++)
            {

                for (j = 0; j < n; j++)
                {
                    s = 0;

                    if (w[j] != 0)
                    {
                        for (i = 0; i < m; i++)
                            s += u[i, j] * b[i][k];
                        s /= w[j];
                    }
                    temp[j] = s;
                }

                for (j = 0; j < n; j++)
                {
                    s = 0;
                    for (jj = 0; jj < n; jj++)
                        s += v[j, jj] * temp[jj];
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
        internal static void svdcmp(double[,] a, double[] w, double[,] v)        
        {
            bool flag;
            int i, its, j, jj, k, l = 0, nm = 0;
            double anorm, c, f, g, h, s, scale, x, y, z;

            int m = a.GetLength(0);
            int n = a.GetLength(1);
            double[] rv1 = new double[n];
            g = scale = anorm = 0.0;
            for (i = 0; i < n; i++)
            {
                l = i + 2;
                rv1[i] = scale * g;
                g = s = scale = 0.0;
                if (i < m)
                {
                    for (k = i; k < m; k++) scale += Math.Abs(a[k, i]);
                    if (scale != 0.0)
                    {
                        for (k = i; k < m; k++)
                        {
                            a[k, i] /= scale;
                            s += a[k, i] * a[k, i];
                        }
                        f = a[i, i];
                        g = -SIGN(Math.Sqrt(s), f);
                        h = f * g - s;
                        a[i, i] = f - g;
                        for (j = l - 1; j < n; j++)
                        {
                            for (s = 0.0, k = i; k < m; k++) s += a[k, i] * a[k, j];
                            f = s / h;
                            for (k = i; k < m; k++) a[k, j] += f * a[k, i];
                        }
                        for (k = i; k < m; k++) a[k, i] *= scale;
                    }
                }
                w[i] = scale * g;
                g = s = scale = 0.0;
                if (i + 1 <= m && i + 1 != n)
                {
                    for (k = l - 1; k < n; k++) scale += Math.Abs(a[i, k]);
                    if (scale != 0.0)
                    {
                        for (k = l - 1; k < n; k++)
                        {
                            a[i, k] /= scale;
                            s += a[i, k] * a[i, k];
                        }
                        f = a[i, l - 1];
                        g = -SIGN(Math.Sqrt(s), f);
                        h = f * g - s;
                        a[i, l - 1] = f - g;
                        for (k = l - 1; k < n; k++) rv1[k] = a[i, k] / h;
                        for (j = l - 1; j < m; j++)
                        {
                            for (s = 0.0, k = l - 1; k < n; k++) s += a[j, k] * a[i, k];
                            for (k = l - 1; k < n; k++) a[j, k] += s * rv1[k];
                        }
                        for (k = l - 1; k < n; k++) a[i, k] *= scale;
                    }
                }
                anorm = MAX(anorm, (Math.Abs(w[i]) + Math.Abs(rv1[i])));
            }
            for (i = n - 1; i >= 0; i--)
            {
                if (i < n - 1)
                {
                    if (g != 0.0)
                    {
                        for (j = l; j < n; j++)
                            v[j, i] = (a[i, j] / a[i, l]) / g;
                        for (j = l; j < n; j++)
                        {
                            for (s = 0.0, k = l; k < n; k++) s += a[i, k] * v[k, j];
                            for (k = l; k < n; k++) v[k, j] += s * v[k, i];
                        }
                    }
                    for (j = l; j < n; j++) v[i, j] = v[j, i] = 0.0;
                }
                v[i, i] = 1.0;
                g = rv1[i];
                l = i;
            }
            for (i = MIN(m, n) - 1; i >= 0; i--)
            {
                l = i + 1;
                g = w[i];
                for (j = l; j < n; j++) a[i, j] = 0.0;
                if (g != 0.0)
                {
                    g = 1.0 / g;
                    for (j = l; j < n; j++)
                    {
                        for (s = 0.0, k = l; k < m; k++) s += a[k, i] * a[k, j];
                        f = (s / a[i, i]) * g;
                        for (k = i; k < m; k++) a[k, j] += f * a[k, i];
                    }
                    for (j = i; j < m; j++) a[j, i] *= g;
                }
                else for (j = i; j < m; j++) a[j, i] = 0.0;
                ++a[i, i];
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
                        if (Math.Abs(w[nm]) + anorm == anorm) break;
                    }
                    if (flag)
                    {
                        c = 0.0;
                        s = 1.0;
                        for (i = l; i < k + 1; i++)
                        {
                            f = s * rv1[i];
                            rv1[i] = c * rv1[i];
                            if (Math.Abs(f) + anorm == anorm) break;
                            g = w[i];
                            h = pythag(f, g);
                            w[i] = h;
                            h = 1.0 / h;
                            c = g * h;
                            s = -f * h;
                            for (j = 0; j < m; j++)
                            {
                                y = a[j, nm];
                                z = a[j, i];
                                a[j, nm] = y * c + z * s;
                                a[j, i] = z * c - y * s;
                            }
                        }
                    }
                    z = w[k];
                    if (l == k)
                    {
                        if (z < 0.0)
                        {
                            w[k] = -z;
                            for (j = 0; j < n; j++) v[j, k] = -v[j, k];
                        }
                        break;
                    }
#if !SILVERLIGHT
                    if (its == 29) Debug.Print("no convergence in 30 svdcmp iterations");
#endif
                    x = w[l];
                    nm = k - 1;
                    y = w[nm];
                    g = rv1[nm];
                    h = rv1[k];
                    f = ((y - z) * (y + z) + (g - h) * (g + h)) / (2.0 * h * y);
                    g = pythag(f, 1.0);
                    f = ((x - z) * (x + z) + h * ((y / (f + SIGN(g, f))) - h)) / x;
                    c = s = 1.0;
                    for (j = l; j <= nm; j++)
                    {
                        i = j + 1;
                        g = rv1[i];
                        y = w[i];
                        h = s * g;
                        g = c * g;
                        z = pythag(f, h);
                        rv1[j] = z;
                        c = f / z;
                        s = h / z;
                        f = x * c + g * s;
                        g = g * c - x * s;
                        h = y * s;
                        y *= c;
                        for (jj = 0; jj < n; jj++)
                        {
                            x = v[jj, j];
                            z = v[jj, i];
                            v[jj, j] = x * c + z * s;
                            v[jj, i] = z * c - x * s;
                        }
                        z = pythag(f, h);
                        w[j] = z;
                        if (z != 0)
                        {
                            z = 1.0 / z;
                            c = f * z;
                            s = h * z;
                        }
                        f = c * g + s * y;
                        x = c * y - s * g;
                        for (jj = 0; jj < m; jj++)
                        {
                            y = a[jj, j];
                            z = a[jj, i];
                            a[jj, j] = y * c + z * s;
                            a[jj, i] = z * c - y * s;
                        }
                    }
                    rv1[l] = 0.0;
                    rv1[k] = f;
                    w[k] = x;
                }
            }
        }

        private static int MIN(int m, int n)
        {
            return m < n ? m : n;
        }

        private static double MAX(double a, double b)
        {
            return (a > b) ? a : b;
        }

        private static double SIGN(double a, double b)
        {
            return ((b) >= 0.0 ? Math.Abs(a) : -Math.Abs(a));
        }

        private static double pythag(double a, double b)
        {
            double absa, absb;
            absa = Math.Abs(a);
            absb = Math.Abs(b);
            if (absa > absb) return absa * Math.Sqrt(1.0 + (absb / absa) * (absb / absa));
            else return (absb == 0.0 ? 0.0 : absb * Math.Sqrt(1.0 + (absa / absb) * (absa / absb)));
        }
    }
}
