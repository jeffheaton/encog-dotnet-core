using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.Util.Data
{
    public sealed class GenerationUtil
    {
        public delegate double EncogFunction(double x);

        /// <summary>
        /// Private constructor.
        /// </summary>
        private GenerationUtil()
        {
        }

        public static IMLDataSet GenerateSingleDataRange(EncogFunction task, double start, double stop, double step)
        {
            BasicMLDataSet result = new BasicMLDataSet();
            double current = start;


            while (current <= stop)
            {
                BasicMLData input = new BasicMLData(1);
                input[0] = current;
                BasicMLData ideal = new BasicMLData(1);
                ideal[0] = task(current);
                result.Add(input, ideal);
                current += step;
            }

            return result;
        }
    }
}
