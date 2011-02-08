using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Encog.App.Quant.Sort;
using Encog.Util.CSV;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestSortCSV
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

            tw.WriteLine("five,5");
            tw.WriteLine("four,4");
            tw.WriteLine("two,2");
            tw.WriteLine("three,3");                      
            tw.WriteLine("six,6");
            tw.WriteLine("one,1");

            // close the stream
            tw.Close();
        }

        [Test]
        public void TestSortHeaders()
        {
            GenerateTestFileHeadings(true);
            SortCSV norm = new SortCSV();
            norm.SortOrder.Add(new SortedField(1,SortType.SortString,true));
            norm.Process(INPUT_NAME,OUTPUT_NAME,true,CSVFormat.ENGLISH);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("\"one\",1", tr.ReadLine());
            Assert.AreEqual("\"two\",2", tr.ReadLine());
            Assert.AreEqual("\"three\",3", tr.ReadLine());
            Assert.AreEqual("\"four\",4", tr.ReadLine());
            Assert.AreEqual("\"five\",5", tr.ReadLine());
            Assert.AreEqual("\"six\",6", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());


            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }

        [Test]
        public void TestSortNoHeaders()
        {
            GenerateTestFileHeadings(false);
            SortCSV norm = new SortCSV();
            norm.SortOrder.Add(new SortedField(1, SortType.SortString, true));
            norm.Process(INPUT_NAME, OUTPUT_NAME, false, CSVFormat.ENGLISH);

            TextReader tr = new StreamReader(OUTPUT_NAME);

            Assert.AreEqual("\"one\",1", tr.ReadLine());
            Assert.AreEqual("\"two\",2", tr.ReadLine());
            Assert.AreEqual("\"three\",3", tr.ReadLine());
            Assert.AreEqual("\"four\",4", tr.ReadLine());
            Assert.AreEqual("\"five\",5", tr.ReadLine());
            Assert.AreEqual("\"six\",6", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());


            tr.Close();

            File.Delete("test.csv");
            File.Delete("test2.csv");
        }


    }
}
