//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Kmeans;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.KMeans
{
    [TestClass]
    public class TestKMeans
    {
        public static double[][] Data = {
                                            new double[] {28, 15, 22},
                                            new double[] {16, 15, 32},
                                            new double[] {32, 20, 44},
                                            new double[] {1, 2, 3},
                                            new double[] {3, 2, 1}
                                        };

        [TestMethod]
        public void TestCluster()
        {
            var set = new BasicMLDataSet();

            int i;
            for (i = 0; i < Data.Length; i++)
            {
                set.Add(new BasicMLData(Data[i]));
            }

            var kmeans = new KMeansClustering(2, set);
            kmeans.Iteration();

            i = 1;
            foreach (IMLCluster cluster in kmeans.Clusters)
            {
                IMLDataSet ds = cluster.CreateDataSet();
                IMLDataPair pair = BasicMLDataPair.CreatePair(ds.InputSize, ds.IdealSize);
                ds.GetRecord(0, pair);
                double t = pair.InputArray[0];

                for (int j = 0; j < ds.Count; j++)
                {
                    ds.GetRecord(j, pair);

                    for (j = 0; j < pair.InputArray.Length; j++)
                    {
                        if (t > 10)
                        {
                            Assert.IsTrue(pair.InputArray[j] > 10);
                        }
                        else
                        {
                            Assert.IsTrue(pair.InputArray[j] < 10);
                        }
                    }
                }

                i++;
            }
        }
    }
}
