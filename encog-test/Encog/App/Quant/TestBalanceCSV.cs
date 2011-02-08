using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Balance;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestBalanceCSV
    {
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT_NAME = "test2.csv";

        public void GenerateTestFile(bool header)
        {
            TextWriter tw = new StreamWriter(INPUT_NAME);

            if (header)
            {
                tw.WriteLine("a,b");
            }
            tw.WriteLine("one,1");
            tw.WriteLine("two,1");
            tw.WriteLine("three,1");
            tw.WriteLine("four,2");
            tw.WriteLine("five,2");
            tw.WriteLine("six,3");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestBalanceCSVHeaders()
        {
            GenerateTestFile(true);
            BalanceCSV norm = new BalanceCSV();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.Process(OUTPUT_NAME, 1, 2);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,1", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            Assert.AreEqual("five,2", tr.ReadLine());
            Assert.AreEqual("six,3", tr.ReadLine());
            Assert.AreEqual(2, norm.Counts["1"]);
            Assert.AreEqual(2, norm.Counts["2"]);
            Assert.AreEqual(1, norm.Counts["3"]);
            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT_NAME);
        }

        [Test]
        public void TestBalanceCSVNoHeaders()
        {
            GenerateTestFile(false);
            BalanceCSV norm = new BalanceCSV();
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.Process(OUTPUT_NAME, 1, 2);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,1", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            Assert.AreEqual("five,2", tr.ReadLine());
            Assert.AreEqual("six,3", tr.ReadLine());
            Assert.AreEqual(2, norm.Counts["1"]);
            Assert.AreEqual(2, norm.Counts["2"]);
            Assert.AreEqual(1, norm.Counts["3"]);
            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT_NAME);
        }

    }
}
