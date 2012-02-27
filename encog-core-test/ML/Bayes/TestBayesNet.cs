using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.ML.Bayesian;

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
