using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Market.DB;
using System.IO;

namespace Encog.Neural.Data.Market.Loader
{
    public class BasicLoader
    {
        private BinaryWriter streamData;
        private int lastYearWritten;
        private MarketDataStoreage marketData;

        public BasicLoader(MarketDataStoreage marketData)
        {
            this.marketData = marketData;
        }

        protected void SelectFile(String ticker, int year)
        {
            if (streamData == null || lastYearWritten != year)
            {
                if (streamData != null)
                    streamData.Close();

                String filename = this.marketData.GetSecurityFile(ticker, year);
                this.streamData = new BinaryWriter(new FileStream(this.marketData.GetSecurityFile(ticker, year), FileMode.Create, FileAccess.Write, FileShare.None));
                this.lastYearWritten = year;
            }
        }


        protected void WriteObject(object o)
        {
            if (o is StoredMarketData)
            {
                StoredMarketData data = (StoredMarketData)o;
                this.streamData.Write((byte)0);
                this.streamData.Write(data.EncodedDate);
                this.streamData.Write(data.Volume);
                this.streamData.Write(data.Open);
                this.streamData.Write(data.Close);
                this.streamData.Write(data.High);
                this.streamData.Write(data.Low);
            }
            else if (o is StoredAdjustmentData)
            {
                StoredAdjustmentData adj = (StoredAdjustmentData)o;
                this.streamData.Write((byte)1);
                this.streamData.Write(adj.EncodedDate);
                this.streamData.Write(adj.Adjustment);
                this.streamData.Write(adj.Div);
                this.streamData.Write(adj.Numerator);
                this.streamData.Write(adj.Denominator);
            }
        }

        public void Close()
        {
            if (streamData != null)
                streamData.Close();
            streamData = null;
        }

        public MarketDataStoreage Storage
        {
            get
            {
                return this.marketData;
            }            
        }
    }
}
