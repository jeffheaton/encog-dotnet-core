using System.IO;
using Encog.App.Analyst.CSV.Filter;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestFilter
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
        public void TestFilterCSVHeaders()
        {
            GenerateTestFile(true);
            var norm = new FilterCSV();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Exclude(1, "1");
            norm.Process(OutputName);

            var tr = new StreamReader(OutputName.ToString());
            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }

        [TestMethod]
        public void TestFilterCSVNoHeaders()
        {
            GenerateTestFile(false);
            var norm = new FilterCSV();
            norm.Analyze(InputName, false, CSVFormat.ENGLISH);
            norm.Exclude(1, "1");
            norm.Process(OutputName);

            var tr = new StreamReader(OutputName.ToString());
            Assert.AreEqual("\"field:0\",\"field:1\"", tr.ReadLine());
            Assert.AreEqual("four,2", tr.ReadLine());
            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}