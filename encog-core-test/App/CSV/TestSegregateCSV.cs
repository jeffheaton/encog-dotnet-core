using System.IO;
using Encog.App.Analyst.CSV.Segregate;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestSegregateCSV
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName1 = TempDir.CreateFile("test2.csv");
        public readonly FileInfo OutputName2 = TempDir.CreateFile("test3.csv");

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

            // close the stream
            tw.Close();
        }

        [TestMethod]
        public void TestFilterCSVHeaders()
        {
            GenerateTestFileHeadings(true);
            var norm = new SegregateCSV();
            norm.Targets.Add(new SegregateTargetPercent(OutputName1, 75));
            norm.Targets.Add(new SegregateTargetPercent(OutputName2, 25));
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Process();

            var tr = new StreamReader(OutputName1.ToString());
            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,2", tr.ReadLine());
            Assert.AreEqual("three,3", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            tr = new StreamReader(OutputName2.ToString());
            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("four,4", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            InputName.Delete();
            OutputName1.Delete();
            OutputName1.Delete();
        }

        [TestMethod]
        public void TestFilterCSVNoHeaders()
        {
            GenerateTestFileHeadings(false);
            var norm = new SegregateCSV();
            norm.Targets.Add(new SegregateTargetPercent(OutputName1, 75));
            norm.Targets.Add(new SegregateTargetPercent(OutputName2, 25));
            norm.Analyze(InputName, false, CSVFormat.ENGLISH);
            norm.ProduceOutputHeaders = false;
            norm.Process();

            var tr = new StreamReader(OutputName1.ToString());
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,2", tr.ReadLine());
            Assert.AreEqual("three,3", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            tr = new StreamReader(OutputName2.ToString());
            Assert.AreEqual("four,4", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            InputName.Delete();
            OutputName1.Delete();
            OutputName2.Delete();
        }
    }
}