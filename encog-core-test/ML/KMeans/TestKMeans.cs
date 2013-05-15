//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
                IMLDataPair pair;
                pair = ds[0];
                double t = pair.Input[0];

                for (int j = 0; j < ds.Count; j++)
                {
                    pair = ds[j];

                    for (j = 0; j < pair.Input.Count; j++)
                    {
                        if (t > 10)
                        {
                            Assert.IsTrue(pair.Input[j] > 10);
                        }
                        else
                        {
                            Assert.IsTrue(pair.Input[j] < 10);
                        }
                    }
                }

                i++;
            }
        }
    }
}
