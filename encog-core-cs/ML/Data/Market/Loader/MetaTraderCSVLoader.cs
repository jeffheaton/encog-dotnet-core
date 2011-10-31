using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
namespace Encog.ML.Data.Market.Loader
{
    /// <summary>
    /// Use this CSV Loader to load MetaTrader CSV files in a market data set, ready to use.
    /// </summary>
    public class CSVMetaTrader : IMarketLoader
    {

        #region IMarketLoader Members

        public List<MarketDataType> TypesNeeded { get; set; }
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
        ICollection<LoadedMarketData> ReadAndCallLoader(TickerSymbol symbol, IEnumerable<MarketDataType> neededTypes, DateTime from, DateTime to, string File)
        {
            try
            {
                //We got a file, lets load it.

                ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                ReadCSV csv = new ReadCSV(File, false, CSVFormat.English);
                //In case we want to use a different date format...and have used the SetDateFormat method, our DateFormat must then not be null..
                //We will use the ?? operator to check for nullables.
                csv.DateFormat = DateFormat ?? "yyyy.MM.dd";
                csv.TimeFormat = TimeFormat ?? "HH:mm";


                if (neededTypes == null)
                {
                    Console.WriteLine("Data Types needed is nul so using the neededTypes...You must have filled the list prior..");
                    neededTypes = TypesNeeded;
                }

                //Meta trader files have no headers...and are as below.
                //2000.09.27,00:00,0.60350,0.60380,0.60280,0.60330,95
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
                        DateTime time = csv.GetTime(1);

                        double open = csv.GetDouble(2);
                        double high = csv.GetDouble(3);
                        double low = csv.GetDouble(4);
                        double close = csv.GetDouble(5);
                        double volume = csv.GetDouble(6);

                        double range = Math.Abs(open - close);
                        double HighLowRange = Math.Abs(high - low);
                        double DirectionalRange = close - open;


                        DateTime dt = new DateTime(datex.Year, datex.Month, datex.Day, time.Hour, time.Minute, time.Second);

                        LoadedMarketData data = new LoadedMarketData(dt, symbol);
                        if (neededTypes.Contains(MarketDataType.Open))
                            data.SetData(MarketDataType.Open, open);
                        if (neededTypes.Contains(MarketDataType.High))
                            data.SetData(MarketDataType.High, high);
                        if (neededTypes.Contains(MarketDataType.Low))
                            data.SetData(MarketDataType.Low, low);
                        if (neededTypes.Contains(MarketDataType.Close))
                            data.SetData(MarketDataType.Close, close);
                        if (neededTypes.Contains(MarketDataType.Volume))
                            data.SetData(MarketDataType.Volume, volume);
                        if (neededTypes.Contains(MarketDataType.RangeHighLow))
                            data.SetData(MarketDataType.RangeHighLow, Math.Round(HighLowRange, 6));
                        if (neededTypes.Contains(MarketDataType.RangeOpenClose))
                            data.SetData(MarketDataType.RangeOpenClose, Math.Round(range, 6));
                        if (neededTypes.Contains(MarketDataType.RangeOpenCloseNonAbsolute))
                            data.SetData(MarketDataType.RangeOpenCloseNonAbsolute, Math.Round(DirectionalRange, 6));




                        result.Add(data);


                    }

                }


                Console.WriteLine("Finished parsing your csv for a count of :" + result.Count, " Closing the csv...");
                csv.Close();
                return result;
            }

            catch (Exception ex)
            {
                Console.WriteLine(@"Something went wrong reading the csv:" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the date format for the whole csv file.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat { get; set; }



        /// <summary>
        /// Gets or sets the date format for the whole csv file.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string TimeFormat { get; set; }

        /// <summary>
        /// Sets the date format for the csv file.
        /// </summary>
        /// <param name="stringFormat">The string format.</param>
        public void SetDateFormat(string stringFormat)
        {
            DateFormat = stringFormat;
            return;
        }

        /// <summary>
        /// Load the specified ticker symbol for the specified date into a marketdataset.
        /// you must specify a metatrader CSV file with the GetFile() before calling this method.
        /// You do not need to change date or time format as they are already set for meta trader files.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">Which data is actually needed.</param>
        /// <param name="from">Beginning date for load.</param>
        /// <param name="to">Ending date for load.</param>
        /// <returns>
        /// A collection of LoadedMarketData objects that was loaded.
        /// </returns>
        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {
            return File.Exists(LoadedFile) ? (ReadAndCallLoader(ticker, dataNeeded, from, to, LoadedFile)) : null;
        }



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