using System;
using System.IO;
using Encog.App.Quant;
using Encog.App.Quant.Loader.Yahoo;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestYahooDownload
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        [TestMethod]
        public void TestYahooDownloadError()
        {
            try
            {
                var yahoo = new YahooDownload();
                yahoo.Precision = 2;
                // load a non-sense ticker, should throw error
                yahoo.LoadAllData("sdfhusdhfuish", OutputName.ToString(), CSVFormat.ENGLISH,
                                  new DateTime(2000, 01, 01),
                                  new DateTime(2000, 01, 10));

                // bad!
                Assert.IsTrue(false);
            }
            catch (QuantError)
            {
                // good!
            }
        }

        [TestMethod]
        public void TestYahooDownloadCSV()
        {
            var yahoo = new YahooDownload();
            yahoo.Precision = 2;
            yahoo.LoadAllData("yhoo", OutputName.ToString(), CSVFormat.ENGLISH,
                              new DateTime(2000, 01, 01),
                              new DateTime(2000, 01, 10));
            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual(
                "date,time,open price,high price,low price,close price,volume,adjusted price",
                tr.ReadLine());
            Assert.AreEqual(
                "20000110,0,432.5,451.25,420,436.06,61022400,109.01",
                tr.ReadLine());
            Assert.AreEqual("20000107,0,366.75,408,363,407.25,48999600,101.81",
                            tr.ReadLine());
            Assert.AreEqual("20000106,0,406.25,413,361,368.19,71301200,92.05",
                            tr.ReadLine());
            Assert.AreEqual(
                "20000105,0,430.5,431.13,402,410.5,83194800,102.62",
                tr.ReadLine());
            tr.Close();
        }
    }
}