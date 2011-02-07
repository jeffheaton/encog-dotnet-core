// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Data;
using System.IO;
using Encog.Persist;
using Encog.Util.CSV;
using Encog.Util;
using Encog.Engine.Util;
using Encog.App.Quant.Normalize;
using Encog.Neural.Data.Basic;
using Encog.App.Quant.Classify;

namespace Encog.Examples.Forest
{
    public class Evaluate
    {
        private IExampleInterface app;

        private int[] treeCount = new int[10];
        private int[] treeCorrect = new int[10];

        public Evaluate(IExampleInterface app)
        {
            this.app = app;
        }

        public void KeepScore(int actual, int ideal)
        {
            treeCount[ideal]++;
            if (actual == ideal)
                treeCorrect[ideal]++;
        }

        
        public BasicNetwork LoadNetwork()
        {
            String file = Constant.TRAINED_NETWORK_FILE;

            if (!File.Exists(file))
            {
                app.WriteLine("Can't read file: " + file);
                return null;
            }

            EncogPersistedCollection encog = new EncogPersistedCollection(file, FileMode.Open);
            BasicNetwork network = (BasicNetwork)encog.Find(Constant.TRAINED_NETWORK_NAME);

            if (network == null)
            {
                app.WriteLine("Can't find network resource: " + Constant.TRAINED_NETWORK_NAME);
                return null;
            }

            return network;
        }

        public INeuralData BuildForNetworkInput(NormalizationStats stats, double[] input)
        {
            INeuralData neuralInput = new BasicNeuralData(input.Length);
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
          
            ReadCSV csv = new ReadCSV(Constant.EVALUATE_FILE.ToString(), false, ',');
            double[] input = new double[Constant.INPUT_COUNT];

            NormalizeCSV norm = new NormalizeCSV();
            norm.ReadStatsFile(Constant.NORMALIZED_STATS_FILE);
            ClassifyStats stats = new ClassifyStats();
            stats.ReadStatsFile(Constant.CLASSIFY_STATS_FILE);

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
                ClassItem coverTypeActual = DetermineType(stats, output);
                String coverTypeIdealStr = csv.Get(54);
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
            for (int i = 0; i < 7; i++)
            {
                if (this.treeCount[i] > 0)
                {
                    double p = ((double)this.treeCorrect[i] / (double)this.treeCount[i]);
                    app.WriteLine("Tree Type #"
                            + i
                            + " - Correct/total: "
                            + this.treeCorrect[i]
                            + "/" + treeCount[i] + "(" + Format.FormatPercentWhole(p) + ")");
                }
            }
        }
    }
}
