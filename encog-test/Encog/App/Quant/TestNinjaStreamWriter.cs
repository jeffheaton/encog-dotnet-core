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

            Assert.AreEqual("date,time,open price,high price,low price,close price,volume,adjusted price",tr.ReadLine());
            Assert.AreEqual("20000110,0,432.50,451.25,420.00,436.06,61022400,109.01",tr.ReadLine());
            Assert.AreEqual("20000107,0,366.75,408.00,363.00,407.25,48999600,101.81", tr.ReadLine());

            tr.Close();
        }
    }
}
