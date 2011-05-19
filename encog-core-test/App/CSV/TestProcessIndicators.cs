using System.IO;
using Encog.App.Quant.Indicators;
using Encog.App.Quant.Indicators.Predictive;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestProcessIndicators
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        public void GenerateTestFileHeadings(bool header)
        {
            var tw = new StreamWriter(InputName.ToString());

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

        [TestMethod]
        public void TestIndicatorsHeaders()
        {
            GenerateTestFileHeadings(true);
            var norm = new ProcessIndicators();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.AddColumn(new MovingAverage(3, true));
            norm.AddColumn(new BestClose(3, true));
            norm.Columns[0].Output = true;
            norm.Process(OutputName);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("\"date\",\"close\",\"MovAvg\",\"PredictBestClose\"", tr.ReadLine());
            Assert.AreEqual("20100103,3,2,6", tr.ReadLine());
            Assert.AreEqual("20100104,4,3,7", tr.ReadLine());
            Assert.AreEqual("20100105,5,4,8", tr.ReadLine());
            Assert.AreEqual("20100106,6,5,9", tr.ReadLine());
            Assert.AreEqual("20100107,7,6,10", tr.ReadLine());

            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }

        [TestMethod]
        public void TestIndicatorsNoHeaders()
        {
            GenerateTestFileHeadings(false);
            var norm = new ProcessIndicators();
            norm.Analyze(InputName, false, CSVFormat.ENGLISH);
            norm.AddColumn(new MovingAverage(3, true));
            norm.AddColumn(new BestClose(3, true));
            norm.Columns[0].Output = true;
            norm.RenameColumn(1, "close");
            norm.Process(OutputName);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("20100103,3,2,6", tr.ReadLine());
            Assert.AreEqual("20100104,4,3,7", tr.ReadLine());
            Assert.AreEqual("20100105,5,4,8", tr.ReadLine());
            Assert.AreEqual("20100106,6,5,9", tr.ReadLine());
            Assert.AreEqual("20100107,7,6,10", tr.ReadLine());

            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}