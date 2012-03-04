using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Folded;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Cross;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util;
using Encog.Util.Simple;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Neural.Networks.Training
{
    [TestClass]
    public class TestFolded
    {
        [TestMethod]
        public void TestMethod1()
        {
        }


        public void TestRPROPFolded()
        {
            IMLDataSet trainingData = XOR.CreateNoisyXORDataSet(10);

            BasicNetwork network = NetworkUtil.CreateXORNetworkUntrained();

            var folded = new FoldedDataSet(trainingData);
            IMLTrain train = new ResilientPropagation(network, folded);
            var trainFolded = new CrossValidationKFold(train, 4);

            EncogUtility.TrainToError(trainFolded, 0.2);

            XOR.VerifyXOR((IMLRegression) trainFolded.Method, 0.2);
        }
    }
}