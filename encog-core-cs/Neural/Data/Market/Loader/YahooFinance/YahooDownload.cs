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
using Encog.Neural.Data.Market.Loader;
using Encog.Neural.Data.Market.DB;

namespace Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance
{
    public class YahooDownload: BasicLoader
    {
        public static readonly DateTime EARLIEST_DATE = new DateTime(1950, 1, 1);

        public const String INDEX_DJIA = "^dji";
        public const String INDEX_SP500 = "^gspc";
        public const String INDEX_NASDAQ = "^ixic";

        private bool noMoreDiv;
        private PriceAdjustments adjustments;

        public YahooDownload(MarketDataStorage marketData): base (marketData)
        {            
        }


        /// <summary>
        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private Uri BuildURL(String ticker, bool div, DateTime from,
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
            this.adjustments = new PriceAdjustments(this.Storage,ticker);
            int year = DateTime.Now.Year;
            DateTime from = YahooDownload.EARLIEST_DATE;
            DateTime to = new DateTime(year, 12, 31);
            LoadData(ticker, from, to);
            this.adjustments.Save();
        }

        private void LoadData(String ticker, DateTime from, DateTime to)
        {
            ulong lastDate = 0;

            this.noMoreDiv = false;
           
            // build web info for data
            Uri urlData = BuildURL(ticker, false, from, to);
            WebRequest httpData = HttpWebRequest.Create(urlData);
            HttpWebResponse responseData = (HttpWebResponse)httpData.GetResponse();

            Stream istreamData = responseData.GetResponseStream();
            ReadCSV csvData = new ReadCSV(istreamData, true, CSVFormat.ENGLISH);

            // build web info for div
            Uri urlDiv = BuildURL(ticker, true, from, to);
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
                        this.adjustments.Add(nextDiv);
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
                    this.adjustments.Add(adj);
                }

                Loaded.Add(data,null);
                //WriteObject(data);

                lastRatio = ratio;
                lastDate = data.EncodedDate;
            }

            WriteLoaded(ticker);
            csvData.Close();
            istreamData.Close();

            // close div
            csvDiv.Close();
            istreamDiv.Close();
        }

    }
}
