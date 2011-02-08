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
using System.IO;
using encog_test.Encog.App.Quant;

namespace TestApp
{
    /// <summary>
    /// This app is really just a "sandbox" to test various things as Encog is developed.
    /// </summary>
    public class Program
    {
        public void TestNormalizeCSV()
        {
            NormalizeArray norm = new NormalizeArray();
            norm.NormalizedHigh = 1;
            norm.NormalizedLow = 0;
            double[] input = { 1, 5, 10 };
            double[] output = norm.Process(input);
            Console.WriteLine(output[0]);// 0
            Console.WriteLine(output[2]);// 1
            Console.WriteLine(norm.Stats.ActualHigh);//10
            Console.WriteLine(norm.Stats.ActualLow);//1
        }

        public void TestNormalizeArray()
        {
            GenerateTestFileHeadings();
            NormalizeCSV norm = new NormalizeCSV();
            norm.Analyze("test.csv", true, CSVFormat.ENGLISH);
            norm.Stats.Data[2].MakePassThrough();
            norm.Stats.Data[3].Action = NormalizationDesired.Ignore;
            norm.Stats.Data[4].Action = NormalizationDesired.Ignore;
            norm.Normalize("test2.csv");

            TextReader tr = new StreamReader("test2.csv");
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            tr.Close();

        }

        public void GenerateTestFileHeadings()
        {
            TextWriter tw = new StreamWriter("test.csv");
            
            tw.WriteLine("a,b,c,d,e");
            tw.WriteLine("one,1,1,2,3");
            tw.WriteLine("two,1,2,3,4");
            tw.WriteLine("three,2,3,4,5");
            tw.WriteLine("four,2,4,5,6");

            // close the stream
            tw.Close();
        }

        static void Main(string[] args)
        {
            TestNormalizeArray t = new TestNormalizeArray();
            t.TestNormalize();
            Console.WriteLine("Done");

        }
    }
}
