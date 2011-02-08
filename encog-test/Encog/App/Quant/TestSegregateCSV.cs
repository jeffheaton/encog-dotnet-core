using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Segregate;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestSegregateCSV
    {
        public const String INPUT_NAME = "test.csv";
        public const String OUTPUT1_NAME = "test2.csv";
        public const String OUTPUT2_NAME = "test3.csv";

        public void GenerateTestFileHeadings(bool header)
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
        public void TestFilterCSVHeaders()
        {
            GenerateTestFileHeadings(true);
            SegregateCSV norm = new SegregateCSV();
            norm.Targets.Add(new SegregateTargetPercent(OUTPUT1_NAME, 75));
            norm.Targets.Add(new SegregateTargetPercent(OUTPUT2_NAME, 25));
            norm.Analyze(INPUT_NAME, true, CSVFormat.ENGLISH);            
            norm.Process();

            TextReader tr = new StreamReader(OUTPUT1_NAME);
            Assert.AreEqual("\"a\",\"b\"",tr.ReadLine());
            Assert.AreEqual("one,1",tr.ReadLine());
            Assert.AreEqual("two,2",tr.ReadLine());
            Assert.AreEqual("three,3", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            tr = new StreamReader(OUTPUT2_NAME);
            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("four,4", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT1_NAME);
            File.Delete(OUTPUT2_NAME);
        }

        [Test]
        public void TestFilterCSVNoHeaders()
        {
            GenerateTestFileHeadings(false);
            SegregateCSV norm = new SegregateCSV();
            norm.Targets.Add(new SegregateTargetPercent(OUTPUT1_NAME, 75));
            norm.Targets.Add(new SegregateTargetPercent(OUTPUT2_NAME, 25));
            norm.Analyze(INPUT_NAME, false, CSVFormat.ENGLISH);
            norm.Process();

            TextReader tr = new StreamReader(OUTPUT1_NAME);
            Assert.AreEqual("one,1", tr.ReadLine());
            Assert.AreEqual("two,2", tr.ReadLine());
            Assert.AreEqual("three,3", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            tr = new StreamReader(OUTPUT2_NAME);
            Assert.AreEqual("four,4", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());
            tr.Close();

            File.Delete(INPUT_NAME);
            File.Delete(OUTPUT1_NAME);
            File.Delete(OUTPUT2_NAME);
        }

      
    }
}
