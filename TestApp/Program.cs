using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;
using Encog.App.Quant.Indicators;
using Encog.App.Quant.Basic;
using Encog.App.Quant.Indicators.Predictive;

namespace TestApp
{
    /// <summary>
    /// This app is really just a "sandbox" to test various things as Encog is developed.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            ProcessIndicators indicators = new ProcessIndicators();
            indicators.Analyze("d:\\data\\test.csv", true, CSVFormat.ENGLISH);
            indicators.Columns[1].Output = false;
            indicators.AddColumn(new MovingAverage(3, true));
            indicators.AddColumn(new BestReturn(3, true));
            indicators.Process("d:\\data\\test2.csv");
        }
    }
}
