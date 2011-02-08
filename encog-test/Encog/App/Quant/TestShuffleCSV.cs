using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Shuffle;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestShuffleCSV
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
            tw.WriteLine("two,2");
            tw.WriteLine("three,3");
            tw.WriteLine("four,4");
            tw.WriteLine("five,5");
            tw.WriteLine("six,6");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestShuffleHeaders()
        {
            GenerateTestFileHeadings(true);
            ShuffleCSV norm = new ShuffleCSV();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);
            String line;
            IDictionary<String, int> list = new Dictionary<String, int>();

            while ((line = tr.ReadLine()) != null)
            {
                list.Add(line, 0);
            }

            Assert.AreEqual(7,list.Count);

            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }


        [Test]
        public void TestShuffleNoHeaders()
        {
            GenerateTestFileHeadings(false);
            ShuffleCSV norm = new ShuffleCSV();
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);
            String line;
            IDictionary<String, int> list = new Dictionary<String, int>();

            while ((line = tr.ReadLine()) != null)
            {
                list.Add(line, 0);
            }

            Assert.AreEqual(6, list.Count);

            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }


    }
}
