using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Bot.HTML;
using System.Net;
using Encog.Util;

namespace Encog.Neural.Data.Market.Loader
{
    class YahooFinanceLoader:IMarketLoader
    {
        	/**
	 * This method builds a URL to load data from Yahoo Finance for a neural
	 * network to train with.
	 * 
	 * @param ticker
	 *            The ticker symbol to access.
	 * @param from
	 *            The beginning date.
	 * @param to
	 *            The ending date.
	 * @return The UEL
	 * @throws IOException
	 *             An error accessing the data.
	 */
	private Uri buildURL( TickerSymbol ticker,  DateTime from,
			 DateTime to) {

		// construct the URL
                 MemoryStream mstream = new MemoryStream();
                 FormUtility form = new FormUtility(mstream, null);

		form.Add("s", ticker.Symbol.ToUpper());
		form.Add("a", "" + from.Month);
		form.Add("b", "" + from.Day);
		form.Add("c", "" + from.Year);
		form.Add("d", "" + to.Month);
		form.Add("e", "" + to.Day);
		form.Add("f", "" + to.Year);
		form.Add("g", "d");
		form.Add("ignore", ".csv");
		mstream.Close();
		 String str = "http://ichart.finance.yahoo.com/table.csv?"
				+ mstream.ToString();
		return new Uri(str);
	}

	/**
	 * Load the specified financial data.
	 * 
	 * @param ticker
	 *            The ticker symbol to load.
	 * @param dataNeeded
	 *            The financial data needed.
	 * @param from
	 *            The beginning date to load data from.
	 * @param to
	 *            The ending date to load data to.
	 * @return A collection of LoadedMarketData objects that represent the data
	 *         loaded.
	 */
	public ICollection<LoadedMarketData> Load( TickerSymbol ticker,
			 IList<MarketDataType> dataNeeded,  DateTime from, 
			 DateTime to) {
		
			 ICollection<LoadedMarketData> result = 
				new List<LoadedMarketData>();
			 Uri url = buildURL(ticker, from, to);
             WebRequest http = HttpWebRequest.Create(url);
             HttpWebResponse response = (HttpWebResponse)http.GetResponse();

             using (Stream istream = response.GetResponseStream())
             {
                 ReadCSV csv = new ReadCSV(istream, true, ',');

                 while (csv.Next())
                 {
                     DateTime date = csv.GetDate("date");
                     double adjClose = csv.GetDouble("adj close");
                     double open = csv.GetDouble("open");
                     double close = csv.GetDouble("close");
                     double high = csv.GetDouble("high");
                     double low = csv.GetDouble("low");
                     double volume = csv.GetDouble("volume");

                     LoadedMarketData data =
                        new LoadedMarketData(date, ticker);
                     data.SetData(MarketDataType.ADJUSTED_CLOSE, adjClose);
                     data.SetData(MarketDataType.OPEN, open);
                     data.SetData(MarketDataType.CLOSE, close);
                     data.SetData(MarketDataType.HIGH, high);
                     data.SetData(MarketDataType.LOW, low);
                     data.SetData(MarketDataType.OPEN, open);
                     data.SetData(MarketDataType.VOLUME, volume);
                     result.Add(data);
                 }

                 csv.Close();
                 istream.Close();
             }
			return result;
		
	}

    }
}
