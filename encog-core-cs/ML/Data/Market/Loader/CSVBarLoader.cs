using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Util.CSV;

namespace Encog.ML.Data.Market.Loader
{
    class CSVBarLoader
    {        #region IMarketLoader Members


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
                    DateTime date = csv.GetDate("Date");
                    Console.WriteLine(@"Current DateTime:"+ParsedDate.ToShortDateString()+ @" Time:"+ParsedDate.ToShortTimeString() +@"  Start date was "+from.ToShortDateString());
                    Console.WriteLine(@"Stopping at date:" + to.ToShortDateString() );
                    ParsedDate = date;
                    double open = csv.GetDouble("Open");
                    double close = csv.GetDouble("High");
                    double high = csv.GetDouble("Low");
                    double low = csv.GetDouble("Close");
                    double volume = csv.GetDouble(4);
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
               
                Console.WriteLine(@"Something went wrong reading the csv:" + ex.Message);
                  return null;
            }
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
