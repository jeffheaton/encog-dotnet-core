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
using Encog.ML.Data.Market;
using Encog.Util.CSV;
using System.IO;
using Encog.ML.Data.Market.Loader;

namespace ErrorTesterConsoleApplication
{
    public class MultiCsvLoader : IMarketLoader
    {

        #region IMarketLoader Members

        string Precision { get; set; }
        private string LoadedFile { get; set; }
        /// <summary>
        /// Set CSVFormat, default CSVFormat.DecimalPoint
        /// </summary>
        public CSVFormat LoadedFormat { get; set; }
        /// <summary>
        /// Use to indicate that Date and Time exist in different columns, default true
        /// </summary>
        bool DateTimeDualColumn { get; set; }
        /// <summary>
        /// Use to set format for Date or DateTime column, default MM/dd/yyyy
        /// </summary>
        string DateFormat { get; set; }
        /// <summary>
        /// Use to set format for Time column, default HHmm
        /// </summary>
        string TimeFormat { get; set; }

        /// <summary>
        /// Links a TickerSymbol to a CSV file
        /// </summary>
        private IDictionary<TickerSymbol, string> fileList = new Dictionary<TickerSymbol, string>();

        /// <summary>
        /// Construct a multi CSV file loader
        /// </summary>
        public MultiCsvLoader()
        {
            LoadedFormat = CSVFormat.DecimalPoint;
            DateTimeDualColumn = true;
            DateFormat = "MM/dd/yyyy";
            TimeFormat = "HHmm";

        }
        /// <summary>
        /// Construct a multi CSV file loader with option to set loaded format and indicate if CSV date is in two columns
        /// </summary>
        /// <param name="format">Set the CSV format</param>
        /// <param name="dualDateTimeColumn"> Indicate true for DateTime information in two separate columns</param>
        public MultiCsvLoader(CSVFormat format, bool dualDateTimeColumn)
        {
            LoadedFormat = format;
            DateTimeDualColumn = dualDateTimeColumn;
        }

        /// <summary>
        /// Reads and parses CSV data from file
        /// </summary>
        /// <param name="ticker">Ticker associated with CSV file</param>
        /// <param name="neededTypes">Columns to parse (headers)</param>
        /// <param name="from">DateTime from</param>
        /// <param name="to">DateTime to</param>
        /// <param name="File">Filepath to CSV</param>
        /// <returns>Marketdata</returns>
        public ICollection<LoadedMarketData> ReadAndCallLoader(TickerSymbol ticker, IList<MarketDataType> neededTypes, DateTime from, DateTime to, string File)
        {
            try
            {
                LoadedFile = File;
                Console.WriteLine("Loading instrument: " + ticker.Symbol + " from: " + File);
                //We got a file, lets load it.
                ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                ReadCSV csv = new ReadCSV(File, true, LoadedFormat);
                if (DateTimeDualColumn)
                {
                    csv.DateFormat = DateFormat;
                    csv.TimeFormat =  TimeFormat;
                }
                else
                {
                    csv.DateFormat = DateFormat;
                }

                //"Date","Time","Open","High","Low","Close","Volume"
                while (csv.Next())
                {
                    string datetime = "";
                    if (DateTimeDualColumn)
                    {
                        datetime = csv.GetDate("Date").ToShortDateString() + " " +
                                          csv.GetTime("Time").ToShortTimeString();
                    }
                    else
                    {
                        datetime = csv.GetDate("Date").ToShortDateString();
                    }
                    DateTime date = DateTime.Parse(datetime);
                    if (date > from && date < to)
                    {
                        // CSV columns
                        double open = csv.GetDouble("Open");
                        double high = csv.GetDouble("High");
                        double low = csv.GetDouble("Low");
                        double close = csv.GetDouble("Close");
                        double volume = csv.GetDouble("Volume");

                        LoadedMarketData data = new LoadedMarketData(date, ticker);
                        foreach (MarketDataType marketDataType in neededTypes)
                        {
                            switch (marketDataType.ToString())
                            {
                                case "Open":
                                    data.SetData(MarketDataType.Open, open);
                                    break;
                                case "High":
                                    data.SetData(MarketDataType.High, high);
                                    break;
                                case "Low":
                                    data.SetData(MarketDataType.Low, low);
                                    break;
                                case "Close":
                                    data.SetData(MarketDataType.Close, close);
                                    break;
                                case "Volume":
                                    data.SetData(MarketDataType.Volume, volume);
                                    break;
                                case "RangeHighLow":
                                    data.SetData(MarketDataType.RangeHighLow, Math.Round(Math.Abs(high - low), 6));
                                    break;
                                case "RangeOpenClose":
                                    data.SetData(MarketDataType.RangeOpenClose, Math.Round(Math.Abs(close - open), 6));
                                    break;
                                case "RangeOpenCloseNonAbsolute":
                                    data.SetData(MarketDataType.RangeOpenCloseNonAbsolute, Math.Round(close - open, 6));
                                    break;
                                case "Weighted":
                                    data.SetData(MarketDataType.Weighted, Math.Round((high + low + 2 * close) / 4, 6));
                                    break;
                            }
                        }
                        result.Add(data);
                    }
                }
                csv.Close();
                return result;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong reading the csv: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Set correct CSVFormat, e.g. "Decimal Point"
        /// </summary>
        /// <param name="csvformat">Enter format, e.g. "Decimal Point"</param>
        /// <returns></returns>
        public CSVFormat fromStringCSVFormattoCSVFormat(string csvformat)
        {
            switch (csvformat)
            {
                case "Decimal Point":
                    LoadedFormat = CSVFormat.DecimalPoint;
                    break;
                case "Decimal Comma":
                    LoadedFormat = CSVFormat.DecimalComma;
                    break;
                case "English Format":
                    LoadedFormat = CSVFormat.English;
                    break;
                case "EG Format":
                    LoadedFormat = CSVFormat.EgFormat;
                    break;
                default:
                    break;
            }

            return LoadedFormat;
        }

        #region IMarketLoader Members
        /// <derived/>
        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, 
            DateTime from, DateTime to)
        {
            try
            {
                string fileCSV = "";
                foreach (TickerSymbol tickerSymbol in fileList.Keys)
                {
                    if (tickerSymbol.Symbol.ToLower().Equals(ticker.Symbol.ToLower()))
                    {
                        if (fileList.TryGetValue(tickerSymbol, out fileCSV))
                        {
                            return File.Exists(fileCSV) ?
                                (ReadAndCallLoader(tickerSymbol, dataNeeded, from, to, fileCSV)) : null; //If file does not exist
                        }
                        return null; //Problem reading list
                    }
                }
                return null; //if ticker is not defined            
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine("Problem with loading data for instruments", fnfe);
                return null;
            }
        }

        //Should be removed/changed in interface
        public string GetFile(string file)
        {
            return LoadedFile;
        }

        #endregion

        /// <summary>
        /// Add filepaths with matching ticker symbols
        /// </summary>
        /// <param name="ticker">Set Ticker, e.g. EURUSD</param>
        /// <param name="fileName">Set filepath, e.g. .\\EURUSD.csv</param>
        /// <returns>True returned if file exists, if file does not exist FileNotFoundException thrown, 
        /// if TickerSymbol exist false is returned</returns>
        public bool SetFiles(string ticker, string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File submitted did not exist", fileName);
            }
            else if (!fileList.ContainsKey(new TickerSymbol(ticker)))
            {
                fileList.Add(new TickerSymbol(ticker), fileName);
                return true;
            }
            return false; //Ticker exists
        }
        #endregion
    }
}
