using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.MathUtil.Matrices.Hessian
{
    /// <summary>
    /// Summary description for TestHessian
    /// </summary>
    [TestClass]
    public class TestHessian
    {
        public TestHessian()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
        }



        [TestMethod]
        public void TestSingleOutput()
        {

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();

            (new ConsistentRandomizer(-1, 1)).Randomize(network);

            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);

            HessianFD testFD = new HessianFD();
            testFD.Init(network, trainingData);
            testFD.Compute();

            HessianCR testCR = new HessianCR();
            testCR.Init(network, trainingData);
            testCR.Compute();

            //dump(testFD, "FD");
            //dump(testCR, "CR");
            Assert.IsTrue(testCR.HessianMatrix.equals(testFD.HessianMatrix, 4));
        }

        [TestMethod]
        public void TestDualOutput()
        {

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 2));
            network.Structure.FinalizeStructure();

            (new ConsistentRandomizer(-1, 1)).Randomize(network);

            IMLDataSet trainingData = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal2);

            HessianFD testFD = new HessianFD();
            testFD.Init(network, trainingData);
            testFD.Compute();

            //dump(testFD, "FD");

            HessianCR testCR = new HessianCR();
            testCR.Init(network, trainingData);
            testCR.Compute();


            //dump(testCR, "CR");
            Assert.IsTrue(testCR.HessianMatrix.equals(testFD.HessianMatrix, 4));
        }
    }
}
