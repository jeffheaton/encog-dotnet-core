using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.App.Quant.Loader.YahooFinance;
using Encog.Util.CSV;
using System.IO;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestYahooDownload
    {
        [Test]
        public void TestYahooDownloadCSV()
        {
            YahooDownload yahoo = new YahooDownload();
            yahoo.Percision = 2;
            yahoo.LoadAllData("yhoo", "test.txt", CSVFormat.ENGLISH, new DateTime(2000, 01, 01), new DateTime(2000, 01, 10));
            TextReader tr = new StreamReader("test.txt");


            Assert.AreEqual("date,time,open price,high price,low price,close price,volume,adjusted price",tr.ReadLine());
            Assert.AreEqual("20000110,0,432.50,451.25,420.00,436.06,61022400,109.01",tr.ReadLine());
            Assert.AreEqual("20000107,0,366.75,408.00,363.00,407.25,48999600,101.81",tr.ReadLine());
            Assert.AreEqual("20000106,0,406.25,413.00,361.00,368.19,71301200,92.05",tr.ReadLine());
            Assert.AreEqual("20000105,0,430.50,431.13,402.00,410.50,83194800,102.62", tr.ReadLine());
            tr.Close();
        }
    }
}
