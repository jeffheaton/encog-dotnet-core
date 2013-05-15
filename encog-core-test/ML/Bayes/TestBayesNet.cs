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
using Encog.ML.Bayesian;
using Encog.ML.Bayesian.Query.Enumeration;

namespace Encog.ML.Bayes
{
    [TestClass]
    public class TestBayesNet
    {
        [TestMethod]
        public void TestCount()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent b = network.CreateEvent("b");
            BayesianEvent c = network.CreateEvent("c");
            BayesianEvent d = network.CreateEvent("d");
            BayesianEvent e = network.CreateEvent("e");
            network.CreateDependency(a, b, d, e);
            network.CreateDependency(c, d);
            network.CreateDependency(b, e);
            network.CreateDependency(d, e);
            network.FinalizeStructure();
            Assert.AreEqual(16, network.CalculateParameterCount());
        }

        [TestMethod]
        public void TestIndependant()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent b = network.CreateEvent("b");
            BayesianEvent c = network.CreateEvent("c");
            BayesianEvent d = network.CreateEvent("d");
            BayesianEvent e = network.CreateEvent("e");
            network.CreateDependency(a, b, d, e);
            network.CreateDependency(c, d);
            network.CreateDependency(b, e);
            network.CreateDependency(d, e);
            network.FinalizeStructure();

            Assert.IsFalse(network.IsCondIndependent(c, e, a));
            Assert.IsFalse(network.IsCondIndependent(b, d, c, e));
            Assert.IsFalse(network.IsCondIndependent(a, c, e));
            Assert.IsTrue(network.IsCondIndependent(a, c, b));
        }

        [TestMethod]
        public void TestIndependant2()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent b = network.CreateEvent("b");
            BayesianEvent c = network.CreateEvent("c");
            BayesianEvent d = network.CreateEvent("d");
            network.CreateDependency(a, b, c);
            network.CreateDependency(b, d);
            network.CreateDependency(c, d);
            network.FinalizeStructure();

            Assert.IsFalse(network.IsCondIndependent(b, c));
            Assert.IsFalse(network.IsCondIndependent(b, c, d));
            Assert.IsTrue(network.IsCondIndependent(a, c, a));
            Assert.IsFalse(network.IsCondIndependent(a, c, a, d));
        }       
    }
}
