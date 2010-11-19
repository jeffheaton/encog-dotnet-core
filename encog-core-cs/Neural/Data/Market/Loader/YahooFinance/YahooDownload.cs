using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using Encog.Util.CSV;
using Encog.Util.HTTP;
using Encog.Util;

namespace Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance
{
    public class YahooDownload
    {
        public static readonly DateTime EARLIEST_DATE = new DateTime(1950, 1, 1);

        public const String INDEX_DJIA = "^dji";
        public const String INDEX_SP500 = "^gspc";
        public const String INDEX_NASDAQ = "^ixic";

        private bool noMoreDiv;
        private BinaryWriter streamData;
        private int lastYearWritten;
        private MarketDataStoreage marketData;

        public YahooDownload(MarketDataStoreage marketData)
        {
            this.marketData = marketData;
        }

        private void SelectFile(String ticker, int year)
        {
            if (streamData == null || lastYearWritten != year)
            {
                if (streamData != null)
                    streamData.Close();

                String filename = this.marketData.GetSecurityFile(ticker, year);
                this.streamData = new BinaryWriter(new FileStream(this.marketData.GetSecurityFile(ticker, year), FileMode.Create, FileAccess.Write, FileShare.None));
                this.lastYearWritten = year;
            }
        }

        /// <summary>
        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private Uri buildURL(String ticker, bool div, DateTime from,
                 DateTime to)
        {
            // construct the URL
            MemoryStream mstream = new MemoryStream();
            FormUtility form = new FormUtility(mstream, null);

            form.Add("s", ticker);
            form.Add("a", "" + (from.Month - 1));
            form.Add("b", "" + from.Day);
            form.Add("c", "" + from.Year);
            form.Add("d", "" + (to.Month - 1));
            form.Add("e", "" + to.Day);
            form.Add("f", "" + to.Year);
            form.Add("g", div ? "v" : "d");
            form.Add("ignore", ".csv");
            mstream.Close();
            byte[] b = mstream.GetBuffer();

            String str = "http://ichart.finance.yahoo.com/table.csv?"
                   + StringUtil.FromBytes(b);
            return new Uri(str);
        }

        private StoredAdjustmentData ReadNextDiv(ReadCSV csvDiv)
        {
            StoredAdjustmentData result = new StoredAdjustmentData();

            if (csvDiv.Next())
            {
                DateTime dateDiv = csvDiv.GetDate("date");
                result.Date = dateDiv;
                result.Div = csvDiv.GetDouble("dividends");
                return result;
            }
            else
            {
                this.noMoreDiv = false;
            }

            return result;
        }

        public void LoadAllData(String ticker)
        {
            int year = DateTime.Now.Year;
            DateTime from = YahooDownload.EARLIEST_DATE;
            DateTime to = new DateTime(year, 12, 31);
            LoadData(ticker, from, to);
            if (streamData != null)
                streamData.Close(); 
        }

        private void LoadData(String ticker, DateTime from, DateTime to)
        {
            ulong lastDate = 0;

            this.noMoreDiv = false;
           
            // build web info for data
            Uri urlData = buildURL(ticker, false, from, to);
            WebRequest httpData = HttpWebRequest.Create(urlData);
            HttpWebResponse responseData = (HttpWebResponse)httpData.GetResponse();

            Stream istreamData = responseData.GetResponseStream();
            ReadCSV csvData = new ReadCSV(istreamData, true, CSVFormat.ENGLISH);

            // build web info for div
            Uri urlDiv = buildURL(ticker, true, from, to);
            WebRequest httpDiv = HttpWebRequest.Create(urlDiv);
            HttpWebResponse responseDiv = (HttpWebResponse)httpDiv.GetResponse();

            Stream istreamDiv = responseDiv.GetResponseStream();
            ReadCSV csvDiv = new ReadCSV(istreamDiv, true, CSVFormat.ENGLISH);

            double lastRatio = 1;

            StoredAdjustmentData nextDiv = ReadNextDiv(csvDiv);

            while (csvData.Next())
            {
                bool foundDiv = false;
                StoredMarketData data = new StoredMarketData();

                DateTime date = csvData.GetDate("date");
                double adjustedClose = csvData.GetDouble("adj close");
                data.Open = csvData.GetDouble("open");
                data.Close = csvData.GetDouble("close");
                data.High = csvData.GetDouble("high");
                data.Low = csvData.GetDouble("low");
                data.Volume = (ulong)csvData.GetDouble("volume");
                data.Date = date;

                this.SelectFile(ticker, date.Year);

                if (!noMoreDiv)
                {
                    if (nextDiv.Date > data.Date)
                    {
                        nextDiv.CalculateAdjustment(data.Close);
                        WriteObject(nextDiv);
                        nextDiv = ReadNextDiv(csvDiv);
                        foundDiv = true;
                    }
                }

                double ratio = adjustedClose / data.Close;
                if (Math.Abs(ratio - lastRatio) > 0.001 && !foundDiv)
                {
                    double split = lastRatio / ratio;
                    SplitRatio s = SplitRateCollection.Instance.FindClosest(split);
                    StoredAdjustmentData adj = new StoredAdjustmentData();
                    adj.EncodedDate = lastDate;
                    adj.Numerator = s.numerator;
                    adj.Denominator = s.denominator;
                    adj.CalculateAdjustment(data.Close);
                    WriteObject(adj);
                    Console.WriteLine("" + lastDate + " " + ratio + " " + lastRatio + " " + (lastRatio / ratio) + " " + s.ToString());
                }

                WriteObject(data);

                lastRatio = ratio;
                lastDate = data.EncodedDate;
            }

            csvData.Close();
            istreamData.Close();

            // close div
            csvDiv.Close();
            istreamDiv.Close();
        }

        private void WriteObject(object o)
        {
            if (o is StoredMarketData)
            {
                StoredMarketData data = (StoredMarketData)o;
                this.streamData.Write((byte)0);
                this.streamData.Write(data.EncodedDate);
                this.streamData.Write(data.Volume);
                this.streamData.Write(data.Open);
                this.streamData.Write(data.Close);
                this.streamData.Write(data.High);
                this.streamData.Write(data.Low);
            }
            else if (o is StoredAdjustmentData)
            {
                StoredAdjustmentData adj = (StoredAdjustmentData)o;
                this.streamData.Write((byte)1);
                this.streamData.Write(adj.EncodedDate);
                this.streamData.Write(adj.Adjustment);
                this.streamData.Write(adj.Div);
                this.streamData.Write(adj.Numerator);
                this.streamData.Write(adj.Denominator);

            }
        }

    }
}
