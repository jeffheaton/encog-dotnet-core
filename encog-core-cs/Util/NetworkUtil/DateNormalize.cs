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
using Encog.MathUtil;

namespace Encog.Util.NetworkUtil
{
    /// <summary>
    /// Use this class to normalize a date with hours, days, months, year, seconds.
    /// This class uses the equilateral normalization.
    /// </summary>
    public class DateNormalize
    {

        #region normalizes days and month in a date.

        public double[] NormalizeMonth(int month)
        {
            var eq = new Equilateral(12, -1, 1);
            return eq.Encode(month);
        }

        public int DenormalizeMonth(double[] montharry)
        {
            var eq = new Equilateral(12, -1, 1);
            return eq.Decode(montharry);
        }

        public double[] NormalizeDays(int days)
        {
            var eq = new Equilateral(31, -1, 1);
            return eq.Encode(days);
        }
        public int DenormalizeDays(double[] days)
        {
            var eq = new Equilateral(31, -1, 1);
            return eq.Decode(days);
        }
        public double[] NormalizeHour(int hour)
        {
            var eq = new Equilateral(24, -1, 1);
            return eq.Encode(hour);
        }
        public int DenormalizeHour(double[] hour)
        {
            var eq = new Equilateral(24, -1, 1);
            return eq.Decode(hour);
        }

        public double[] NormalizeSeconds(int Seconds)
        {
            var eq = new Equilateral(60, -1, 1);
            return eq.Encode(Seconds);
        }
        public int DenormalizeSeconds(double[] seconds)
        {
            var eq = new Equilateral(60, -1, 1);
            return eq.Decode(seconds);
        }


        public double[] NormalizeYear(int year)
        {
            var eq = new Equilateral(2011, -1, 1);
            return eq.Encode(year);
        }
        public static int DenormalizeYear(double[] year)
        {
            var eq = new Equilateral(2011, -1, 1);
            return eq.Decode(year);
        }
        public double[] NormalizeYear(int hour, int MaxYear)
        {
            var eq = new Equilateral(MaxYear, -1, 1);
            return eq.Encode(hour);
        }
        public int DenormalizeYear(double[] days, int MaxYear)
        {
            var eq = new Equilateral(MaxYear, -1, 1);
            return eq.Decode(days);
        }

        #endregion
    }
}
