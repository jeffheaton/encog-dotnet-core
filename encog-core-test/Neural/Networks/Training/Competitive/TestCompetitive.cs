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
using Encog.MathUtil.Matrices;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.SOM;
using Encog.Neural.SOM.Training.Neighborhood;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Neural.Networks.Training.Competitive
{
    [TestClass]
    public class TestCompetitive
    {
        public static double[][] SOMInput = {
                                                 new[] {0.0, 0.0, 1.0, 1.0},
                                                 new[] {1.0, 1.0, 0.0, 0.0}
                                             };

        // Just a random starting matrix, but it gives us a constant starting point
        public static double[][] MatrixArray = {
                                                    new[]
                                                        {
                                                            0.9950675732277183, -0.09315692732658198, 0.9840257865083011,
                                                            0.5032129897356723
                                                        },
                                                    new[]
                                                        {
                                                            -0.8738960119753589, -0.48043680531294997, -0.9455207768842442,
                                                            -0.8612565984447569
                                                        }
                                                };

        [TestMethod]
        public void TestSOM()
        {
            // create the training set
            IMLDataSet training = new BasicMLDataSet(
                SOMInput, null);

            // Create the neural network.
            var network = new SOMNetwork(4, 2) {Weights = new Matrix(MatrixArray)};

            var train = new BasicTrainSOM(network, 0.4,
                                          training, new NeighborhoodSingle()) {ForceWinner = true};
            int iteration = 0;

            for (iteration = 0; iteration <= 100; iteration++)
            {
                train.Iteration();
            }

            IMLData data1 = new BasicMLData(
                SOMInput[0]);
            IMLData data2 = new BasicMLData(
                SOMInput[1]);

            int result1 = network.Classify(data1);
            int result2 = network.Classify(data2);

            Assert.IsTrue(result1 != result2);
        }
    }
}
