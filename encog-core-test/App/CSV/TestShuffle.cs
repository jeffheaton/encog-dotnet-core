using System;
using System.Collections.Generic;
using System.IO;
using Encog.App.Analyst.CSV.Shuffle;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestShuffle
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        public void GenerateTestFileHeadings(bool header)
        {
            var tw = new StreamWriter(InputName.ToString());

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

        public void TestShuffleHeaders()
        {
            GenerateTestFileHeadings(true);
            var norm = new ShuffleCSV();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Process(OutputName);

            var tr = new StreamReader(OutputName.ToString());
            String line;
            IDictionary<String, int> list = new Dictionary<String, int>();

            while ((line = tr.ReadLine()) != null)
            {
                list[line] = 0;
            }

            Assert.AreEqual(7, list.Count);

            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }


        public void TestShuffleNoHeaders()
        {
            GenerateTestFileHeadings(false);
            var norm = new ShuffleCSV();
            norm.Analyze(InputName, false, CSVFormat.ENGLISH);
            norm.ProduceOutputHeaders = false;
            norm.Process(OutputName);

            var tr = new StreamReader(OutputName.ToString());
            String line;
            IDictionary<String, int> list = new Dictionary<String, int>();

            while ((line = tr.ReadLine()) != null)
            {
                list[line] = 0;
            }

            Assert.AreEqual(6, list.Count);

            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}