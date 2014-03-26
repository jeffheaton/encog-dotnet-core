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
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.Normalize;
using Encog.Util.Normalize.Output.Nominal;

namespace Encog.Examples.Forest
{
    public class Evaluate
    {
        private readonly ForestConfig _config;
        private readonly int[] _treeCorrect = new int[10];
        private readonly int[] _treeCount = new int[10];

        public Evaluate(ForestConfig config)
        {
            _config = config;
        }

        public void KeepScore(int actual, int ideal)
        {
            _treeCount[ideal]++;
            if (actual == ideal)
                _treeCorrect[ideal]++;
        }

        public BasicNetwork LoadNetwork()
        {
            FileInfo file = _config.TrainedNetworkFile;

            if (!file.Exists)
            {
                Console.WriteLine(@"Can't read file: " + file);
                return null;
            }

            var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(file);

            return network;
        }

        public DataNormalization LoadNormalization()
        {
            DataNormalization norm = null;

            if (_config.NormalizeFile.Exists)
            {
                norm = (DataNormalization) SerializeObject.Load(_config.NormalizeFile.ToString());
            }

            if (norm == null)
            {
                Console.WriteLine(@"Can't find normalization resource: "
                                  + _config.NormalizeFile);
                return null;
            }

            return norm;
        }

        public int DetermineTreeType(OutputEquilateral eqField, IMLData output)
        {
            int result;

            if (eqField != null)
            {
                result = eqField.Equilateral.Decode(output);
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

            var csv = new ReadCSV(_config.EvaluateFile.ToString(), false, ',');
            var input = new double[norm.InputFields.Count];
            var eqField = (OutputEquilateral) norm.FindOutputField(
                typeof (OutputEquilateral), 0);

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
                int coverTypeIdeal = (int) csv.GetDouble(54) - 1;

                KeepScore(coverTypeActual, coverTypeIdeal);

                if (coverTypeActual == coverTypeIdeal)
                {
                    correct++;
                }
            }

            Console.WriteLine(@"Total cases:" + total);
            Console.WriteLine(@"Correct cases:" + correct);
            double percent = correct/(double) total;
            Console.WriteLine(@"Correct percent:"
                              + Format.FormatPercentWhole(percent));
            for (int i = 0; i < 7; i++)
            {
                double p = (_treeCorrect[i]/(double) _treeCount[i]);
                Console.WriteLine(@"Tree Type #" + i + @" - Correct/total: "
                                  + _treeCorrect[i] + @"/" + _treeCount[i] + @"("
                                  + Format.FormatPercentWhole(p) + @")");
            }
        }
    }
}
