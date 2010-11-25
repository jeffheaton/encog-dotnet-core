using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Market.DB
{
    public struct SplitRatio
    {
        public uint numerator;
        public uint denominator;
        public double ratio;

        public void Reduce()
        {
            uint max = Math.Max(numerator, denominator);

            for (double current = max; current >= 2; current--)
            {
                if ((numerator % current) == 0 && (denominator % current) == 0)
                {
                    numerator /= (uint)current;
                    denominator /= (uint)current;
                    return;
                }
            }
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(numerator);
            result.Append(" for ");
            result.Append(denominator);
            return result.ToString();
        }
    }
}
