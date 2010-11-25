using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Encog.Neural.NeuralData.Market.DB;
using System.IO;

namespace Encog.Neural.NeuralData.Market.DB
{
    public class PriceAdjustments
    {
        private String ticker;
        private SortedList<StoredAdjustmentData, object> data = new SortedList<StoredAdjustmentData, object>();
        private MarketDataStorage storage;

        public PriceAdjustments(MarketDataStorage storage, String ticker)
        {
            this.ticker = ticker;
            this.storage = storage;
        }

        public SortedList<StoredAdjustmentData, object> Data
        {
            get
            {
                return this.data;
            }
        }

        public void Add(StoredAdjustmentData adj)
        {
            data.Add(adj,null);
        }

        public void Save()
        {
            String filename = this.storage.GetAdjustmentFile(this.ticker);

            FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter writer = new BinaryWriter(stream);

            foreach( StoredAdjustmentData adj in this.data.Keys )
            {
                writer.Write(adj.EncodedDate);
                writer.Write(adj.Adjustment);
                writer.Write(adj.Div);
                writer.Write(adj.Numerator);
                writer.Write(adj.Denominator);
            }

            writer.Close();
            stream.Close();

        }

        public void Load()
        {
            String filename = this.storage.GetAdjustmentFile(this.ticker);

            FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream);
            long length = stream.Length;

            while (stream.Position < length)
            {
                StoredAdjustmentData adj = new StoredAdjustmentData();
                adj.EncodedDate = (ulong)reader.ReadInt64();
                adj.Adjustment = reader.ReadDouble();
                adj.Div = reader.ReadDouble();
                adj.Numerator = reader.ReadUInt32();
                adj.Denominator = reader.ReadUInt32();
                this.data.Add(adj, null);
            }
            reader.Close();
            stream.Close();
        }

        public double CalculateAdjustment(DateTime when)
        {
            double result = 1.0;

            foreach (StoredAdjustmentData adj in this.data.Keys)
            {
                if(  adj.Time.CompareTo(when)>0 )
                    result *= adj.Adjustment;
            }

            return result;
        }

        public void Clear()
        {
            this.data.Clear();
        }
    }
}
