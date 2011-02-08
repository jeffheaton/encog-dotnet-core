using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Normalize;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestNormalizeCSV
    {
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

        public void TestNormCSVHeaders()
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

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }
    }
}
