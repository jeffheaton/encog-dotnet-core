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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Logging;
using System.IO;
using Encog.Persist;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData.Market;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.NeuralData.Market.Loader;

namespace Encog.Examples.Market
{
    public class MarketPredict
    {
        public enum Direction
        {
            up,
            down
        };

        public Direction determineDirection(double d)
        {
            if (d < 0)
                return Direction.down;
            else
                return Direction.up;
        }


        public MarketNeuralDataSet grabData()
        {
            IMarketLoader loader = new YahooFinanceLoader();
            MarketNeuralDataSet result = new MarketNeuralDataSet(
                    loader,
                    Config.INPUT_WINDOW,
                    Config.PREDICT_WINDOW);
            MarketDataDescription desc = new MarketDataDescription(
                    new TickerSymbol(Config.TICKER),
                    MarketDataType.ADJUSTED_CLOSE,
                    true,
                    true);
            result.AddDescription(desc);

            DateTime end = DateTime.Now;
            DateTime begin = end.AddDays(-60);

            result.Load(begin, end);
            result.Generate();

            return result;

        }

        public void Run()
        {
            Logging.StopConsoleLogging();

            if (!File.Exists(Config.FILENAME))
            {
                Market.app.WriteLine("Can't read file: " + Config.FILENAME);
                return;
            }

            EncogPersistedCollection encog = new EncogPersistedCollection(Config.FILENAME, FileMode.Open);
            BasicNetwork network = (BasicNetwork)encog.Find(Config.MARKET_NETWORK);

            if (network == null)
            {
                Market.app.WriteLine("Can't find network resource: " + Config.MARKET_NETWORK);
                return;
            }

            MarketNeuralDataSet data = grabData();

            int count = 0;
            int correct = 0;
            foreach (INeuralDataPair pair in data)
            {
                INeuralData input = pair.Input;
                INeuralData actualData = pair.Ideal;
                INeuralData predictData = network.Compute(input);

                double actual = actualData.Data[0];
                double predict = predictData.Data[0];
                double diff = Math.Abs(predict - actual);

                Direction actualDirection = determineDirection(actual);
                Direction predictDirection = determineDirection(predict);

                if (actualDirection == predictDirection)
                    correct++;

                count++;

                Market.app.WriteLine("Day " + count + ":actual="
                        + (actual*100) + "(" + actualDirection + ")"
                        + ",predict="
                        + (predict*100) + "(" + actualDirection + ")"
                        + ",diff=" + diff);

            }
            double percent = (double)correct / (double)count;
            Market.app.WriteLine("Direction correct:" + correct + "/" + count);
            Market.app.WriteLine("Directional Accuracy:" + percent + "%");

        }

    }

}
