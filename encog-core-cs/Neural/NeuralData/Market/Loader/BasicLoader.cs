using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Market.DB;
using System.IO;

namespace Encog.Neural.NeuralData.Market.Loader
{
    public class BasicLoader: IMarketLoader
    {
        private int lastYearWritten;
        private MarketDataStorage marketData;
        private SortedList<StoredMarketData, object> loaded = new SortedList<StoredMarketData, object>();

        public BasicLoader(MarketDataStorage marketData)
        {
            this.marketData = marketData;
        }

        public SortedList<StoredMarketData, object> Loaded
        {
            get
            {
                return this.loaded;
            }
        }

        protected void SelectFile(String ticker, int year)
        {
            if ( lastYearWritten != year )
            {
                if (lastYearWritten > 0)
                {
                    WriteLoaded(ticker);
                }
                this.lastYearWritten = year;
            }

            
        }      

        protected void WriteLoaded(String ticker)
        {
            String filename = this.marketData.GetSecurityFile(ticker, this.lastYearWritten);
            BinaryWriter stream = new BinaryWriter(new FileStream(this.marketData.GetSecurityFile(ticker, this.lastYearWritten), FileMode.Create, FileAccess.Write, FileShare.None));

            foreach (StoredMarketData data in this.loaded.Keys)
            {
                stream.Write(data.EncodedDate);
                stream.Write(data.EncodedTime);
                stream.Write(data.Volume);
                stream.Write((double)data.Open);
                stream.Write((double)data.Close);
                stream.Write((double)data.High);
                stream.Write((double)data.Low);
            }
            stream.Close();    
            this.loaded.Clear();
        }

        public MarketDataStorage Storage
        {
            get
            {
                return this.marketData;
            }            
        }
    }
}
