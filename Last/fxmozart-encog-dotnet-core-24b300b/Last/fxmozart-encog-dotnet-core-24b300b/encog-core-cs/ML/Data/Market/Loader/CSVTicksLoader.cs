using System;
using System.Collections.Generic;
using System.IO;
using Encog.Util.CSV;

namespace Encog.ML.Data.Market.Loader
{
    /// <summary>
    /// Load data from a CSV file.
    /// 
    /// From code provided by: fxmozart
    /// http://www.heatonresearch.com/node/2102
    /// </summary>
    public class CSVTicksLoader : IMarketLoader
    {
        /// <summary>
        /// The data loaded.
        /// </summary>
        public ICollection<LoadedMarketData> DataLoaded { get; set; }

        /// <summary>
        /// The file loaded from
        /// </summary> 
        public string TheFile { get; set; }

        /// <summary>
        /// The prevision.
        /// </summary>
        public string Precision { get; set; }

        #region IMarketLoader Members

        /// <summary>
        /// Load financial data from a CSV file.
        /// </summary>
        /// <param name="ticker">The ticker being loaded, ignored for a CSV load.</param>
        /// <param name="dataNeeded">The data needed.</param>
        /// <param name="from">The starting date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns></returns>
        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from,
                                                  DateTime to)
        {
            try
            {
                if (File.Exists(TheFile))
                {
                    //We got a file, lets load it.
                    TheFile = TheFile;
                    ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                    var csv = new ReadCSV(TheFile, true, CSVFormat.English);

                    //  Time,Open,High,Low,Close,Volume
                    while (csv.Next())
                    {
                        DateTime date = csv.GetDate("Time");
                        double open = csv.GetDouble("Open");
                        double close = csv.GetDouble("High");
                        double high = csv.GetDouble("Low");
                        double low = csv.GetDouble("Close");
                        double volume = csv.GetDouble("Volume");
                        var data = new LoadedMarketData(date, ticker);
                        data.SetData(MarketDataType.Open, open);
                        data.SetData(MarketDataType.Volume, close);
                        data.SetData(MarketDataType.High, high);
                        data.SetData(MarketDataType.Low, low);
                        data.SetData(MarketDataType.Volume, volume);
                        result.Add(data);
                    }

                    csv.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new LoaderError(ex);
            }

            throw new LoaderError(@"Something went wrong reading the csv");
        }

        #endregion
    }
}