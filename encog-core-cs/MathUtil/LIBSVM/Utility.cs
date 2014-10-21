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
using Encog.Util;

namespace Encog.MathUtil.LIBSVM
{
    public class Utility
    {
        public static double[][] AllocateDouble2D(int d1, int d2)
        {
            double[][] result = new double[d1][];

            for (int i = 0; i < d1; i++)
            {
                result[i] = new double[d2];
            }

            return result;
        }

        public static float[][] AllocateFloat2D(int d1, int d2)
        {
            float[][] result = new float[d1][];

            for (int i = 0; i < d1; i++)
            {
                result[i] = new float[d2];
            }

            return result;
        }








        internal static void arraycopy(int[] source, int sourceIndex, int[] target, int targetIndex, int size)
        {
            EngineArray.ArrayCopy(source, sourceIndex, target, targetIndex, size);
        }

        internal static void arraycopy(float[] source, int sourceIndex, float[] target, int targetIndex, int size)
        {
            EngineArray.ArrayCopy(source, sourceIndex, target, targetIndex, size);
        }

        internal static int random(Random rand, int p)
        {
            return (int)(rand.NextDouble()*p);
        }
    }
}
