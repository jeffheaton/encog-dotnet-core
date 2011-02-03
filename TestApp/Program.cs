using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;
using Encog.App.Quant.Indicators;
using Encog.App.Quant.Basic;
using Encog.App.Quant.Indicators.Predictive;
using Encog.App.Quant.Normalize;
using Encog.Persist;

namespace TestApp
{
    /// <summary>
    /// This app is really just a "sandbox" to test various things as Encog is developed.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            /*EncogNormalize norm = new EncogNormalize();
            norm.Analyze("d:\\data\\ge.csv",true,CSVFormat.ENGLISH);
            norm.Normalize("d:\\data\\ge.norm");*/
            EncogMemoryCollection encog = new EncogMemoryCollection();
            //encog.Add("stats", norm.Stats);
            encog.Load("c:\\temp\\step5_network.eg");
            NormalizationStats stats = (NormalizationStats)encog.Find("stat");
            Console.WriteLine(stats);

        }
    }
}
