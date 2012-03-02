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