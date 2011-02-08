using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Classify;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestClassifyCSV
    {
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT_NAME = "test2.csv";

        public void GenerateTestFile(bool header)
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

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestTheClassifyCSV()
        {
            InternalTest(true, ClassifyMethod.SingleField);
            InternalTest(false, ClassifyMethod.OneOf);
            InternalTest(false, ClassifyMethod.Equilateral);
        }

        [Test]
        public void TestKeepOrig()
        {
            GenerateTestFile(true);
            ClassifyCSV norm = new ClassifyCSV();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH, 1);
            norm.Process(OUTPUT_NAME, ClassifyMethod.SingleField, -1, "org");

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("\"a\",\"org\",\"b\"",tr.ReadLine());
            Assert.AreEqual("one,1,0",tr.ReadLine());
            Assert.AreEqual("two,2,1",tr.ReadLine());
            Assert.AreEqual("three,3,2", tr.ReadLine());
            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT_NAME);
        }

        public void InternalTest(bool headers, ClassifyMethod method)
        {
            GenerateTestFile(headers);
            ClassifyCSV norm = new ClassifyCSV();
            norm.Precision = 4;
            norm.Analyze(INPUT_NAME, headers, CSVFormat.ENGLISH, 1);
            norm.Process(OUTPUT_NAME,method,-1,null);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            if( headers )
                Assert.AreEqual("\"a\",\"b\"",tr.ReadLine());

            switch( method )
            {
                case ClassifyMethod.SingleField:
                    Assert.AreEqual("one,0", tr.ReadLine());
                    Assert.AreEqual("two,1",tr.ReadLine());
                    Assert.AreEqual("three,2",tr.ReadLine());
                    Assert.AreEqual("four,3",tr.ReadLine());
                    break;
                case ClassifyMethod.Equilateral:
                    Assert.AreEqual("one,-0.8165,-0.4714,-0.3333",tr.ReadLine());
                    Assert.AreEqual("two,0.8165,-0.4714,-0.3333",tr.ReadLine());
                    Assert.AreEqual("three,0.0000,0.9428,-0.3333",tr.ReadLine());
                    Assert.AreEqual("four,0.0000,0.0000,1.0000", tr.ReadLine());
                    break;
                case ClassifyMethod.OneOf:
                    Assert.AreEqual("one,1,-1,-1,-1", tr.ReadLine());
                    Assert.AreEqual("two,-1,1,-1,-1", tr.ReadLine());
                    Assert.AreEqual("three,-1,-1,1,-1", tr.ReadLine());
                    Assert.AreEqual("four,-1,-1,-1,1", tr.ReadLine());
                    break;
            }

            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT_NAME);
        }

    }
}
