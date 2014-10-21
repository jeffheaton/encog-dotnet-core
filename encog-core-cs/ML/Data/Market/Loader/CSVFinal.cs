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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Encog.Util.CSV;

namespace Encog.ML.Data.Market.Loader
{
    /// <summary>
    /// Use this class to loads CSVs and places them in a MarketDataset.
    /// You must call GetFile to point to the CSV you want to use.
    /// </summary>
    public class CSVFinal : IMarketLoader
    {

        #region IMarketLoader Members


        string Precision { get; set; }
        public static string LoadedFile { get; set; }


        /// <summary>
        /// Reads the CSV and call loader.
        /// Used internally to load the csv and place data in the marketdataset.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="neededTypes">The needed types.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="File">The file.</param>
        /// <returns></returns>
        ICollection<LoadedMarketData> ReadAndCallLoader(
            TickerSymbol symbol, 
            IEnumerable<MarketDataType> neededTypes, 
            DateTime from, 
            DateTime to, 
            string File)
        {
                //We got a file, lets load it.

                ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                ReadCSV csv = new ReadCSV(File, true, CSVFormat.English);
                //In case we want to use a different date format...and have used the SetDateFormat method, our DateFormat must then not be null..
                //We will use the ?? operator to check for nullables.
                csv.DateFormat = DateFormat ?? "yyyy-MM-dd HH:mm:ss";
                csv.TimeFormat = "HH:mm:ss";

                DateTime ParsedDate = from;
                bool writeonce = true;

                while (csv.Next())
                {
                    DateTime date = csv.GetDate(0);
                    ParsedDate = date;

                    if (writeonce)
                    {
                        Console.WriteLine(@"First parsed date in csv:" + ParsedDate.ToShortDateString());
                        Console.WriteLine(@"Stopping at date:" + to.ToShortDateString());
                        Console.WriteLine(@"Current DateTime:" + ParsedDate.ToShortDateString() + @" Time:" +
                                          ParsedDate.ToShortTimeString() + @"  Asked Start date was " +
                                          from.ToShortDateString());
                        writeonce = false;
                    }
                    if (ParsedDate >= from && ParsedDate <= to)
                    {
                        DateTime datex = csv.GetDate(0);
                        double open = csv.GetDouble(1);
                        double close = csv.GetDouble(2);
                        double high = csv.GetDouble(3);
                        double low = csv.GetDouble(4);
                        double volume = csv.GetDouble(5);
                        double range = Math.Abs(open - close);
                        double HighLowRange = Math.Abs(high - low);
                        double DirectionalRange = close - open;
                        LoadedMarketData data = new LoadedMarketData(datex, symbol);
                        data.SetData(MarketDataType.Open, open);
                        data.SetData(MarketDataType.High, high);
                        data.SetData(MarketDataType.Low, low);
                        data.SetData(MarketDataType.Close, close);
                        data.SetData(MarketDataType.Volume, volume);
                        data.SetData(MarketDataType.RangeHighLow, Math.Round(HighLowRange, 6));
                        data.SetData(MarketDataType.RangeOpenClose, Math.Round(range, 6));
                        data.SetData(MarketDataType.RangeOpenCloseNonAbsolute, Math.Round(DirectionalRange, 6));
                        result.Add(data);


                    }

                }

                csv.Close();
                return result;
        }

        /// <summary>
        /// Gets or sets the date format for the whole csv file.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat { get; set; }
        /// <summary>
        /// Sets the date format for the csv file.
        /// </summary>
        /// <param name="stringFormat">The string format.</param>
        public void SetDateFormat(string stringFormat)
        {
            DateFormat = stringFormat;
            return;
        }

        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {

           
            return File.Exists(LoadedFile) ? (ReadAndCallLoader(ticker, dataNeeded, from, to, LoadedFile)) : null;
        }



        #endregion

        #region IMarketLoader Members




        #endregion

        #region IMarketLoader Members


        /// <summary>
        /// Gets the file we want to parse.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public string GetFile(string file)
        {
            if (File.Exists(file))
                LoadedFile = file;
            return LoadedFile;
        }

        #endregion

    }
}
