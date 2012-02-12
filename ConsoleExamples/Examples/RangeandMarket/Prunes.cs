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
