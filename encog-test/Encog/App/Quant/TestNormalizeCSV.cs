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
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT_NAME = "test2.csv";

        public void GenerateTestFileHeadings(bool header)
        {
            TextWriter tw = new StreamWriter(INPUT_NAME);

            if (header)
            {
                tw.WriteLine("a,b,c,d,e");
            }
            tw.WriteLine("one,1,1,2,3");
            tw.WriteLine("two,2,2,3,4");
            tw.WriteLine("three,3,3,4,5");
            tw.WriteLine("four,3,4,5,6");
            tw.WriteLine("five,2,5,6,7");
            tw.WriteLine("six,1,6,7,8");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestNormCSVHeaders()
        {
            GenerateTestFileHeadings(true);
            NormalizeCSV norm = new NormalizeCSV();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.Stats.Data[2].MakePassThrough();
            norm.Stats.Data[3].Action = NormalizationDesired.Ignore;
            norm.Stats.Data[4].Action = NormalizationDesired.Ignore;
            norm.Normalize(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);
            Assert.AreEqual("\"a\",\"b\",\"c\"",tr.ReadLine());
            Assert.AreEqual("\"one\",-1,\"1\"",tr.ReadLine());
            Assert.AreEqual("\"two\",0,\"2\"",tr.ReadLine());
            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }

        [Test]
        public void TestNormCSVNoHeaders()
        {
            GenerateTestFileHeadings(false);
            NormalizeCSV norm = new NormalizeCSV();
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.Stats.Data[2].MakePassThrough();
            norm.Stats.Data[3].Action = NormalizationDesired.Ignore;
            norm.Stats.Data[4].Action = NormalizationDesired.Ignore;
            norm.Normalize(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);
            Assert.AreEqual("\"one\",-1,\"1\"", tr.ReadLine());
            Assert.AreEqual("\"two\",0,\"2\"", tr.ReadLine());
            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }
    }
}
