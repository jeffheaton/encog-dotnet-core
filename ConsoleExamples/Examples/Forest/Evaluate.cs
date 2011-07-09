using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using System.IO;
using Encog.Persist;
using Encog.Util.Normalize;
using Encog.Util;
using Encog.Util.Normalize.Output.Nominal;
using Encog.Util.CSV;
using Encog.ML.Data;

namespace Encog.Examples.Forest
{
    public class Evaluate
    {
        private int[] treeCount = new int[10];
        private int[] treeCorrect = new int[10];
        private ForestConfig config;

        public Evaluate(ForestConfig config)
        {
            this.config = config;
        }

        public void KeepScore(int actual, int ideal)
        {
            treeCount[ideal]++;
            if (actual == ideal)
                treeCorrect[ideal]++;
        }

        public BasicNetwork LoadNetwork()
        {
            FileInfo file = config.TrainedNetworkFile;

            if (!file.Exists)
            {
                Console.WriteLine("Can't read file: " + file.ToString());
                return null;
            }

            BasicNetwork network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(file);

            return network;
        }

        public DataNormalization LoadNormalization()
        {

            DataNormalization norm = null;

            if (config.NormalizeFile.Exists)
            {
                norm = (DataNormalization)SerializeObject.Load(config.NormalizeFile.ToString());
            }

            if (norm == null)
            {
                Console.WriteLine("Can't find normalization resource: "
                        + config.NormalizeFile.ToString());
                return null;
            }

            return norm;
        }

        public int DetermineTreeType(OutputEquilateral eqField, IMLData output)
        {
            int result = 0;

            if (eqField != null)
            {
                result = eqField.Equilateral.Decode(output.Data);
            }
            else
            {
                double maxOutput = Double.NegativeInfinity;
                result = -1;

                for (int i = 0; i < output.Count; i++)
                {
                    if (output[i] > maxOutput)
                    {
                        maxOutput = output[i];
                        result = i;
                    }
                }
            }

            return result;
        }

        public void EvaluateNetwork()
        {
            BasicNetwork network = LoadNetwork();
            DataNormalization norm = LoadNormalization();

            ReadCSV csv = new ReadCSV(config.EvaluateFile.ToString(), false, ',');
            double[] input = new double[norm.InputFields.Count];
            OutputEquilateral eqField = (OutputEquilateral)norm.FindOutputField(
                    typeof(OutputEquilateral), 0);

            int correct = 0;
            int total = 0;
            while (csv.Next())
            {
                total++;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = csv.GetDouble(i);
                }
                IMLData inputData = norm.BuildForNetworkInput(input);
                IMLData output = network.Compute(inputData);
                int coverTypeActual = DetermineTreeType(eqField, output);
                int coverTypeIdeal = (int)csv.GetDouble(54) - 1;

                KeepScore(coverTypeActual, coverTypeIdeal);

                if (coverTypeActual == coverTypeIdeal)
                {
                    correct++;
                }
            }

            Console.WriteLine("Total cases:" + total);
            Console.WriteLine("Correct cases:" + correct);
            double percent = (double)correct / (double)total;
            Console.WriteLine("Correct percent:"
                    + Format.FormatPercentWhole(percent));
            for (int i = 0; i < 7; i++)
            {
                double p = ((double)this.treeCorrect[i] / (double)this.treeCount[i]);
                Console.WriteLine("Tree Type #" + i + " - Correct/total: "
                        + this.treeCorrect[i] + "/" + treeCount[i] + "("
                        + Format.FormatPercentWhole(p) + ")");
            }
        }
    }
}
