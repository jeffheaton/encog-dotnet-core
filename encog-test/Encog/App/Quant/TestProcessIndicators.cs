using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Indicators;
using Encog.Util.CSV;
using Encog.App.Quant.Indicators.Predictive;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestProcessIndicators
    {
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT_NAME = "test2.csv";

        public void GenerateTestFileHeadings(bool header)
        {
            TextWriter tw = new StreamWriter(INPUT_NAME);

            if (header)
            {
                tw.WriteLine("date,close");
            }
            tw.WriteLine("20100101,1");
            tw.WriteLine("20100102,2");
            tw.WriteLine("20100103,3");
            tw.WriteLine("20100104,4");
            tw.WriteLine("20100105,5");
            tw.WriteLine("20100106,6");
            tw.WriteLine("20100107,7");
            tw.WriteLine("20100108,8");
            tw.WriteLine("20100109,9");
            tw.WriteLine("20100110,10");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestIndicatorsHeaders()
        {
            GenerateTestFileHeadings(true);
            ProcessIndicators norm = new ProcessIndicators();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.AddColumn(new MovingAverage(3, true));
            norm.AddColumn(new BestClose(3,true));
            norm.Columns[0].Output = true;
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("\"date\",\"close\",\"MovAvg\",\"PredictBestClose\"", tr.ReadLine());
            Assert.AreEqual("20100103,3,2,6", tr.ReadLine());
            Assert.AreEqual("20100104,4,3,7", tr.ReadLine());
            Assert.AreEqual("20100105,5,4,8", tr.ReadLine());
            Assert.AreEqual("20100106,6,5,9", tr.ReadLine());
            Assert.AreEqual("20100107,7,6,10", tr.ReadLine());

            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }

        [Test]
        public void TestIndicatorsNoHeaders()
        {
            GenerateTestFileHeadings(false);
            ProcessIndicators norm = new ProcessIndicators();
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.AddColumn(new MovingAverage(3, true));
            norm.AddColumn(new BestClose(3, true));
            norm.Columns[0].Output = true;
            norm.RenameColumn(1, "close");
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("20100103,3,2,6", tr.ReadLine());
            Assert.AreEqual("20100104,4,3,7", tr.ReadLine());
            Assert.AreEqual("20100105,5,4,8", tr.ReadLine());
            Assert.AreEqual("20100106,6,5,9", tr.ReadLine());
            Assert.AreEqual("20100107,7,6,10", tr.ReadLine());

            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }
    }
}
