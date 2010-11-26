using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.MarketDB;

namespace Encog.App.Quant.Loader
{
    public class SplitRateCollection
    {
        private IList<SplitRatio> data = new List<SplitRatio>();
        private static SplitRateCollection instance;

        public static SplitRateCollection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SplitRateCollection();
                    instance.Populate();
                }
                return instance;
            }
        }


        public IList<SplitRatio> Data
        {
            get
            {
                return this.data;
            }
        }

        public SplitRatio FindClosest(double d)
        {
            double bestDiff = double.PositiveInfinity;
            SplitRatio result = new SplitRatio();

            foreach(SplitRatio ratio in this.Data )
            {
                double diff = Math.Abs(ratio.ratio - d);

                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    result = ratio;
                }
            }

            return result;
        }

        public void Populate()
        {
            IDictionary<String, Object> exists = new Dictionary<String, Object>();

            for (uint denominator = 1; denominator <= 10; denominator++)
            {
                for (uint numerator = 1; numerator <= 10; numerator++)
                {
                    SplitRatio ratio = new SplitRatio();
                    ratio.denominator = denominator;
                    ratio.numerator = numerator;
                    ratio.Reduce();

                    if (!exists.ContainsKey(ratio.ToString()))
                    {
                        ratio.ratio = (double)ratio.numerator / (double)ratio.denominator;
                        exists.Add(ratio.ToString(),null);
                        data.Add(ratio);
                    }
                }
            }
        }
    }
}
