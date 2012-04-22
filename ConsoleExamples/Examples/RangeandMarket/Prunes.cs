//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Pattern;
using Encog.Neural.Prune;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.RangeandMarket
{
    public static class Prunes
    {
        public static void Incremental(FileInfo dataDir, string networkfiletosave, string trainingfile)
        {
            FileInfo file = FileUtil.CombinePath(dataDir, networkfiletosave);
            FileInfo trainfile = FileUtil.CombinePath(dataDir, trainingfile);

            if (!trainfile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + trainfile);
                return;
            }

            IMLDataSet training = EncogUtility.LoadEGB2Memory(trainfile);


            var pattern = new FeedForwardPattern
            {
                InputNeurons = training.InputSize,
                OutputNeurons = training.IdealSize,
                ActivationFunction = new ActivationTANH()
            };

            var prune = new PruneIncremental(training, pattern, 100, 1, 10,
                                             new ConsoleStatusReportable());

            prune.AddHiddenLayer(5, 50);
            prune.AddHiddenLayer(0, 50);

            prune.Process();

            Encog.Util.NetworkUtil.NetworkUtility.SaveTraining(dataDir.Directory.FullName, trainingfile, training);

            EncogDirectoryPersistence.SaveObject(file, prune.BestNetwork);
        }
    }
}
