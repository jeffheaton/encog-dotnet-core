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
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using System.Windows.Forms;

namespace Encog.ML.Data.Market.Loader
{
    public class CSVFileLoader : IMarketLoader
    {

        #region IMarketLoader Members


        string Precision { get; set; }
        public static string LoadedFile { get; set; }


        public ICollection<LoadedMarketData> ReadAndCallLoader(TickerSymbol symbol, IList<MarketDataType> neededTypes, DateTime from, DateTime to, string File)
        {
            try
            {
                //We got a file, lets load it.
                ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                ReadCSV csv = new ReadCSV(File, true, CSVFormat.English);
                csv.DateFormat = "yyyy.MM.dd HH:mm:ss";

                DateTime ParsedDate = from;


                //  Time,Open,High,Low,Close,Volume
                while (csv.Next() && ParsedDate >= from && ParsedDate <= to  )
                {
                    DateTime date = csv.GetDate("Time");
                    double Bid= csv.GetDouble("Bid");
                    double Ask = csv.GetDouble("Ask");
                    double AskVolume = csv.GetDouble("AskVolume");
                    double BidVolume= csv.GetDouble("BidVolume");
                    double _trade = ( Bid + Ask ) /2;
                    double _tradeSize = (AskVolume + BidVolume) / 2;
                    LoadedMarketData data = new LoadedMarketData(date, symbol);
                    data.SetData(MarketDataType.Trade, _trade);
                    data.SetData(MarketDataType.Volume, _tradeSize);
                    result.Add(data);

                    Console.WriteLine("Current DateTime:"+ParsedDate.ToShortDateString()+ " Time:"+ParsedDate.ToShortTimeString() +"  Start date was "+from.ToShortDateString());
                    Console.WriteLine("Stopping at date:" + to.ToShortDateString() );
                    ParsedDate = date;
                    //double open = csv.GetDouble("Open");
                    //double close = csv.GetDouble("High");
                    //double high = csv.GetDouble("Low");
                    //double low = csv.GetDouble("Close");
                    //double volume = csv.GetDouble("Volume");
                    //LoadedMarketData data = new LoadedMarketData(date, symbol);
                    //data.SetData(MarketDataType.Open, open);
                    //data.SetData(MarketDataType.High, high);
                    //data.SetData(MarketDataType.Low, low);
                    //data.SetData(MarketDataType.Close, close);
                    //data.SetData(MarketDataType.Volume, volume);
                    result.Add(data);
                }

                csv.Close();
                return result;
            }

            catch (Exception ex)
            {

                Console.WriteLine("Something went wrong reading the csv");
                Console.WriteLine("Something went wrong reading the csv:" + ex.Message);
            }

            Console.WriteLine("Something went wrong reading the csv");
            return null;
        }



        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {
            ICollection<LoadedMarketData> result = new List<LoadedMarketData>();



            
            if (File.Exists(LoadedFile))
            {
               
              
                result = ReadAndCallLoader(ticker, dataNeeded, from, to, LoadedFile);
                return result;
            }


            return null;
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

