using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Loader.OpenQuant
{
    public class OpenQuantLoader
    {

        /// <summary>
        /// A class to know which types have been loaded.
        /// </summary>
        public class LoaderTypes
        {
            public DateTime from { get; set; }
            public DateTime to { get; set; }
            public long Barsize { get; set; }
            public string Instrument { get; set; }
            public Data.Data.BarType TypeBar { get; set; }

            public LoaderTypes(string instrument, DateTime StartingDate, DateTime ToDate,Data.Data.BarType Type, long SizeOfBars)
            {
                this.from = StartingDate;
                this.to = ToDate;
                this.Barsize = SizeOfBars;
                this.Instrument = instrument;
                this.TypeBar = Type;
            }
        }


        private string instrument = "EURUSD";
        /// <summary>
        /// Froms the bar series to a double array.
        /// </summary>
        /// <param name="thebarserie">The thebarserie.</param>
        /// <returns></returns>
        public static double[] FromBarSeriestoDouble(Encog.App.Quant.Loader.OpenQuant.IDataSeries thebarserie)
        {

            double[] outputtedseries = new double[thebarserie.Count];
            int index = 0;
            foreach (Data.Data.Bar abar in thebarserie)
            {
                outputtedseries[index] = abar.Close;
                index++;
            }
            return outputtedseries;
        }

        /// <summary>
        /// the data loader for openquant.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="dtfrom">The dtfrom.</param>
        /// <param name="dtto">The dtto.</param>
        /// <param name="barsize">The barsize.</param>
        /// <returns></returns>
        public static LoaderTypes OpenquantDataLoader(string instrument, DateTime dtfrom, DateTime dtto, Data.Data.BarType bartype, long barsize)
        {

            DataArray BarSerie = new DataArray();
            Console.WriteLine("Initialized the Openquant-Encog Loader");
            try
            {
                //	BarSerie = this.GetHistoricalBars(dtfrom, dtto, BarType.Time, barsize);
                //	BarSerie = this.GetHistoricalBars(this.MarketDataProvider,Instrument,dtfrom,dtto,(int)barsize);
                // BarSerie = DataManager.GetHistoricalBars(instrument, Data.Data.BarType.Time, 3600);

                LoaderTypes typeLoaded = new LoaderTypes(instrument, dtfrom, dtto,bartype, barsize);

                Console.WriteLine("Loaded Types instrument:" + typeLoaded.Instrument);
                return typeLoaded;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error :" + ex.Message);
                Console.WriteLine("Error :" + ex.StackTrace);
                Console.WriteLine("Error:" + ex.InnerException);
                Console.WriteLine("Full message:" + ex);
                return null;
            }
        }


        //public static DataArray DoubleSeriesForEncog(object ReceivingObject)
        //{
        //    bool b = ReceivingObject is DataArray;
        //    DataArray dat = new DataArray();
        //    if (b)
        //    {
        //        foreach (Data.Data.Bar d in ReceivingObject)
        //        {
        //            dat.Add(d);

        //        }
        //    }
        //    return dat;
        //}
    }
}
