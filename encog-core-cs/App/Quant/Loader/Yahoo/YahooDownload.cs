using System;
using System.IO;
using System.Net;
using System.Text;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.HTTP;
using Encog.Util.Time;

namespace Encog.App.Quant.Loader.Yahoo
{
    /// <summary>
    /// A loader, that will pull basic EOD data from YahooFinance.
    /// </summary>
    public class YahooDownload
    {
        public const String INDEX_DJIA = "^dji";
        public const String INDEX_SP500 = "^gspc";
        public const String INDEX_NASDAQ = "^ixic";

        public YahooDownload()
        {
            Percision = 10;
        }

        public int Percision { get; set; }


        /// <summary>
        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private Uri BuildURL(String ticker, DateTime from,
                             DateTime to)
        {
            // construct the URL
            var mstream = new MemoryStream();
            var form = new FormUtility(mstream, null);

            form.Add("s", ticker);
            form.Add("a", "" + (from.Month - 1));
            form.Add("b", "" + from.Day);
            form.Add("c", "" + from.Year);
            form.Add("d", "" + (to.Month - 1));
            form.Add("e", "" + to.Day);
            form.Add("f", "" + to.Year);
            form.Add("g", "d");
            form.Add("ignore", ".csv");
            mstream.Close();
            byte[] b = mstream.GetBuffer();

            String str = "http://ichart.finance.yahoo.com/table.csv?"
                         + StringUtil.FromBytes(b);
            return new Uri(str);
        }

        public void LoadAllData(String ticker, String output, CSVFormat outputFormat, DateTime from,
                                DateTime to)
        {
            Uri urlData = BuildURL(ticker, from, to);
            WebRequest httpData = WebRequest.Create(urlData);
            var responseData = (HttpWebResponse) httpData.GetResponse();

            Stream istreamData = responseData.GetResponseStream();
            var csvData = new ReadCSV(istreamData, true, CSVFormat.ENGLISH);

            TextWriter tw = new StreamWriter(output);
            tw.WriteLine("date,time,open price,high price,low price,close price,volume,adjusted price");

            while (csvData.Next())
            {
                DateTime date = csvData.GetDate("date");
                double adjustedClose = csvData.GetDouble("adj close");
                double open = csvData.GetDouble("open");
                double close = csvData.GetDouble("close");
                double high = csvData.GetDouble("high");
                double low = csvData.GetDouble("low");
                var volume = (long) csvData.GetDouble("volume");

                var line = new StringBuilder();
                line.Append(NumericDateUtil.DateTime2Long(date));
                line.Append(outputFormat.Separator);
                line.Append(NumericDateUtil.Time2Int(date));
                line.Append(outputFormat.Separator);
                line.Append(outputFormat.Format(open, Percision));
                line.Append(outputFormat.Separator);
                line.Append(outputFormat.Format(high, Percision));
                line.Append(outputFormat.Separator);
                line.Append(outputFormat.Format(low, Percision));
                line.Append(outputFormat.Separator);
                line.Append(outputFormat.Format(close, Percision));
                line.Append(outputFormat.Separator);
                line.Append(volume);
                line.Append(outputFormat.Separator);
                line.Append(outputFormat.Format(adjustedClose, Percision));
                tw.WriteLine(line.ToString());
            }

            tw.Close();
        }
    }
}