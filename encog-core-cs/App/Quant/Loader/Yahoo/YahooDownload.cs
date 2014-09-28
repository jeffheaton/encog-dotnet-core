//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
    ///     A loader, that will pull basic EOD data from YahooFinance.
    /// </summary>
    public class YahooDownload
    {
        /// <summary>
        ///     The DJIA index.
        /// </summary>
        public const String IndexDjia = "^dji";

        /// <summary>
        ///     The SP500 index.
        /// </summary>
        public const String IndexSp500 = "^gspc";

        /// <summary>
        ///     The NASDAQ index.
        /// </summary>
        public const String IndexNasdaq = "^ixic";

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public YahooDownload()
        {
            Precision = 10;
        }

        /// <summary>
        ///     The percision.
        /// </summary>
        public int Precision { get; set; }


        /// <summary>
        ///     This method builds a URL to load data from Yahoo Finance for a neural
        ///     network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private static Uri BuildURL(String ticker, DateTime from,
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

        /// <summary>
        ///     Load financial data.
        /// </summary>
        /// <param name="ticker">The ticker symbol.</param>
        /// <param name="output">The output file.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <param name="from">Starting date.</param>
        /// <param name="to">Ending date.</param>
        public void LoadAllData(String ticker, String output, CSVFormat outputFormat, DateTime from,
                                DateTime to)
        {
            try
            {
                Uri urlData = BuildURL(ticker, from, to);
                WebRequest httpData = WebRequest.Create(urlData);
                var responseData = (HttpWebResponse) httpData.GetResponse();

                if (responseData != null)
                {
                    Stream istreamData = responseData.GetResponseStream();
                    var csvData = new ReadCSV(istreamData, true, CSVFormat.English);

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
                        line.Append(outputFormat.Format(open, Precision));
                        line.Append(outputFormat.Separator);
                        line.Append(outputFormat.Format(high, Precision));
                        line.Append(outputFormat.Separator);
                        line.Append(outputFormat.Format(low, Precision));
                        line.Append(outputFormat.Separator);
                        line.Append(outputFormat.Format(close, Precision));
                        line.Append(outputFormat.Separator);
                        line.Append(volume);
                        line.Append(outputFormat.Separator);
                        line.Append(outputFormat.Format(adjustedClose, Precision));
                        tw.WriteLine(line.ToString());
                    }

                    tw.Close();
                }
            }
            catch (WebException ex)
            {
                throw new QuantError(ex);
            }
        }
    }
}
