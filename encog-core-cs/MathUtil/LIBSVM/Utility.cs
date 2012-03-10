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
