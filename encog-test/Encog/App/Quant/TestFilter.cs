using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Filter;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestFilter
    {
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT_NAME = "test2.csv";

        public void GenerateTestFileHeadings(bool header)
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
            tw.WriteLine("five,1");
            tw.WriteLine("six,1");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestFilterCSVHeaders()
        {
            GenerateTestFileHeadings(true);
            FilterCSV norm = new FilterCSV();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.Exclude(1, "1");
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);
            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }

        [Test]
        public void TestFilterCSVNoHeaders()
        {
            GenerateTestFileHeadings(false);
            FilterCSV norm = new FilterCSV();
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.Exclude(1, "1");
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);
            Assert.AreEqual("four,2", tr.ReadLine());
            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }


    }
}
