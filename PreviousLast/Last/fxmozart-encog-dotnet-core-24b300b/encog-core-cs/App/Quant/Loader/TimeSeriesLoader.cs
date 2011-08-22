using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Encog.Util.CSV;

namespace Encog.App.Quant.Loader
{
    class TimeSeriesLoader
    {
        #region IMarketLoader Members

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
            var response = (HttpWebResponse)http.GetResponse();
            try
            {
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
            catch (System.Exception ex)
            {
                LoaderError ne = new LoaderError(ex);

                return null;
            }

        }

        #endregion
    }
}