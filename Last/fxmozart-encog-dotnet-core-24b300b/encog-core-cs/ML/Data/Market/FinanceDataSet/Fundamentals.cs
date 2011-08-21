
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Parse.Tags;

namespace Encog.ML.Data.Market.FinanceDataSet
{
    public class Fundamental : App.Quant.Loader.OpenQuant.Data.Data.IDataObject, ICloneable
    {
        // Fields
        private DateTime m_exchangeTimeStamp;
        private DateTimeKind m_exchangeTSKind;
        private byte m_providerId;
        private DateTime m_providerTimeStamp;
        private DateTimeKind m_providerTSKind;
       

        // Methods
        public object Clone()
        {
            return this;
        }

        private DateTimeKind GetDateTimeKind(int value)
        {
            switch (value)
            {
                case 0:
                    return DateTimeKind.Unspecified;

                case 1:
                    return DateTimeKind.Utc;

                case 2:
                    return DateTimeKind.Local;
            }
            throw new Exception("Unknown DateTimeKind value:" + value);
        }



        public void ReadFrom(BinaryReader reader)
        {
            byte num = reader.ReadByte();
            switch (num)
            {
                case 1:
                    this.m_providerTimeStamp = new DateTime(reader.ReadInt64());
                    this.m_providerId = reader.ReadByte();
                  
                    return;

                case 2:
                    this.m_providerTSKind = this.GetDateTimeKind(reader.ReadInt32());
                    this.m_exchangeTSKind = this.GetDateTimeKind(reader.ReadInt32());
                    this.m_providerTimeStamp = new DateTime(reader.ReadInt64(), this.m_providerTSKind);
                    this.m_exchangeTimeStamp = new DateTime(reader.ReadInt64(), this.m_exchangeTSKind);
                    this.m_providerId = reader.ReadByte();
               return;
            }
            throw new Exception("Unknown version - " + num);
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)2);
            writer.Write((int)this.m_providerTSKind);
            writer.Write((int)this.m_exchangeTSKind);
            writer.Write(this.m_providerTimeStamp.Ticks);
            writer.Write(this.m_exchangeTimeStamp.Ticks);
            writer.Write(this.m_providerId);

        }

        // Properties
        public double CashFlowPerShare { get; set;}

        public double CashPerShare { get; set; }

        public DateTime DateTime
        {
            get
            {
                return this.ProviderUTCTimeStamp;
            }
            set
            {
                this.ProviderUTCTimeStamp = value;
            }
        }

        public double DebtPerShare { get; set; }

        public double EarningsPerShare { get; set; }

        public double InterestPaymentPerShare { get; set; }

        public DateTime MarketUTCTimeStamp
        {
            get
            {
                return this.m_exchangeTimeStamp;
            }
            set
            {
                this.m_exchangeTimeStamp = value;
                this.m_exchangeTSKind = value.Kind;
            }
        }

        public byte ProviderId
        {
            get
            {
                return this.m_providerId;
            }
            set
            {
                this.m_providerId = value;
            }
        }

        public DateTime ProviderUTCTimeStamp
        {
            get
            {
                return this.m_providerTimeStamp;
            }
            set
            {
                this.m_providerTimeStamp = value;
                this.m_providerTSKind = value.Kind;
            }
        }

            

      

 

 

    }
}


