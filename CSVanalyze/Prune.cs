using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Util.File;
using Encog.ML.Data;
using Encog.Neural.Pattern;
using Encog.Engine.Network.Activation;
using Encog.Neural.Prune;
using Encog;
using Encog.Util.Simple;
using Encog.Persist;

namespace CSVanalyze
{
    public class Prune
    {
        #region prune networks

        public static void Incremental(string filetoget, string CurrentDirectory)
        {
            FileInfo file = FileUtil.CombinePath(new FileInfo(CurrentDirectory), filetoget);

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



        #endregion

    }
}
