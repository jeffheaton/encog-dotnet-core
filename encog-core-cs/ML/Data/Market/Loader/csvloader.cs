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
    /// <summary>
    /// CSVLoader class.
    /// This class is used to load CSV's into a 
    /// </summary>
    public class CSVLoader :IMarketLoader
    {

                #region IMarketLoader Members  
      
      
        string Precision { get; set; }
        string LoadedFile { get;set;}
        public List<MarketDataType> TypesLoaded = new List<MarketDataType>();
        CSVFormat LoadedFormat { get; set; }
        string DateTimeFormat { get; set; }

        public ICollection<LoadedMarketData> ReadAndCallLoader(TickerSymbol symbol, IList<MarketDataType> neededTypes, DateTime from, DateTime to,string File)
        {
            try
            {


                        //We got a file, lets load it.

                    

                        ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                        ReadCSV csv = new ReadCSV(File, true,LoadedFormat);


                        csv.DateFormat = DateTimeFormat.Normalize();
                        //  Time,Open,High,Low,Close,Volume
                        while (csv.Next())
                        {
                            DateTime date = csv.GetDate("Time");
                            double open = csv.GetDouble("Open");
                            double close = csv.GetDouble("High");
                            double high = csv.GetDouble("Low");
                            double low = csv.GetDouble("Close");
                            double volume = csv.GetDouble("Volume");
                            LoadedMarketData data = new LoadedMarketData(date, symbol);
                            data.SetData(MarketDataType.Open, open);
                            data.SetData(MarketDataType.High, high);
                            data.SetData(MarketDataType.Low, low);
                            data.SetData(MarketDataType.Close, close);
                            data.SetData(MarketDataType.Volume, volume);
                            result.Add(data);
                        }

                        csv.Close();
                        return result;                 
                }
            
            catch (Exception ex)
            {
                
              Console.WriteLine("Something went wrong reading the csv");
              Console.WriteLine("Something went wrong reading the csv:"+ex.Message);
            }

            Console.WriteLine("Something went wrong reading the csv");
            return null;
        }

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

        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {
              ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
            
            
                CSVFormLoader formLoader = new CSVFormLoader();
                    if (File.Exists(formLoader.Chosenfile))
                    {
                        LoadedFormat = formLoader.format;

                        //Lets add all the marketdatatypes we selected in the form.
                        foreach (MarketDataType item in formLoader.TypesLoaded)
                        {
                            TypesLoaded.Add(item);

                        }
                        DateTimeFormat = formLoader.DateTimeFormatTextBox.Text;
                        result = ReadAndCallLoader(ticker, dataNeeded, from, to,formLoader.Chosenfile);
                        return result;
                    }
                

                return null;
    }
        

        
        #endregion

        #region IMarketLoader Members


        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
   
