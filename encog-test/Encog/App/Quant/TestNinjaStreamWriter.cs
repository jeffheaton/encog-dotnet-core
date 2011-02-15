using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.App.Quant.Ninja;
using Encog.Util.CSV;
using System.IO;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestNinjaStreamWriter
    {
        [Test]
        public void TestWrite()
        {
            NinjaStreamWriter nsw = new NinjaStreamWriter();

            nsw.Open("test.csv", true, CSVFormat.ENGLISH);

            nsw.BeginBar(new DateTime(2010, 01, 01));
            nsw.StoreColumn("close", 10);
            nsw.EndBar();

            nsw.BeginBar(new DateTime(2010, 01, 02));
            nsw.StoreColumn("close", 11);
            nsw.EndBar();

            TextReader tr = new StreamReader("test.txt");

            Assert.AreEqual("date,time,\"close\"",tr.ReadLine());
            Assert.AreEqual("20100101,0,10",tr.ReadLine());
            Assert.AreEqual("20100102,0,11", tr.ReadLine());

            tr.Close();
        }
    }
}
