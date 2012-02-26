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

            if (dataDir.Directory != null)
            {
                Encog.Util.NetworkUtil.NetworkUtility.SaveTraining(dataDir.Directory.FullName, trainingfile, training);

                EncogDirectoryPersistence.SaveObject(file, prune.BestNetwork);
            }
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
                if (System.String.Compare(app.Args[0], "prune", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    PrunerLoader.Incremental(dataDir, app.Args[2], app.Args[3]);
                }
            }
        }
    }
}
