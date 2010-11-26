using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Encog;
using Encog.Bot;
using Encog.Util.Time;

namespace Encog.App.Quant.MarketDB
{
    public class MarketDataStorage
    {
        public const String EXTENSION_MARKET_DATA = ".dat";
        public const String EXTENSION_ADJUSTMENT_DATA = ".adj";
        private String pathBase;
        private String pathMarket;
        private Stream streamData;
        private BinaryReader binaryReader;
        private int currentYear;

        public MarketDataStorage(String pathBase)
        {
            this.pathBase = pathBase;
            CreateDirectory(pathBase);
            this.pathMarket = PathAppend(pathBase, "marketdata");
            CreateDirectory(pathBase);
        }

        public MarketDataStorage()
            :this(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal)+"\\EncogMarket")
        {            
        }

        public void Reset(String ticker)
        {
            currentYear = EarliestYear(ticker);
        }

        private String PathAppend(String b, String f)
        {
            StringBuilder result = new StringBuilder();
            result.Append(b);
            if (result[result.Length - 1] != '\\')
                result.Append('\\');
            result.Append(f);
            return result.ToString();
        }

        public String NormalizeTicker(String ticker)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < ticker.Length; i++)
            {
                if (Char.IsLetterOrDigit(ticker[i]))
                {
                    result.Append( Char.ToLower(ticker[i]) );
                }
                else
                {
                    result.Append( "_" );
                }
            }

            return result.ToString();
        }

        public String GetSecurityFile(String ticker, int year)
        {
            String ticker2 = NormalizeTicker(ticker);

            char letter = ticker2.ToLower()[0];
            String result = PathAppend(this.pathMarket, ""+letter);
            result = PathAppend(result, ticker2);
            CreateDirectory(result);
            result = PathAppend(result, ticker2) + "_" + year + MarketDataStorage.EXTENSION_MARKET_DATA;
            return result;
        }

        private void CreateDirectory(String path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private bool SelectFile(String ticker)
        {
            int nowYear = DateTime.Now.Year;

            while (this.streamData == null || (streamData.Position >= streamData.Length))
            {
                if (this.streamData != null)
                {
                    this.streamData.Close();
                    this.streamData = null;
                    this.currentYear++;
                }

                String filename = GetSecurityFile(ticker, this.currentYear);

                try
                {
                    this.streamData = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                    this.binaryReader = new BinaryReader(this.streamData);
                }
                catch (FileNotFoundException ex)
                {
                    if (currentYear > DateTime.Now.Year)
                        break;
                    this.currentYear++;
                }
            }

            if (this.streamData == null)
                return false;
            else
                return streamData.Position < streamData.Length;
        }

        public StoredMarketData LoadNextItem(String ticker)
        {
            StoredMarketData result = null;

            if (SelectFile(ticker))
            {
                result = ReadObject();
            }

            return result;
        }

        public void Dump(String ticker, String filename)
        {
            StreamWriter sw;
            sw = File.CreateText(filename);

            this.currentYear = EarliestYear(ticker);

            object obj;
            sw.WriteLine("Adjustments:");
            PriceAdjustments adjust = new PriceAdjustments(this, ticker);
            adjust.Load();
            foreach (StoredAdjustmentData adj in adjust.Data.Keys)
            {
                sw.WriteLine(adj);
            }

            sw.WriteLine("Market Data:");
            while ((obj = LoadNextItem(ticker)) != null)
            {
                StoredMarketData eod = (StoredMarketData)obj;
                double a = adjust.CalculateAdjustment(eod.Time);
                sw.WriteLine(obj.ToString() + ",adjust=" + a + ",adj close=" + (eod.Close * a));                
            }
            sw.Close();
            if (streamData != null)
            {
                streamData.Close();
                this.streamData = null;
            }
        }

        private bool ShouldReportMinute(uint currentTime, uint lastTime, int period)
        {
            if (currentTime == 0 || lastTime == 0)
                return false;

            int now = NumericDateUtil.GetMinutePeriod(currentTime, period);
            int last = NumericDateUtil.GetMinutePeriod(lastTime, period);
            return now != last;
        }

        private bool ShouldReport(StoredMarketData data, ulong lastDate, uint lastTime, BarPeriod period)
        {
            switch (period)
            {
                case BarPeriod.MINUTE_1:
                    return true;
                case BarPeriod.MINUTES_5:
                    if( ShouldReportMinute(data.EncodedTime,lastTime,5) )
                        return true;
                    break;
                case BarPeriod.MINUTES_10:
                    if (ShouldReportMinute(data.EncodedTime, lastTime, 10))
                        return true;
                    break;
                case BarPeriod.MINUTES_15:
                    if (ShouldReportMinute(data.EncodedTime, lastTime, 15))
                        return true;
                    break;
                case BarPeriod.MINUTES_30:
                    if (ShouldReportMinute(data.EncodedTime, lastTime, 30))
                        return true;
                    break;
                case BarPeriod.MINTUES_60:
                    if (ShouldReportMinute(data.EncodedTime, lastTime, 60))
                        return true;
                    break;
                case BarPeriod.EOD:
                    if ( lastDate>0 && (lastDate!=data.EncodedDate) )
                        return true;
                    break;
                case BarPeriod.WEEKLY:
                    if (lastDate > 0)
                    {
                        int prev = NumericDateUtil.GetDayOfWeek(lastDate);
                        int now = NumericDateUtil.GetDayOfWeek(data.EncodedDate);
                        if (prev != now && prev > now)
                            return true;
                    }
                    break;
                case BarPeriod.MONTHLY:
                    if (lastDate > 0 && ( NumericDateUtil.GetMonth(lastDate) != NumericDateUtil.GetMonth(data.EncodedDate) ))
                        return true;
                    break;
                case BarPeriod.YEARLY:
                    if (lastDate > 0 && ( NumericDateUtil.GetYear(lastDate) != NumericDateUtil.GetYear(data.EncodedDate) ))
                        return true;
                    break;
            }
            return false;
        }
       
        public IList<StoredMarketData> LoadRange(
            String ticker,
            DateTime fromDate,
            DateTime toDate,
            BarPeriod period)
        {
            IList<StoredMarketData> result = new List<StoredMarketData>();
            StoredMarketData reportedItem = null;
            ulong lastDate = 0;
            uint lastTime = 0;
            this.currentYear = fromDate.Year;

            ulong from = NumericDateUtil.DateTime2Long(fromDate);
            ulong to = NumericDateUtil.DateTime2Long(toDate);

            StoredMarketData data;

            while ((data = LoadNextItem(ticker)) != null)
            {
                if (data.EncodedDate >= from && data.EncodedDate <= to)
                {
                    if (reportedItem!=null && ShouldReport(data, lastDate, lastTime, period) )
                    {
                        reportedItem.Average();
                        result.Add(reportedItem);
                        reportedItem = null;
                    }
                    else
                    {
                        if (reportedItem == null)
                        {
                            reportedItem = data;
                        }
                        else
                        {
                            reportedItem.Add(data);
                        }
                    }
                    lastDate = data.EncodedDate;
                    lastTime = data.EncodedTime;
                }

                if (data.EncodedDate > to)
                    break;
                
            }

            if (streamData != null)
            {
                streamData.Close();
                this.streamData = null;
            }

            return result;
        }

        private StoredMarketData ReadObject()
        {
            StoredMarketData data = new StoredMarketData();
            data.EncodedDate = (ulong)this.binaryReader.ReadInt64();
            data.EncodedTime = (uint)this.binaryReader.ReadInt32();
            data.Volume = (ulong)this.binaryReader.ReadInt64();
            data.Open = this.binaryReader.ReadDouble();
            data.Close = this.binaryReader.ReadDouble();
            data.High = this.binaryReader.ReadDouble();
            data.Low = this.binaryReader.ReadDouble();
            data.Adjust(1.0);
            return data;
        }

        public void Close()
        {            
            if (streamData != null)
            {
                streamData.Close();
                this.streamData = null;
            }
        }

        public String ObtainBaseDirectory(String ticker)
        {
            String ticker2 = NormalizeTicker(ticker);

            char letter = ticker2.ToLower()[0];
            String result = PathAppend(this.pathMarket, "" + letter);
            result = PathAppend(result, ticker2);
            CreateDirectory(result);

            return result;
        }

        public int EarliestYear(String ticker)
        {
            int result = DateTime.Now.Year;
            DirectoryInfo di = new DirectoryInfo(ObtainBaseDirectory(ticker));

            FileInfo[] rgFiles = di.GetFiles();
            foreach (FileInfo fi in rgFiles)
            {
                int year;
                String yr = BotUtil.Extract(fi.Name, "_", ".",0);
                if (yr != null && int.TryParse(yr, out year) )
                {
                    result = Math.Min(result, year);
                }
            }

            return result;
        }

        public string GetAdjustmentFile(string ticker)
        {
            String ticker2 = NormalizeTicker(ticker);

            char letter = ticker2.ToLower()[0];
            String result = PathAppend(this.pathMarket, "" + letter);
            result = PathAppend(result, ticker2);
            CreateDirectory(result);
            result = PathAppend(result, ticker2) + MarketDataStorage.EXTENSION_ADJUSTMENT_DATA;
            return result;

        }
    }
}
