using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestNinjaFileConvert
    {
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT_NAME = "test2.csv";

        public void GenerateTestFileHeadings(bool header)
        {
            TextWriter tw = new StreamWriter(INPUT_NAME);

            if (header)
            {
                tw.WriteLine("date,time,open,high,low,close,volume");
            }
            tw.WriteLine("20100101,000000,10,12,8,9,1000");
            tw.WriteLine("20100102,000000,9,17,7,15,1000");


            // close the stream
            tw.Close();
        }

        [Test]
        public void TestConvert()
        {
            GenerateTestFileHeadings(true);
            NinjaFileConvert norm = new NinjaFileConvert();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("20100101 000000;10;12;8;9;1000",tr.ReadLine());
            Assert.AreEqual("20100102 000000;9;17;7;15;1000", tr.ReadLine());

            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }
    }
}
