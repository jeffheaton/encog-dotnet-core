// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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
using System.IO;
using Encog.Util.Logging;
using Encog.Persist;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Examples.Market
{
    public class MarketTrain
    {
        const int tickCount = 10000;

        public void Run()
        {
            Logging.StopConsoleLogging();

            if (!File.Exists(Config.FILENAME))
            {
                Market.app.WriteLine("Can't read file: " + Config.FILENAME);
                return;
            }

            EncogPersistedCollection encog = new EncogPersistedCollection(Config.FILENAME, FileMode.Open);
            INeuralDataSet trainingSet = (INeuralDataSet)encog.Find(Config.MARKET_TRAIN);

            BasicNetwork network = (BasicNetwork)encog.Find(Config.MARKET_NETWORK);

            // train the neural network
            ITrain train = new ResilientPropagation(network, trainingSet);
            //final Train train = new Backpropagation(network, trainingSet, 0.0001, 0.0);



            int epoch = 1;
            long startTime = DateTime.Now.Ticks;
            int left = 0;

            do
            {
                int running = (int)((DateTime.Now.Ticks - startTime) / (60000*tickCount) );
                left = Config.TRAINING_MINUTES - running;
                train.Iteration();
                Market.app.WriteLine("Epoch #" + epoch + " Error:" + (train.Error * 100.0) + "%,"
                                + " Time Left: " + left + " Minutes");
                epoch++;
            } while ((left >= 0) && (train.Error > 0.001));

            network.Description = "Trained neural network";
            encog.Add(Config.MARKET_NETWORK, network);
        }
    }
}
