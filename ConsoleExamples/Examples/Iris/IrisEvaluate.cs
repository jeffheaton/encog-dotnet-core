using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Persist;
using System.IO;
using Encog.Neural.Data;
using Encog.Util.CSV;
using Encog.Engine.Util;
using Encog.Neural.Data.Basic;
using Encog.App.Quant.Normalize;
using Encog.App.Quant.Classify;

namespace Encog.Examples.Iris
{
    public class IrisEvaluate
    {
                private IExampleInterface app;

        private int[] irisCount = new int[10];
        private int[] irisCorrect = new int[10];

        public IrisEvaluate(IExampleInterface app)
        {
            this.app = app;
        }

        public void KeepScore(int actual, int ideal)
        {
            irisCount[ideal]++;
            if (actual == ideal)
                irisCorrect[ideal]++;
        }

        public BasicNetwork LoadNetwork()
        {
            String file = IrisConstant.TRAINED_NETWORK_FILE;

            if (!File.Exists(file))
            {
                app.WriteLine("Can't read file: " + file);
                return null;
            }

            EncogPersistedCollection encog = new EncogPersistedCollection(file, FileMode.Open);
            BasicNetwork network = (BasicNetwork)encog.Find(IrisConstant.TRAINED_NETWORK_NAME);

            if (network == null)
            {
                app.WriteLine("Can't find network resource: " + IrisConstant.TRAINED_NETWORK_NAME);
                return null;
            }

            return network;
        }

        public INeuralData BuildForNetworkInput(NormalizationStats stats, double[] input)
        {
            INeuralData neuralInput = new BasicNeuralData(4);
            for (int i = 0; i < input.Length; i++)
            {
                neuralInput[i] = stats[i].Normalize(input[i]);
            }

            return neuralInput;
        }

        public ClassItem DetermineType(ClassifyStats stats, INeuralData output)
        {
            ClassItem item = stats.DetermineClass(output.Data);
            return item;
        }

        public void PerformEvaluate()
        {
            BasicNetwork network = LoadNetwork();
          
            ReadCSV csv = new ReadCSV(IrisConstant.EVALUATE_FILE.ToString(), false, ',');
            double[] input = new double[IrisConstant.INPUT_COUNT];

            NormalizeCSV norm = new NormalizeCSV();
            norm.ReadStatsFile(IrisConstant.NORMALIZED_STATS_FILE);
            ClassifyStats stats = new ClassifyStats();
            stats.ReadStatsFile(IrisConstant.CLASSIFY_STATS_FILE);

            int correct = 0;
            int total = 0;
            while (csv.Next())
            {
                total++;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = csv.GetDouble(i);
                }

                INeuralData inputData = BuildForNetworkInput(norm.Stats, input);
                INeuralData output = network.Compute(inputData);
                ClassItem coverTypeActual = DetermineType(stats,output);
                String coverTypeIdealStr = csv.Get(4);
                int coverTypeIdeal = stats.Lookup(coverTypeIdealStr);

                KeepScore(coverTypeActual.Index, coverTypeIdeal);

                if (coverTypeActual.Index == coverTypeIdeal)
                {
                    correct++;
                }
            }

            app.WriteLine("Total cases:" + total);
            app.WriteLine("Correct cases:" + correct);
            double percent = (double)correct / (double)total;
            app.WriteLine("Correct percent:" + Format.FormatPercentWhole(percent));
            for (int i = 0; i < stats.Classes.Count; i++)
            {
                double p = ((double)this.irisCorrect[i] / (double)this.irisCount[i]);
                app.WriteLine("Iris Type: "
                        + stats.Classes[i].Name
                        + " - Correct/total: "
                        + this.irisCorrect[i]
                        + "/" + irisCount[i] + "(" + Format.FormatPercentWhole(p) + ")");
            }
        }

    }
}
