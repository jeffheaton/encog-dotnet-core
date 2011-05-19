using System.IO;
using Encog.App.Analyst.CSV.Balance;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestBalanceCSV
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        public void GenerateTestFile(bool header)
        {
            var file = new StreamWriter(InputName.ToString());

            if (header)
            {
                file.WriteLine("a,b");
            }
            file.WriteLine("one,1");
            file.WriteLine("two,1");
            file.WriteLine("three,1");
            file.WriteLine("four,2");
            file.WriteLine("five,2");
            file.WriteLine("six,3");

            // close the stream
            file.Close();
        }

        [TestMethod]
        public void TestBalanceCSVHeaders()
        {
            GenerateTestFile(true);
            var norm = new BalanceCSV();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Process(OutputName, 1, 2);

            var tr = new StreamReader(OutputName.ToString());

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

            InputName.Delete();
            OutputName.Delete();
        }

        [TestMethod]
        public void TestBalanceCSVNoHeaders()
        {
            GenerateTestFile(false);
            var norm = new BalanceCSV();
            norm.Analyze(InputName, false, CSVFormat.ENGLISH);
            norm.Process(OutputName, 1, 2);

            var tr = new StreamReader(OutputName.ToString());
            Assert.AreEqual("\"field:0\",\"field:1\"", tr.ReadLine());
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,1", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            Assert.AreEqual("five,2", tr.ReadLine());
            Assert.AreEqual("six,3", tr.ReadLine());
            Assert.AreEqual(2, norm.Counts["1"]);
            Assert.AreEqual(2, norm.Counts["2"]);
            Assert.AreEqual(1, norm.Counts["3"]);
            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}