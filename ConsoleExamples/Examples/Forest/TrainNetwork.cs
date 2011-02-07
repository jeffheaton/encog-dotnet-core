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
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Logic;
using Encog.Persist;
using Encog.Neural.Data.Buffer;
using Encog.Util.Simple;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.Neural.Data.Basic;

namespace Encog.Examples.Forest
{
    public class TrainNetwork
    {
        private IExampleInterface app;

        public TrainNetwork(IExampleInterface app)
        {
            this.app = app;
        }

        public void Train(bool useGUI)
        {
            BufferedNeuralDataSet dataFile = new BufferedNeuralDataSet(Constant.BINARY_FILE);
            INeuralDataSet trainingSet = dataFile.LoadToMemory();
            int inputSize = trainingSet.InputSize;
            int idealSize = trainingSet.IdealSize;

            BasicNetwork network = EncogUtility.SimpleFeedForward(inputSize, Constant.HIDDEN_COUNT, 0, idealSize, true);

            if (useGUI)
            {
                EncogUtility.TrainDialog(network, trainingSet);
            }
            else
            {
                EncogUtility.TrainConsole(network, trainingSet, Constant.TRAINING_MINUTES);
            }

            EncogMemoryCollection encog = new EncogMemoryCollection();
            if (File.Exists(Constant.TRAINED_NETWORK_FILE))
                encog.Load(Constant.TRAINED_NETWORK_FILE);
            encog.Add(Constant.TRAINED_NETWORK_NAME, network);
            encog.Save(Constant.TRAINED_NETWORK_FILE);

            app.WriteLine("Training complete, saving network...");
        }
    }
}
