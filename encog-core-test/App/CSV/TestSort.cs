using System.IO;
using Encog.App.Analyst.CSV.Sort;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestSort
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

            tw.WriteLine("five,5");
            tw.WriteLine("four,4");
            tw.WriteLine("two,2");
            tw.WriteLine("three,3");
            tw.WriteLine("six,6");
            tw.WriteLine("one,1");

            // close the stream
            tw.Close();
        }

        [TestMethod]
        public void TestSortHeaders()
        {
            GenerateTestFileHeadings(true);
            var norm = new SortCSV();
            norm.SortOrder.Add(new SortedField(1, SortType.SortString, true));
            norm.Process(InputName, OutputName, true, CSVFormat.ENGLISH);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("\"one\",1", tr.ReadLine());
            Assert.AreEqual("\"two\",2", tr.ReadLine());
            Assert.AreEqual("\"three\",3", tr.ReadLine());
            Assert.AreEqual("\"four\",4", tr.ReadLine());
            Assert.AreEqual("\"five\",5", tr.ReadLine());
            Assert.AreEqual("\"six\",6", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());


            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }

        [TestMethod]
        public void TestSortNoHeaders()
        {
            GenerateTestFileHeadings(false);
            var norm = new SortCSV();
            norm.SortOrder.Add(new SortedField(1, SortType.SortInteger, true));
            norm.ProduceOutputHeaders = false;
            norm.Process(InputName, OutputName, false, CSVFormat.ENGLISH);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("\"one\",1", tr.ReadLine());
            Assert.AreEqual("\"two\",2", tr.ReadLine());
            Assert.AreEqual("\"three\",3", tr.ReadLine());
            Assert.AreEqual("\"four\",4", tr.ReadLine());
            Assert.AreEqual("\"five\",5", tr.ReadLine());
            Assert.AreEqual("\"six\",6", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());


            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}