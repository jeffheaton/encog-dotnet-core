//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.App.Quant.Loader.OpenQuant
{
    public class OpenQuantLoader
    {
        private string instrument = "EURUSD";

        /// <summary>
        ///     Froms the bar series to a double array.
        /// </summary>
        /// <param name="thebarserie">The thebarserie.</param>
        /// <returns></returns>
        public static double[] FromBarSeriestoDouble(IDataSeries thebarserie)
        {
            var outputtedseries = new double[thebarserie.Count];
            int index = 0;
            foreach (Data.Data.Bar abar in thebarserie)
            {
                outputtedseries[index] = abar.Close;
                index++;
            }
            return outputtedseries;
        }

        /// <summary>
        ///     the data loader for openquant.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="dtfrom">The dtfrom.</param>
        /// <param name="dtto">The dtto.</param>
        /// <param name="barsize">The barsize.</param>
        /// <returns></returns>
        public static LoaderTypes OpenquantDataLoader(string instrument, DateTime dtfrom, DateTime dtto,
                                                      Data.Data.BarType bartype, long barsize)
        {
            var BarSerie = new DataArray();
            Console.WriteLine("Initialized the Openquant-Encog Loader");
            try
            {
                //	BarSerie = this.GetHistoricalBars(dtfrom, dtto, BarType.Time, barsize);
                //	BarSerie = this.GetHistoricalBars(this.MarketDataProvider,Instrument,dtfrom,dtto,(int)barsize);
                // BarSerie = DataManager.GetHistoricalBars(instrument, Data.Data.BarType.Time, 3600);

                var typeLoaded = new LoaderTypes(instrument, dtfrom, dtto, bartype, barsize);
                Console.WriteLine("Loaded Types instrument:" + typeLoaded.Instrument);
                return typeLoaded;
            }
            catch (Exception ex)
            {
                var er = new EncogError(ex);
                Console.WriteLine("Error :" + ex.Message);
                Console.WriteLine("Error :" + ex.StackTrace);
                Console.WriteLine("Error:" + ex.InnerException);
                Console.WriteLine("Full message:" + ex);
                return null;
            }
        }

        public class LoaderTypes
        {
            public LoaderTypes(string instrument, DateTime StartingDate, DateTime ToDate, Data.Data.BarType Type,
                               long SizeOfBars)
            {
                @from = StartingDate;
                to = ToDate;
                Barsize = SizeOfBars;
                Instrument = instrument;
                TypeBar = Type;
            }

            public DateTime from { get; set; }
            public DateTime to { get; set; }
            public long Barsize { get; set; }
            public string Instrument { get; set; }
            public Data.Data.BarType TypeBar { get; set; }
        }
    }
}
