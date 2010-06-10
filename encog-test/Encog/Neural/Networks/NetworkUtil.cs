using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Activation;
using Encog.MathUtil.Matrices;
using NUnit.Framework;
using Encog.MathUtil.Randomize;

namespace encog_test.Encog.Neural.Networks
{
    public class NetworkUtil
    {
        public static BasicNetwork CreateXORNetworkUntrained()
        {
            // random matrix data.  However, it provides a constant starting point 
            // for the unit tests.

            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 1));
            network.Structure.FinalizeStructure();
            (new ConsistentRandomizer(-1, 1)).Randomize(network);

            return network;
        }

        public static void TestTraining(ITrain train, double requiredImprove)
        {
            train.Iteration();
            double error1 = train.Error;

            for (int i = 0; i < 10; i++)
                train.Iteration();

            double error2 = train.Error;

            double improve = (error1 - error2) / error1;
            Assert.IsTrue(improve >= requiredImprove);
        }
    }
}
