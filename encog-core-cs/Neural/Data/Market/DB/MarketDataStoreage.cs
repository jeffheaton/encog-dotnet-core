using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Encog;
using Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance;

namespace Encog.Neural.NeuralData.Market.DB
{
    public class MarketDataStoreage
    {
        public const String EXTENSION_MARKET_DATA = ".dat";
        private String pathBase;
        private String pathMarket;
        private Stream streamData;
        private BinaryReader binaryReader;
        private int currentYear;
        private double adjust = 1;

        public MarketDataStoreage(String pathBase)
        {
            this.pathBase = pathBase;
            CreateDirectory(pathBase);
            this.pathMarket = PathAppend(pathBase, "marketdata");
            CreateDirectory(pathBase);
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

        public String GetSecurityFile(String ticker, int year)
        {
            String ticker2 = "";
            for (int i = 0; i < ticker.Length; i++)
            {
                if (Char.IsLetterOrDigit(ticker[i]))
                {
                    ticker2 += Char.ToLower(ticker[i]);
                }
                else
                {
                    ticker2 += "_";
                }
            }

            char letter = ticker2.ToLower()[0];
            String result = PathAppend(this.pathMarket, ""+letter);
            result = PathAppend(result, ticker2);
            CreateDirectory(result);
            result = PathAppend(result, ticker2) + "_" + year + MarketDataStoreage.EXTENSION_MARKET_DATA;
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
                    this.currentYear--;
                }

                String filename = GetSecurityFile(ticker, this.currentYear);

                try
                {
                    this.streamData = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                    this.binaryReader = new BinaryReader(this.streamData);
                }
                catch (FileNotFoundException ex)
                {
                    if (currentYear < YahooDownload.EARLIEST_DATE.Year)
                        break;
                    this.currentYear--;
                }
            }

            if (this.streamData == null)
                return false;
            else
                return streamData.Position < streamData.Length;
        }

        private object LoadNextItem(String ticker)
        {
            object result = null;

            if (SelectFile(ticker))
            {
                result = ReadObject();

                if (result is StoredAdjustmentData)
                {
                    StoredAdjustmentData adj = (StoredAdjustmentData)result;
                    adjust *= adj.Adjustment;
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

            this.adjust = 1;
            this.currentYear = DateTime.Now.Year;

            object obj;

            while ((obj = LoadNextItem(ticker)) != null)
            {
                if (obj is StoredAdjustmentData)
                {
                    StoredAdjustmentData adj = (StoredAdjustmentData)obj;
                    sw.WriteLine(obj.ToString() + "newadj=" + adjust);
                }
                else if (obj is StoredMarketData)
                {
                    StoredMarketData eod = (StoredMarketData)obj;
                    sw.WriteLine(obj.ToString() + ",adjust=" + adjust + ",adj close=" + (eod.Close * adjust));
                }
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

            this.adjust = 1;
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

            this.adjust = 1;
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
            byte b = this.binaryReader.ReadByte();

            switch (b)
            {
                case 0:
                    StoredMarketData data = new StoredMarketData();
                    data.EncodedDate = (ulong)this.binaryReader.ReadInt64();
                    data.Volume = (ulong)this.binaryReader.ReadInt64();
                    data.Open = this.binaryReader.ReadDouble();
                    data.Close = this.binaryReader.ReadDouble();
                    data.High = this.binaryReader.ReadDouble();
                    data.Low = this.binaryReader.ReadDouble();
                    return data;
                case 1:
                    StoredAdjustmentData adj = new StoredAdjustmentData();
                    adj.EncodedDate = (ulong)this.binaryReader.ReadInt64();
                    adj.Adjustment = this.binaryReader.ReadDouble();
                    adj.Div = this.binaryReader.ReadDouble();
                    adj.Numerator = this.binaryReader.ReadUInt32();
                    adj.Denominator = this.binaryReader.ReadUInt32();
                    return adj;
                default:
                    throw new EncogError("Invalid file");
            }
            return null;
        }
    }
}
