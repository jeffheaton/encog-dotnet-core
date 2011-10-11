using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Arrayutil
{
    public class CompareArray
    {
        public static bool Compare(double[] d1, double[] d2, double p)
        {
            for (int i = 0; i < d1.Length;i++ )
            {
                double diff = Math.Abs(d1[i] - d2[i]);
                if (diff > p)
                    return false;
            }
            return true;
        }
    }
}
