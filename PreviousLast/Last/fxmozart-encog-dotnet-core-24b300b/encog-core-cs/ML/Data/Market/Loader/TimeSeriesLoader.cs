// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Encog.App.Quant;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.HTTP;


namespace Encog.ML.Data.Market.Loader
{
    class TimeSeriesLoader
    {
    

        public const string basesite = "http://robjhyndman.com/tsdldata/data/";
        /// <summary>
        /// Load the specified financial data. 
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">The financial data needed.</param>
        /// <param name="from">The beginning date to load data from.</param>
        /// <param name="to">The ending date to load data to.</param>
        /// <returns>A collection of LoadedMarketData objects that represent the data
        /// loaded.</returns>
        public ICollection<double> Load(string filenamed)
        {
            ICollection<double> result = new List<double>();
            Uri url = new Uri(basesite + filenamed);
            WebRequest http = WebRequest.Create(url);
            var response = (HttpWebResponse) http.GetResponse();

            if (response != null)
                using (Stream istream = response.GetResponseStream())
                {
                    var csv = new ReadCSV(istream, true, ' ');

                    while (csv.Next())
                    {
                       
                        double close = csv.GetDouble(0);
                       
                        result.Add(close);
                    }

                    csv.Close();
                    if (istream != null) istream.Close();
                }
            return result;
        }
        public string GetFile(string file)
        {
            return null;
        }
    }
}
#endif