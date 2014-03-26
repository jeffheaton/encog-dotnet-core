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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.Examples.Persist;
using Encog.ML.Data;
using Encog.Neural.Pattern;
using Encog.Neural.Prune;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.NetWorkPruner
{
    class PrunerLoader : IExample
    {
        //public const string MaketTrainingToLoad = @"c:\EncogOutput\ELMHANmarketdTrainingFile.egb";
   private IExampleInterface app;

   public static ExampleInfo Info
   {
       get
       {
           var info = new ExampleInfo(
               typeof(PrunerLoader),
               "Prunate",
               "Prunes network",
               "This example will prune networks. You must pass the training file of the network , and it will start the pruning process");
           return info;
       }


   }
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

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length < 3)
            {
                Console.WriteLine(@" [prunator ]  [data dir]  [trainingfile] [networkfile]");
            }
            else
            {
                var dataDir = new FileInfo(app.Args[1]);
                if (String.Compare(app.Args[0], "prune", true) == 0)
                {
                    PrunerLoader.Incremental(dataDir, app.Args[2], app.Args[3]);
                }
            }
        }
    }
}
