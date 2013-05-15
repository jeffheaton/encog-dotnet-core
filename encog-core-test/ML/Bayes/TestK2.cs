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
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Bayesian;
using Encog.ML.Bayesian.Training;

namespace Encog.ML.Bayes
{
    [TestClass]
    public class TestK2
    {
        public readonly double[][] DATA = {
		    new double[] { 1, 0, 0 }, // case 1
		    new double[] { 1, 1, 1 }, // case 2
		    new double[] { 0, 0, 1 }, // case 3
		    new double[] { 1, 1, 1 }, // case 4
		    new double[] { 0, 0, 0 }, // case 5
		    new double[] { 0, 1, 1 }, // case 6
		    new double[] { 1, 1, 1 }, // case 7
		    new double[] { 0, 0, 0 }, // case 8
		    new double[] { 1, 1, 1 }, // case 9
		    new double[] { 0, 0, 0 }, // case 10		
	    };
	

        [TestMethod]
        public void TestK2Structure()
        {
            String[] labels = { "available", "not" };

            IMLDataSet data = new BasicMLDataSet(DATA, null);
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent x1 = network.CreateEvent("x1", labels);
            BayesianEvent x2 = network.CreateEvent("x2", labels);
            BayesianEvent x3 = network.CreateEvent("x3", labels);
            network.FinalizeStructure();
            TrainBayesian train = new TrainBayesian(network, data, 10);
            train.InitNetwork = BayesianInit.InitEmpty;
            while (!train.TrainingDone)
            {
                train.Iteration();
            }
            train.Iteration();
            Assert.IsTrue(x1.Parents.Count == 0);
            Assert.IsTrue(x2.Parents.Count == 1);
            Assert.IsTrue(x3.Parents.Count == 1);
            Assert.IsTrue(x2.Parents.Contains(x1));
            Assert.IsTrue(x3.Parents.Contains(x2));
            Assert.AreEqual(0.714, network.GetEvent("x2").Table.FindLine(1, new int[] { 1 }).Probability, 0.001);
		
        }
    }
}
