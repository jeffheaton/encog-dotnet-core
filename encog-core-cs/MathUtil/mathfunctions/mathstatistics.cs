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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.MathFunctions
{
    /// <summary>
    /// Class to calculate average , Variance , and Pearson's correlation.
    /// </summary>
    public class MathStatistics
    {

        public static double GetMax(double[] DoubleCollection)
        {
            double max = double.MinValue;
            foreach (double i in DoubleCollection)
            {
                if (i > max)
                {
                    max = i;
                }
            }
            return max;
        }
        public static double GetMin(double[] DoubleCollection)
        {
            double max = double.MaxValue;
            foreach (double i in DoubleCollection)
            {
                if (i < max)
                {
                    max = i;
                }
            }
            return max;
        }

        //Overload for System.Collections.ObjectModel.Collection<>
        public static double GetMax(System.Collections.ObjectModel.Collection<double> DoubleCollection)
        {
            double max = double.MinValue;
            foreach (double i in DoubleCollection)
            {
                if (i > max)
                {
                    max = i;
                }
            }
            return max;
        }
        //Overload for System.Collections.ObjectModel.Collection<>
        public static double GetMin(System.Collections.ObjectModel.Collection<double> DoubleCollection)
        {
            double max = double.MaxValue;
            foreach (double i in DoubleCollection)
            {
                if (i < max)
                {
                    max = i;
                }
            }
            return max;
        } 

    


        /// <summary>
        /// Get average
        /// </summary>
        public static double GetAverage(double[] data)
        {
            int len = data.Length;
            if (len == 0)
                throw new Exception("No data");

            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += data[i];
            return sum / len;
        }

        /// <summary>
        /// Get variance
        /// </summary>
        public static double GetVariance(double[] data)
        {
            int len = data.Length;
            // Get average

            double avg = GetAverage(data);

            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += Math.Pow((data[i] - avg), 2);
            return sum / len;
        }


        /// <summary>
        /// Get standard deviation
        /// </summary>
        public static double GetStdev(double[] data)
        {
            return Math.Sqrt(GetVariance(data));
        }

        /// <summary>

        /// Get correlation
        /// To calculate covariance we need to get the average and standard deviation for each variable.
        /// We sum up the multiplication of x - Avg(x) and y - Avg(y) and finally divide it by the length of the variables.
        ///To get the pearson value we divide the covariance by the multiplication of stDevX and stDevY.
        /// </summary>

        public static void GetCorrelation(double[] x, double[] y, ref double covXY, ref double pearson)
        {
            if (x.Length != y.Length)
                throw new Exception("Length of sources is different");
            double avgX = GetAverage(x);
            double stdevX = GetStdev(x);
            double avgY = GetAverage(y);
            double stdevY = GetStdev(y);
            int len = x.Length;

            for (int i = 0; i < len; i++)
                covXY += (x[i] - avgX) * (y[i] - avgY);
            covXY /= len;
            pearson = covXY / (stdevX * stdevY);
        }


    }

}
