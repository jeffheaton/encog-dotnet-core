using System.IO;
using Encog.App.Quant.Ninja;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestNinjaFileConvert
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");


        public void GenerateTestFileHeadings(bool header)
        {
            var tw = new StreamWriter(InputName.ToString());
                
            if (header)
            {
                tw.WriteLine("date,time,open,high,low,close,volume");
            }
            tw.WriteLine("20100101,000000,10,12,8,9,1000");
            tw.WriteLine("20100102,000000,9,17,7,15,1000");


            // close the stream
            tw.Close();
        }

        [TestMethod]
        public void TestConvert()
        {
            GenerateTestFileHeadings(true);
            var norm = new NinjaFileConvert();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Process(OutputName.ToString());

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("20100101 000000;10;12;8;9;1000", tr.ReadLine());
            Assert.AreEqual("20100102 000000;9;17;7;15;1000", tr.ReadLine());

            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}