//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML.Data.Buffer;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.CSV;
using Encog.Util.Simple;

namespace Encog.Examples.Forest
{
    public class TrainNetwork
    {
        private readonly ForestConfig _config;

        public TrainNetwork(ForestConfig config)
        {
            _config = config;
        }

        public void Train(bool useGui)
        {
            // load, or create the neural network
            BasicNetwork network;

            if (!_config.TrainedNetworkFile.Exists)
            {
                throw new EncogError(@"Can't find neural network file, please generate data");
            }

            network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(_config.TrainedNetworkFile);

            // convert training data
            Console.WriteLine(@"Converting training file to binary");


            EncogUtility.ConvertCSV2Binary(
                _config.NormalizedDataFile.ToString(),
                CSVFormat.English,
                _config.BinaryFile.ToString(),
                network.InputCount,
                network.OutputCount,
                false, false);

            var trainingSet = new BufferedMLDataSet(
                _config.BinaryFile.ToString());


            if (useGui)
            {
                EncogUtility.TrainDialog(network, trainingSet);
            }
            else
            {
                EncogUtility.TrainConsole(network, trainingSet,
                                          _config.TrainingMinutes);
            }

            Console.WriteLine(@"Training complete, saving network...");
            EncogDirectoryPersistence.SaveObject(_config.TrainedNetworkFile, network);
        }
    }
}
