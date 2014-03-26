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
using System.IO;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.Market
{
    public class MarketTrain
    {
        public static void Train(FileInfo dataDir)
        {
            FileInfo networkFile = FileUtil.CombinePath(dataDir, Config.NETWORK_FILE);
            FileInfo trainingFile = FileUtil.CombinePath(dataDir, Config.TRAINING_FILE);

            // network file
            if (!networkFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + networkFile);
                return;
            }

            var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(networkFile);

            // training file
            if (!trainingFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + trainingFile);
                return;
            }

            IMLDataSet trainingSet = EncogUtility.LoadEGB2Memory(trainingFile);

            // train the neural network
            EncogUtility.TrainConsole(network, trainingSet, Config.TRAINING_MINUTES);

            Console.WriteLine(@"Final Error: " + network.CalculateError(trainingSet));
            Console.WriteLine(@"Training complete, saving network.");
            EncogDirectoryPersistence.SaveObject(networkFile, network);
            Console.WriteLine(@"Network saved.");

            EncogFramework.Instance.Shutdown();
        }
    }
}
