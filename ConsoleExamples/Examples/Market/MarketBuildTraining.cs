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
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using System.IO;
using Encog.App.Quant.MarketDB;
using Encog.App.Quant.Loader.YahooFinance;
using Encog.App.Quant.Dataset;

namespace Encog.Examples.Market
{
    public class MarketBuildTraining
    {
        public void Run()
        {
            Console.WriteLine("Downloading market data");
            Logging.StopConsoleLogging();
            MarketDataStorage store = new MarketDataStorage();
            YahooDownload loader = new YahooDownload(store);
            loader.LoadAllData("aapl");

            Console.WriteLine("Building training data");
            MarketNeuralDataSet market = new MarketNeuralDataSet(
                    store,
                    Config.INPUT_WINDOW,
                    Config.PREDICT_WINDOW,
                    BarPeriod.EOD);
            
            MarketDataDescription desc = new MarketDataDescription(
                    Config.TICKER,
                    MarketDataType.CLOSE,
                    true,
                    true);
            market.AddDescription(desc);

            DateTime begin = new DateTime(
                Config.TRAIN_BEGIN_YEAR, 
                Config.TRAIN_BEGIN_MONTH, 
                Config.TRAIN_BEGIN_DAY);

            DateTime end = new DateTime(
                Config.TRAIN_END_YEAR,
                Config.TRAIN_END_MONTH,
                Config.TRAIN_END_DAY);

            market.Load(begin, end);
            market.Generate();
            market.Description = "Market data for: " + Config.TICKER;

            // create a network
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(market.InputSize));
            network.AddLayer(new BasicLayer(Config.HIDDEN1_COUNT));
            if (Config.HIDDEN2_COUNT != 0)
                network.AddLayer(new BasicLayer(Config.HIDDEN2_COUNT));
            network.AddLayer(new BasicLayer(market.IdealSize));
            network.Structure.FinalizeStructure();
            network.Reset();

            // save the network and the training
            EncogPersistedCollection encog = new EncogPersistedCollection(Config.FILENAME,FileMode.Create);
            encog.Create();
            encog.Add(Config.MARKET_TRAIN, market);
            encog.Add(Config.MARKET_NETWORK, network);
        }
    }
}
