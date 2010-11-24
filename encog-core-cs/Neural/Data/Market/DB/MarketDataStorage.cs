using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Encog;
using Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance;
using Encog.Neural.Data.Market.DB;
using Encog.Bot;

namespace Encog.Neural.NeuralData.Market.DB
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

        public object LoadNextItem(String ticker)
        {
            object result = null;

            if (SelectFile(ticker))
            {
                result = ReadObject();

                if (result is StoredAdjustmentData)
                {
                    StoredAdjustmentData adj = (StoredAdjustmentData)result;                 
                }
                else if (result is StoredMarketData)
                {
                    StoredMarketData eod = (StoredMarketData)result;
                }
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

        public IList<StoredMarketData> LoadEODRange(String ticker, DateTime fromDate, DateTime toDate)
        {
            IList<StoredMarketData> result = new List<StoredMarketData>();

            this.currentYear = toDate.Year;

            ulong from = DateUtil.DateTime2Long(fromDate);
            ulong to = DateUtil.DateTime2Long(toDate);

            object obj;

            while ((obj = LoadNextItem(ticker)) != null)
            {
                if (obj is StoredMarketData)
                {
                    StoredMarketData eod = (StoredMarketData)obj;
                    if (eod.EncodedDate >= from && eod.EncodedDate <= to)
                        result.Insert(0, eod);

                    if (eod.EncodedDate < from)
                        break;
                }

            }

            if (streamData != null)
            {
                streamData.Close();
                this.streamData = null;
            }

            return result;

        }

        public IList<object> LoadRange(
            String ticker,
            DateTime fromDate,
            DateTime toDate)
        {
            return LoadRange(ticker, fromDate, toDate, true, true, true);
        }

        public IList<object> LoadRange(
            String ticker,
            DateTime fromDate,
            DateTime toDate,
            bool shouldAdjust,
            bool wantAdjustments,
            bool wantData)
        {
            IList<object> result = new List<object>();

            this.currentYear = fromDate.Year;

            ulong from = DateUtil.DateTime2Long(fromDate);
            ulong to = DateUtil.DateTime2Long(toDate);

            object obj;

            while ((obj = LoadNextItem(ticker)) != null)
            {
                if (obj is StoredAdjustmentData)
                {
                    StoredAdjustmentData adj = (StoredAdjustmentData)obj;
                    if (wantAdjustments && adj.EncodedDate >= from && adj.EncodedDate <= to)
                        result.Insert(0, adj);
                }
                else if (obj is StoredMarketData)
                {
                    StoredMarketData eod = (StoredMarketData)obj;
                    if (wantData && eod.EncodedDate >= from && eod.EncodedDate <= to)
                        result.Insert(0, eod);

                    if (eod.EncodedDate < from)
                        break;
                }
            }

            if (streamData != null)
            {
                streamData.Close();
                this.streamData = null;
            }

            return result;
        }

        private object ReadObject()
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
