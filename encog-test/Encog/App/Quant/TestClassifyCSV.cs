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
            InternalTest(true);
            InternalTest(false);
        }

        [Test]
        public void TestKeepOrig()
        {
            GenerateTestFile(true);
            ClassifyCSV norm = new ClassifyCSV();
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH, 1);
            norm.Process(OUTPUT_NAME, ClassifyMethod.SingleField, -1, "org");

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());
            Console.WriteLine(tr.ReadLine());

            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT_NAME);
        }

        public void InternalTest(bool headers)
        {
            GenerateTestFile(headers);
            ClassifyCSV norm = new ClassifyCSV();
            norm.Analyze(INPUT_NAME, headers, CSVFormat.ENGLISH, 1);
            norm.Process(OUTPUT_NAME,ClassifyMethod.SingleField,-1,null);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            if( headers )
                Assert.AreEqual("\"a\",\"b\"",tr.ReadLine());
            Assert.AreEqual("one,0", tr.ReadLine());
            Assert.AreEqual("two,1",tr.ReadLine());
            Assert.AreEqual("three,2",tr.ReadLine());
            Assert.AreEqual("four,3",tr.ReadLine());

            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT_NAME);
        }

    }
}
