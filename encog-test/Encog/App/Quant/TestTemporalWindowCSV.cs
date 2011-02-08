using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.App.Quant.Sort;
using Encog.Util.CSV;
using System.IO;
using Encog.App.Quant.Temporal;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestTemporalWindowCSV
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

            tw.WriteLine("100,1");
            tw.WriteLine("200,2");
            tw.WriteLine("300,3");
            tw.WriteLine("400,4");
            tw.WriteLine("500,5");
            tw.WriteLine("600,6");
            tw.WriteLine("700,7");
            tw.WriteLine("800,8");
            tw.WriteLine("900,9");
            tw.WriteLine("1000,10");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestTemp()
        {
            GenerateTestFileHeadings(true);
            TemporalWindowCSV norm = new TemporalWindowCSV();
            norm.InputWindow = 5;
            norm.PredictWindow = 1;
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);
            norm.Fields[0].Action = TemporalType.PassThrough;
            norm.Fields[1].Action = TemporalType.InputAndPredict;
            norm.Process(OUTPUT_NAME);
           
            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("a,Input:b(t),Input:b(t-1),Input:b(t-2),Input:b(t-3),Input:b(t-4),Predict:b(t+1)",tr.ReadLine());
            Assert.AreEqual("\"600\",1,2,3,4,5,6",tr.ReadLine());
            Assert.AreEqual("\"700\",2,3,4,5,6,7",tr.ReadLine());
            Assert.AreEqual("\"800\",3,4,5,6,7,8",tr.ReadLine());
            Assert.AreEqual("\"900\",4,5,6,7,8,9",tr.ReadLine());
            Assert.AreEqual("\"1000\",5,6,7,8,9,10",tr.ReadLine());
            Assert.IsNull(tr.ReadLine());

            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }

        [Test]
        public void TestTempNoHeaders()
        {
            GenerateTestFileHeadings(false);
            TemporalWindowCSV norm = new TemporalWindowCSV();
            norm.InputWindow = 5;
            norm.PredictWindow = 1;
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.Fields[0].Action = TemporalType.PassThrough;
            norm.Fields[1].Action = TemporalType.InputAndPredict;
            norm.Process(OUTPUT_NAME);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("\"600\",1,2,3,4,5,6", tr.ReadLine());
            Assert.AreEqual("\"700\",2,3,4,5,6,7", tr.ReadLine());
            Assert.AreEqual("\"800\",3,4,5,6,7,8", tr.ReadLine());
            Assert.AreEqual("\"900\",4,5,6,7,8,9", tr.ReadLine());
            Assert.AreEqual("\"1000\",5,6,7,8,9,10", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());

            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }

    }
}
