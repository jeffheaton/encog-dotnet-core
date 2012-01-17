// Encog Simple Candlestick Example
// Copyright 2010 by Jeff Heaton (http://www.jeffheaton.com)
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Neural.Data.Basic;
using Encog.Neural.NeuralData;


namespace EncogCandleStickExample
{
    public class GatherUtil
    {
        /// <summary>
        /// The percent that a stock falls below to be considered bearish.
        /// </summary>
        public double BearPercent { get; set; }

        /// <summary>
        /// The percent that a stock rises above to be considered bullish.
        /// </summary>
        public double BullPercent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EvalWindow { get; set; }
        public int PredictWindow { get; set; }

        /// <summary>
        /// Generate the input to the neural network to predict.  It will look at the current
        /// date as well as the number of days leading up to it specified by EvalWindow.
        /// This method is used both internally and externally.  
        /// </summary>
        /// <param name="marketData">The market data to use.</param>
        /// <param name="marketDataIndex">The point that we want to predict from.</param>
        /// <returns></returns>
        public INeuralData CreateData(
        List<LoadedMarketData> marketData,
        int marketDataIndex)
        {
            INeuralData neuralData = new BasicNeuralData(14);
            int totalPatterns = 0;
            int[] patternCount = new int[14];

            for (int i = 0; i < EvalWindow; i++)
            {
                LoadedMarketData data = marketData[(marketDataIndex-EvalWindow) + i];

                IdentifyCandleStick candle = new IdentifyCandleStick();
                candle.SetStats(data);
                int pattern = candle.DeterminePattern();
                if (pattern != IdentifyCandleStick.UNKNOWN)
                {
                    totalPatterns++;
                    patternCount[pattern]++;
                }
            }

            if (totalPatterns == 0)
                return null;

            for (int i = 0; i < 14; i++)
            {
                neuralData[i] = ((double)patternCount[i]) / ((double)totalPatterns);
            }

            return neuralData;
        }

        /// <summary>
        /// Create one training pair, either good or bad.
        /// </summary>
        /// <param name="data">The data to create from.</param>
        /// <param name="index">The index into the data to create from.</param>
        /// <param name="good">True if this was a good(bearish) period.</param>
        /// <returns></returns>
        public IMLDataPair CreateData(List<LoadedMarketData> data, int index, bool good)
        {
            BasicNeuralData ideal = new BasicNeuralData(1);

            INeuralData input = CreateData(data, index);

            if (input == null)
                return null;

            // ideal
            if (good)
                ideal[0] = 0.9;
            else
                ideal[0] = 0.1;

            return new BasicMLDataPair(input, ideal);
        }

        /// <summary>
        /// Called to load training data for a company.  This is how the training data is actually created.
        /// To prepare input data for recognition use the CreateData method.  The training set will be
        /// added to.  This allows the network to learn from multiple companies if this method is called
        /// multiple times.
        /// </summary>
        /// <param name="symbol">The ticker symbol.</param>
        /// <param name="training">The training set to add to.</param>
        /// <param name="from">Beginning date</param>
        /// <param name="to">Ending date</param>
        public void LoadCompany(String symbol, IMLDataSet training, DateTime from, DateTime to)
        {
            IMarketLoader loader = new YahooFinanceLoader();
            TickerSymbol ticker = new TickerSymbol(symbol);
            IList<MarketDataType> dataNeeded = new List<MarketDataType>();
            dataNeeded.Add(MarketDataType.AdjustedClose);
            dataNeeded.Add(MarketDataType.Close);
            dataNeeded.Add(MarketDataType.Open);
            dataNeeded.Add(MarketDataType.High);
            dataNeeded.Add(MarketDataType.Low);
            List<LoadedMarketData> results = (List<LoadedMarketData>)loader.Load(ticker, dataNeeded, from, to);
            results.Sort();

            for (int index = PredictWindow; index < results.Count - EvalWindow; index++)
            {
                LoadedMarketData data = results[index];

                // determine bull or bear position, or neither
                bool bullish = false;
                bool bearish = false;

                for (int search = 1; search <= EvalWindow; search++)
                {
                    LoadedMarketData data2 = results[index + search];
                    double priceBase = data.GetData(MarketDataType.AdjustedClose);
                    double priceCompare = data2.GetData(MarketDataType.AdjustedClose);
                    double diff = priceCompare - priceBase;
                    double percent = diff / priceBase;
                    if (percent > BullPercent)
                    {
                        bullish = true;
                    }
                    else if (percent < BearPercent)
                    {
                        bearish = true;
                    }
                }

                IMLDataPair pair = null;

                if (bullish)
                {
                    pair = CreateData(results, index, true);
                }
                else if (bearish)
                {
                    pair = CreateData(results, index, false);
                }

                if (pair != null)
                {
                    training.Add(pair);
                }
            }
        }
    }
}
