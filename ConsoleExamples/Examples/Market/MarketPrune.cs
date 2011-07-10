using System;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Pattern;
using Encog.Neural.Prune;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.Market
{
    public class MarketPrune
    {
        public static void Incremental(FileInfo dataDir)
        {
            FileInfo file = FileUtil.CombinePath(dataDir, Config.TRAINING_FILE);

            if (!file.Exists)
            {
                Console.WriteLine(@"Can't read file: " + file);
                return;
            }

            IMLDataSet training = EncogUtility.LoadEGB2Memory(file);

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

            EncogDirectoryPersistence.SaveObject(file, prune.BestNetwork);
        }
    }
}