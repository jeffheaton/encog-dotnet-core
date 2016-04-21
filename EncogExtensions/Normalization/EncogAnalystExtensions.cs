using Encog.App.Analyst;
using Encog.App.Analyst.Analyze;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncogExtensions.Normalization
{
    public static class EncogAnalystExtensions
    {
        public static void Analyze(this EncogAnalyst analyst, DataSet data)
        {
            var a = new PerformAnalysis(analyst.Script);
            a.Process(analyst, data);// Kiran:2
        }
    }
}
