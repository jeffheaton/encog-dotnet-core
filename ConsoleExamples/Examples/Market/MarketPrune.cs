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
using Encog.Persist;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Prune;

namespace Encog.Examples.Market
{
    public class MarketPrune
    {
        public void Run()
        {
            String file = Config.FILENAME;

            if (!File.Exists(file))
            {
                Console.WriteLine("Can't read file: " + file);
                return;
            }

            EncogPersistedCollection encog = new EncogPersistedCollection(file, FileMode.Open);

            INeuralDataSet training = (INeuralDataSet)encog
                    .Find(Config.MARKET_TRAIN);
            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.InputNeurons = training.InputSize;
            pattern.OutputNeurons = training.IdealSize;
            pattern.ActivationFunction = new ActivationTANH();

            PruneIncremental prune = new PruneIncremental(training, pattern, 100,
                    new ConsoleStatusReportable());

            prune.AddHiddenLayer(5, 50);
            prune.AddHiddenLayer(0, 50);

            prune.Process();

            encog.Add(Config.MARKET_NETWORK, prune.BestNetwork);

        }
    }
}
