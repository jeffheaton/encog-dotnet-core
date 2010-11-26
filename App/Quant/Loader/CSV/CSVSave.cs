using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Util.CSV;
using Encog.App.Quant.MarketDB;

namespace Encog.App.Quant.Loader.CSV
{
    public class CSVSave
    {
        private MarketDataStorage storage;
        private CSVFormat format; 
        private String dateFormat; 
        private String timeFormat; 
        private CSVDataItem[] columns;

        public CSVSave(MarketDataStorage marketData, CSVFormat format, String dateFormat, String timeFormat, CSVDataItem[] columns)
        {
            this.storage = marketData;
            this.format = format;
            this.dateFormat = dateFormat;
            this.timeFormat = timeFormat;
            this.columns  = columns;
        }

        public void Save(String ticker, String filename)
        {
            StreamWriter sw;
            sw = File.CreateText(filename);

            this.storage.Reset(ticker);
            object obj;

            while ((obj = storage.LoadNextItem(ticker)) != null)
            {
                if (obj is StoredMarketData)
                {
                    StringBuilder line = new StringBuilder();

                    StoredMarketData eod = (StoredMarketData)obj;

                    foreach (CSVDataItem item in columns)
                    {
                        if (line.Length > 0)
                        {
                            line.Append(",");
                        }

                        switch (item)
                        {
                            case CSVDataItem.Ticker:
                                line.Append(ticker);
                                break;
                            case CSVDataItem.Ignore:
                                break;
                            case CSVDataItem.Open:
                                line.Append(this.format.Format(eod.Open, 8));
                                break;
                            case CSVDataItem.Close:
                                line.Append(this.format.Format(eod.Close, 8));
                                break;
                            case CSVDataItem.High:
                                line.Append(this.format.Format(eod.High, 8));
                                break;
                            case CSVDataItem.Low:
                                line.Append(this.format.Format(eod.Low, 8));
                                break;
                            case CSVDataItem.Volume:
                                line.Append(eod.Volume);
                                break;
                            case CSVDataItem.DateAndTime:
                                line.Append(eod.Date.ToString(this.dateFormat));
                                break;
                            case CSVDataItem.DateOnly:
                                line.Append(eod.Date.ToString(this.dateFormat));
                                break;
                            case CSVDataItem.TimeOnly:
                                line.Append(eod.Time.ToString(this.timeFormat));
                                break;
                        }                       
                    }
                    
                    sw.WriteLine(line);
                }                
            }

            sw.Close();
            this.storage.Close();
        }
    }
}
