using System;
using System.IO;
using Encog.App.Quant.Ninja;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestNinjaStreamWriter
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        [TestMethod]
        public void TestWrite()
        {
            var nsw = new NinjaStreamWriter();

            nsw.Open(OutputName.ToString(), true, CSVFormat.ENGLISH);

            nsw.BeginBar(new DateTime(2010, 01, 01));
            nsw.StoreColumn("close", 10);
            nsw.EndBar();

            nsw.BeginBar(new DateTime(2010, 01, 02));
            nsw.StoreColumn("close", 11);
            nsw.EndBar();

            nsw.Close();

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("date,time,\"close\"", tr.ReadLine());
            Assert.AreEqual("20100101,0,10", tr.ReadLine());
            Assert.AreEqual("20100102,0,11", tr.ReadLine());

            tr.Close();
        }
    }
}