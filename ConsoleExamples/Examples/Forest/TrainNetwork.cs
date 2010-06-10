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
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Logic;
using Encog.Persist;
using Encog.Normalize;
using Encog.Neural.Data.Buffer;
using Encog.Util.Simple;
using System.IO;

namespace Encog.Examples.Forest
{
    public class TrainNetwork
    {
        private IExampleInterface app;

        public TrainNetwork(IExampleInterface app)
        {
            this.app = app;
        }

        public BasicNetwork GenerateNetwork(INeuralDataSet trainingSet)
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, trainingSet.InputSize));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, Constant.HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, trainingSet.IdealSize));
            network.Logic = new FeedforwardLogic();
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public void Train(bool useGUI)
        {
            app.WriteLine("Converting training file to binary");
            EncogPersistedCollection encog = new EncogPersistedCollection(Constant.TRAINED_NETWORK_FILE, FileMode.Open);
            DataNormalization norm = (DataNormalization)encog.Find(Constant.NORMALIZATION_NAME);

            EncogUtility.ConvertCSV2Binary(Constant.NORMALIZED_FILE, Constant.BINARY_FILE, norm.GetNetworkInputLayerSize(), norm.GetNetworkOutputLayerSize(), false);
            BufferedNeuralDataSet trainingSet = new BufferedNeuralDataSet(Constant.BINARY_FILE);

            BasicNetwork network = (BasicNetwork)encog.Find(Constant.TRAINED_NETWORK_NAME);
            if (network == null)
                network = EncogUtility.SimpleFeedForward(norm.GetNetworkInputLayerSize(), Constant.HIDDEN_COUNT, 0, norm.GetNetworkOutputLayerSize(), false);

            if (useGUI)
            {
                EncogUtility.TrainDialog(network, trainingSet);
            }
            else
            {
                EncogUtility.TrainConsole(network, trainingSet, Constant.TRAINING_MINUTES);
            }

            app.WriteLine("Training complete, saving network...");
            encog.Add(Constant.TRAINED_NETWORK_NAME, network);
        }

    }
}
