using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using Encog.App.Quant.MarketDB;

namespace Encog.App.Quant.Loader.CSV
{
    public class CSVLoad: BasicLoader
    {
        public static readonly CSVDataItem[] STANDARD_COLUMNS = { 
                CSVDataItem.Ticker, 
                CSVDataItem.DateOnly, 
                CSVDataItem.TimeOnly, 
                CSVDataItem.Open, 
                CSVDataItem.High, 
                CSVDataItem.Low, 
                CSVDataItem.Close, 
                CSVDataItem.Volume };

        private CSVFormat format;
        private CSVDataItem[] columns;
        private String dateFormat;
        private String timeFormat;

        public CSVLoad(MarketDataStorage marketData, CSVFormat format, String dateFormat, String timeFormat, CSVDataItem[] columns)
            : base( marketData)
        {
            this.format = format;
            this.columns = columns;
            this.dateFormat = dateFormat;
            this.timeFormat = timeFormat;
        }

        public void Load(String ticker, String filename, bool headers)
        {
            ReadCSV csv = new ReadCSV(filename, headers, format);
            csv.DateFormat = this.dateFormat;
            csv.TimeFormat = this.timeFormat;

            while (csv.Next())
            {
                StoredMarketData item = new StoredMarketData();
                for(int i=0;i<columns.Length;i++)
                {
                    switch(columns[i])
                    {
                        case CSVDataItem.Ignore:
                            // ignore
                            break;
                        case CSVDataItem.Open:
                            item.Open = (float)csv.GetDouble(i);
                            break;
                        case CSVDataItem.Close:
                            item.Close = (float)csv.GetDouble(i);
                            break;
                        case CSVDataItem.High:
                            item.High = (float)csv.GetDouble(i);
                            break;
                        case CSVDataItem.Low:
                            item.Low = (float)csv.GetDouble(i);
                            break;
                        case CSVDataItem.Volume:
                            item.Volume = (ulong)csv.GetLong(i);
                            break;
                        case CSVDataItem.DateAndTime:
                            DateTime t = csv.GetDate(i);
                            item.Date = t.Date;                          
                            break;
                        case CSVDataItem.DateOnly:
                            item.Date = csv.GetDate(i);
                            break;
                        case CSVDataItem.TimeOnly:
                            DateTime tm = csv.GetTime(i);
                            item.Time = tm;
                            break;
                    }
                }
                SelectFile(ticker, item.Date.Year);
                Loaded.Add(item,null);
            }
            WriteLoaded(ticker);
            csv.Close();
        }
    }
}
