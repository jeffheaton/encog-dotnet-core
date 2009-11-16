using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Normalize;
using Encog.Neural.NeuralData;
using Encog.Util.CSV;
using Encog.Normalize.Output.Nominal;
using Encog.Neural.Data.Buffer;
using Encog.Neural.Data;

namespace Sandbox
{
    public class Evaluate
    {
        public BasicNetwork LoadNetwork()
        {
            String file = Constant.TRAINED_NETWORK_FILE;

            if (!File.Exists(file))
            {
                Console.WriteLine("Can't read file: " + file);
                return null;
            }

            EncogPersistedCollection encog = new EncogPersistedCollection(file, FileMode.Open);
            BasicNetwork network = (BasicNetwork)encog.Find(Constant.TRAINED_NETWORK_NAME);

            if (network == null)
            {
                Console.WriteLine("Can't find network resource: " + Constant.TRAINED_NETWORK_NAME);
                return null;
            }

            return network;
        }

        public INeuralDataSet LoadData()
        {
            String file = Constant.EVAL_FILE;

            if (!File.Exists(file))
            {
                Console.WriteLine("Can't read file: " + file);
                return null;
            }

            BufferedNeuralDataSet trainingSet = new BufferedNeuralDataSet(Constant.EVAL_FILE);
            return trainingSet;
        }

        public DataNormalization LoadNormalization()
        {
            String file = Constant.TRAINED_NETWORK_FILE;

            EncogPersistedCollection encog = new EncogPersistedCollection(file, FileMode.Open);

            DataNormalization norm = (DataNormalization)encog.Find(Constant.NORMALIZATION_NAME);
            if (norm == null)
            {
                Console.WriteLine("Can't find normalization resource: " + Constant.NORMALIZATION_NAME);
                return null;
            }

            return norm;
        }

        public void PerformEvaluate()
        {
            BasicNetwork network = LoadNetwork();
            DataNormalization norm = LoadNormalization();

            ReadCSV csv = new ReadCSV(Constant.EVAL_FILE, false, ',');
            double[] input = new double[network.GetLayer(BasicNetwork.TAG_INPUT).NeuronCount];
            OutputEquilateral eqField = (OutputEquilateral)norm.FindOutputField(typeof(OutputEquilateral), 0);

            int correct = 0;
            int total = 0;
            while (csv.Next())
            {
                total++;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = csv.GetDouble(i);
                }
                INeuralData inputData = norm.BuildForNetworkInput(input);
                INeuralData output = network.Compute(inputData);
                int coverTypeActual = eqField.Equilateral.Decode(output.Data);
                int coverTypeIdeal = (int)csv.GetDouble(54) - 1;
                if (coverTypeActual == coverTypeIdeal)
                {
                    correct++;
                }
                Console.WriteLine(coverTypeActual + " - " + coverTypeIdeal);
            }

            Console.WriteLine("Total cases:" + total);
            Console.WriteLine("Correct cases:" + correct);
            double percent = (double)correct / (double)total;
            Console.WriteLine("Correct percent:" + (percent * 100.0));
        }
    }
}
